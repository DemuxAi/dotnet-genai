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
        Console.WriteLine($"\n{'='} Starting Stress Test {'='}\n");
        Console.WriteLine($"Test: {TestContext?.TestName ?? "Unknown"}");
        Console.WriteLine($"Configuration loaded: API Key present = {!string.IsNullOrEmpty(Config.ApiKey)}");

        // Initialize resource monitor with configured snapshot interval
        ResourceMonitor = new ResourceMonitor(Config.Monitoring.SnapshotIntervalSeconds);

        if (MockServerPort.HasValue)
        {
            ResourceMonitor.SetMockServerPort(MockServerPort.Value);
        }

        // Capture baseline
        var baseline = ResourceMonitor.CaptureBaselineSnapshot();
        Console.WriteLine($"Baseline: {baseline}");
    }

    [TestCleanup]
    public void BaseCleanup()
    {
        Console.WriteLine($"\n{'='} Test Complete {'='}\n");
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
        double? memoryThreshold = null,
        int? connectionThreshold = null,
        int? handleThreshold = null,
        int? threadThreshold = null)
    {
        if (ResourceMonitor == null)
        {
            throw new InvalidOperationException("ResourceMonitor not initialized");
        }

        var testName = TestContext?.TestName ?? "Unknown";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var reportName = $"{testName}-{timestamp}";
        var reportFolder = Path.Combine(Config.Reporting.OutputDirectory, reportName);
        var logFileName = Path.Combine(reportFolder, $"{reportName}.log");

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFolder(reportFolder)
            .WithReportFileName(reportName)
            .WithReportFormats(ReportFormat.Html, ReportFormat.Md)

            .Run();

        Console.WriteLine($"\nCooldown period: {Config.Monitoring.CooldownMinutes} minutes...");
        await Task.Delay(TimeSpan.FromMinutes(Config.Monitoring.CooldownMinutes));

        if (Config.Monitoring.ForceGCBeforeFinalSnapshot)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
        }

        var finalSnapshot = ResourceMonitor.CaptureSnapshot();
        Console.WriteLine($"Final: {finalSnapshot}");

        var totalRequests = stats.ScenarioStats.FirstOrDefault()?.Ok.Request.Count ?? 0;
        var leakAnalysis = ResourceMonitor.AnalyzeLeaks(totalRequests);

        var metrics = MetricsCollector.FromNBomberStats(
            stats,
            leakAnalysis,
            Config.Thresholds,
            testName,
            clientPattern.PatternName,
            loadPattern,
            memoryThreshold,
            connectionThreshold,
            handleThreshold,
            threadThreshold);

        Console.WriteLine("\n" + metrics.GetSummary());

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
        AssertNoResourceLeaks(metrics, memoryThreshold: Config.Thresholds.WebSocketMemoryGrowthRateBytesPerRequest);
    }

    /// <summary>
    /// Assert that no resource leaks were detected with customizable thresholds.
    /// Uses thresholds from metrics (if set during RunScenario) or falls back to args/config.
    /// </summary>
    protected void AssertNoResourceLeaks(
        MetricsCollector metrics,
        double? memoryThreshold = null,
        int? connectionThreshold = null,
        int? handleThreshold = null,
        int? threadThreshold = null)
    {
        var failures = new List<string>();

        // Priority:
        // 1. Explicit arguments to this method
        // 2. Thresholds recorded in metrics (from RunScenario)
        // 3. Global config
        var effectiveMemoryThreshold = memoryThreshold
            ?? metrics.AppliedThresholds?.MemoryGrowthRateBytesPerRequest
            ?? Config.Thresholds.MemoryGrowthRateBytesPerRequest;

        var effectiveConnectionThreshold = connectionThreshold
            ?? metrics.AppliedThresholds?.ConnectionLeakThreshold
            ?? Config.Thresholds.ConnectionLeakThreshold;

        var effectiveHandleThreshold = handleThreshold 
            ?? metrics.AppliedThresholds?.HandleLeakThreshold
            ?? Config.Thresholds.HandleLeakThreshold;

        var effectiveThreadThreshold = threadThreshold
            ?? metrics.AppliedThresholds?.ThreadLeakThreshold
            ?? Config.Thresholds.ThreadLeakThreshold;

        // Check memory leak
        if (metrics.MemoryGrowthRate > effectiveMemoryThreshold)
        {
            failures.Add($"Memory leak detected: {metrics.MemoryGrowthRate:F2} bytes/request " +
                        $"(threshold: {effectiveMemoryThreshold})");
        }

        // Check connection leak
        var connectionLeak = metrics.ConnectionsEnd - metrics.ConnectionsStart;
        if (connectionLeak > effectiveConnectionThreshold)
        {
            failures.Add($"Connection leak detected: {connectionLeak} leaked " +
                        $"(threshold: {effectiveConnectionThreshold})");
        }

        // Check handle leak
        var handleLeak = metrics.HandlesEnd - metrics.HandlesStart;
        if (handleLeak > effectiveHandleThreshold)
        {
            failures.Add($"Handle leak detected: {handleLeak} leaked " +
                        $"(threshold: {effectiveHandleThreshold})");
        }

        // Check thread leak
        var threadLeak = metrics.ThreadsEnd - metrics.ThreadsStart;
        if (threadLeak > effectiveThreadThreshold)
        {
            failures.Add($"Thread leak detected: {threadLeak} leaked " +
                        $"(threshold: {effectiveThreadThreshold})");
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
