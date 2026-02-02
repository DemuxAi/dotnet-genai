/*
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NBomber.CSharp;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using Google.GenAI.StressTests.Configuration;
using Google.GenAI.StressTests.ClientPatterns;
using Google.GenAI.StressTests.MockServer;

namespace Google.GenAI.StressTests.Infrastructure;

/// <summary>
/// Per-test configuration overrides for RunScenario.
/// All properties are optional - null means use global config default.
/// </summary>
public class RunScenarioOptions
{
    // Threshold overrides (slope-based: units per request from linear regression)
    public double? MemoryThreshold { get; set; }
    public double? ConnectionThreshold { get; set; }
    public double? HandleThreshold { get; set; }
    public double? ThreadThreshold { get; set; }

    /// <summary>
    /// Force GC before every resource snapshot (default: true).
    /// This ensures accurate memory measurements throughout the test
    /// by reclaiming garbage before each measurement.
    /// </summary>
    public bool ForceGCBeforeSnapshots { get; set; } = true;
}

/// <summary>
/// Base class for all stress test scenarios
/// Provides common functionality for resource monitoring, scenario execution, and reporting
/// </summary>
[TestClass]
public abstract class StressTestBase
{
    protected ResourceMonitor? ResourceMonitor { get; private set; }
    protected StressTestConfig Config => StressTestConfig.Instance;

    /// <summary>
    /// Shared mock server instance for all tests in a class
    /// </summary>
    protected static StressMockServer? MockServer { get; private set; }

    /// <summary>
    /// The mock server URL if running in mock mode, null otherwise
    /// </summary>
    protected static string? MockServerUrl => MockServer?.BaseUrl;

    /// <summary>
    /// The mock server port if running in mock mode, null otherwise
    /// </summary>
    protected static int? MockServerPort => MockServer?.Port;

    /// <summary>
    /// Initialize the mock server for the test class.
    /// Call this from [ClassInitialize] in derived test classes.
    /// </summary>
    protected static async Task InitializeMockServerAsync()
    {
        var config = StressTestConfig.Instance;
        if (StressTestConfig.IsMockMode && config.MockServer.Enabled)
        {
            MockServer = new StressMockServer(config.MockServer);
            await MockServer.StartAsync();
            Console.WriteLine($"[StressTestBase] Mock mode enabled, server at: {MockServer.BaseUrl}");

            // Ensure GOOGLE_API_KEY is set to avoid Client validation error
            // when GOOGLE_CLOUD_PROJECT/LOCATION are present in the environment
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_API_KEY")))
            {
                Environment.SetEnvironmentVariable("GOOGLE_API_KEY", "mock-api-key");
                Console.WriteLine("[StressTestBase] Set mock GOOGLE_API_KEY to satisfy Client validation");
            }
        }
        else
        {
            Console.WriteLine("[StressTestBase] Live mode - using real API");
        }
    }

    /// <summary>
    /// Cleanup the mock server when the test class is done.
    /// Call this from [ClassCleanup] in derived test classes.
    /// </summary>
    protected static void CleanupMockServer()
    {
        MockServer?.Dispose();
        MockServer = null;
    }

    /// <summary>
    /// Create and configure a client pattern with the mock server URL if in mock mode.
    /// </summary>
    protected T CreateClientPattern<T>() where T : IClientPattern, new()
    {
        var pattern = new T();
        pattern.Configure(MockServerUrl);
        return pattern;
    }

    /// <summary>
    /// Create and configure a ClientPoolPattern with the specified pool size.
    /// </summary>
    protected ClientPoolPattern CreateClientPoolPattern(int poolSize = 10)
    {
        var pattern = new ClientPoolPattern(poolSize);
        pattern.Configure(MockServerUrl);
        return pattern;
    }

    [TestInitialize]
    public void BaseSetup()
    {
        // Initialize resource monitor with configured snapshot interval
        ResourceMonitor = new ResourceMonitor(Config.Monitoring.SnapshotIntervalSeconds);

        if (MockServerPort.HasValue)
        {
            ResourceMonitor.SetMockServerPort(MockServerPort.Value);
        }

        // Capture baseline
        ResourceMonitor.CaptureBaselineSnapshot();
    }

    [TestCleanup]
    public void BaseCleanup()
    {
        ResourceMonitor?.Dispose();
    }

    public TestContext? TestContext { get; set; }

    /// <summary>
    /// Run a stress test scenario with resource monitoring
    /// </summary>
    protected async Task<MetricsCollector> RunScenario(
        ScenarioProps scenario,
        IClientPattern clientPattern,
        string loadPattern,
        RunScenarioOptions? options = null)
    {
        if (ResourceMonitor == null)
        {
            throw new InvalidOperationException("ResourceMonitor not initialized");
        }

        options ??= new RunScenarioOptions();

        // Configure ResourceMonitor with options
        ResourceMonitor.ForceGCBeforeSnapshots = options.ForceGCBeforeSnapshots;

        var testName = TestContext?.TestName ?? "Unknown";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var reportName = $"{testName}-{timestamp}";
        var reportFolder = Path.Combine(Config.Reporting.OutputDirectory, reportName);

        // Create report folder
        Directory.CreateDirectory(reportFolder);

        // Run NBomber without its built-in reports (we generate our own)
        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .WithoutReports()
            .Run();

        // Immediately capture test end snapshot after NBomber finishes
        // This excludes post-processing overhead from leak analysis
        ResourceMonitor.CaptureTestEndSnapshot();

        var totalRequests = stats.ScenarioStats.FirstOrDefault()?.Ok.Request.Count ?? 0;
        var leakAnalysis = ResourceMonitor.AnalyzeLeaks(totalRequests);

        var metrics = MetricsCollector.FromNBomberStats(
            stats,
            leakAnalysis,
            Config.Thresholds,
            testName,
            clientPattern.PatternName,
            loadPattern,
            options.MemoryThreshold,
            options.ConnectionThreshold,
            options.HandleThreshold,
            options.ThreadThreshold);

        Console.WriteLine("\n" + metrics.GetSummary());

        // Generate our custom HTML report with resource charts
        var snapshots = ResourceMonitor.GetInScenarioSnapshots();
        var reportHtml = ReportGenerator.GenerateHtml(metrics, snapshots, leakAnalysis);
        var reportPath = Path.Combine(reportFolder, $"{reportName}.html");
        await File.WriteAllTextAsync(reportPath, reportHtml);
        Console.WriteLine($"Report saved: {reportPath}");

        if (Config.Reporting.SaveBaseline)
        {
            await SaveBaseline(metrics);
        }

        return metrics;
    }

    /// <summary>
    /// Assert that no resource leaks were detected for WebSocket scenarios
    /// (uses higher threshold appropriate for WebSocket overhead)
    /// </summary>
    protected void AssertNoResourceLeaksWebSocket(MetricsCollector metrics)
    {
        AssertNoResourceLeaks(metrics, memoryThreshold: Config.Thresholds.WebSocketMemorySlopeThreshold);
    }

    /// <summary>
    /// Assert that no resource leaks were detected with customizable thresholds.
    /// Uses slope + R² analysis: leak detected when slope > threshold AND R² >= minRSquared.
    /// Thresholds from metrics (if set during RunScenario) or falls back to args/config.
    /// </summary>
    protected void AssertNoResourceLeaks(
        MetricsCollector metrics,
        double? memoryThreshold = null,
        double? connectionThreshold = null,
        double? handleThreshold = null,
        double? threadThreshold = null)
    {
        var failures = new List<string>();

        // Priority:
        // 1. Explicit arguments to this method
        // 2. Thresholds recorded in metrics (from RunScenario)
        // 3. Global config
        var effectiveMemoryThreshold = memoryThreshold
            ?? metrics.AppliedThresholds?.MemorySlopeThreshold
            ?? Config.Thresholds.MemorySlopeThreshold;

        var effectiveConnectionThreshold = connectionThreshold
            ?? metrics.AppliedThresholds?.ConnectionSlopeThreshold
            ?? Config.Thresholds.ConnectionSlopeThreshold;

        var effectiveHandleThreshold = handleThreshold
            ?? metrics.AppliedThresholds?.HandleSlopeThreshold
            ?? Config.Thresholds.HandleSlopeThreshold;

        var effectiveThreadThreshold = threadThreshold
            ?? metrics.AppliedThresholds?.ThreadSlopeThreshold
            ?? Config.Thresholds.ThreadSlopeThreshold;

        var minRSquared = metrics.AppliedThresholds?.MinRSquared
            ?? Config.Thresholds.MinRSquared;

        // Check memory leak (slope > threshold AND R² >= minRSquared)
        if (metrics.MemorySlope > effectiveMemoryThreshold && metrics.MemoryRSquared >= minRSquared)
        {
            failures.Add($"Memory leak detected: slope={metrics.MemorySlope:F2} bytes/req, R2={metrics.MemoryRSquared:F3} " +
                        $"(threshold: slope>{effectiveMemoryThreshold}, R2>={minRSquared})");
        }

        // Check connection leak
        if (metrics.ConnectionSlope > effectiveConnectionThreshold && metrics.ConnectionRSquared >= minRSquared)
        {
            failures.Add($"Connection leak detected: slope={metrics.ConnectionSlope:F6} conn/req, R2={metrics.ConnectionRSquared:F3} " +
                        $"(threshold: slope>{effectiveConnectionThreshold}, R2>={minRSquared})");
        }

        // Check handle leak
        if (metrics.HandleSlope > effectiveHandleThreshold && metrics.HandleRSquared >= minRSquared)
        {
            failures.Add($"Handle leak detected: slope={metrics.HandleSlope:F6} handles/req, R2={metrics.HandleRSquared:F3} " +
                        $"(threshold: slope>{effectiveHandleThreshold}, R2>={minRSquared})");
        }

        // Check thread leak
        if (metrics.ThreadSlope > effectiveThreadThreshold && metrics.ThreadRSquared >= minRSquared)
        {
            failures.Add($"Thread leak detected: slope={metrics.ThreadSlope:F6} threads/req, R2={metrics.ThreadRSquared:F3} " +
                        $"(threshold: slope>{effectiveThreadThreshold}, R2>={minRSquared})");
        }

        if (failures.Any())
        {
            Assert.Fail("Resource leaks detected:\n" + string.Join("\n", failures));
        }
    }

    /// <summary>
    /// Assert acceptable latency
    /// </summary>
    protected void AssertAcceptableLatency(MetricsCollector metrics, TimeSpan? p95Threshold = null)
    {
        var threshold = p95Threshold ?? TimeSpan.FromMilliseconds(Config.Thresholds.LatencyP95Milliseconds);

        if (metrics.LatencyP95 > threshold)
        {
            Assert.Fail($"P95 latency {metrics.LatencyP95.TotalMilliseconds:F0}ms exceeds threshold {threshold.TotalMilliseconds:F0}ms");
        }
    }

    /// <summary>
    /// Assert minimum success rate
    /// </summary>
    protected void AssertSuccessRate(MetricsCollector metrics, double minimumSuccessRate = 95.0)
    {
        if (metrics.SuccessRate < minimumSuccessRate)
        {
            Assert.Fail($"Success rate {metrics.SuccessRate:F2}% is below minimum {minimumSuccessRate}%");
        }
    }

    /// <summary>
    /// Save baseline results for regression detection
    /// </summary>
    private async Task SaveBaseline(MetricsCollector metrics)
    {
        try
        {
            var baselineDir = Path.Combine(Config.Reporting.OutputDirectory, "..", "Baselines");
            Directory.CreateDirectory(baselineDir);

            var sdkVersion = typeof(Client).Assembly.GetName().Version?.ToString() ?? "unknown";
            var fileName = $"v{sdkVersion}-{metrics.TestName}-{metrics.ClientPattern}-{metrics.LoadPattern}.json";
            var filePath = Path.Combine(baselineDir, fileName);

            var json = System.Text.Json.JsonSerializer.Serialize(metrics, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
            Console.WriteLine($"\nBaseline saved: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nWarning: Could not save baseline: {ex.Message}");
        }
    }
}
