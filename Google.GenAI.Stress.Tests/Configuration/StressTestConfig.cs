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

using Microsoft.Extensions.Configuration;

namespace Google.GenAI.StressTests.Configuration;

public class StressTestConfig
{
    private static StressTestConfig? _instance;
    private static readonly object _lock = new object();

    public string ApiKey { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Location { get; set; } = "us-central1";
    public bool VertexAI { get; set; } = false;

    public LoadPatternsConfig LoadPatterns { get; set; } = new();
    public ThresholdsConfig Thresholds { get; set; } = new();
    public ScenariosConfig Scenarios { get; set; } = new();
    public MonitoringConfig Monitoring { get; set; } = new();
    public ReportingConfig Reporting { get; set; } = new();
    public MockServerConfig MockServer { get; set; } = new();

    /// <summary>
    /// Check if stress tests should run in mock mode (using local mock server)
    /// Default is mock mode. Set STRESS_TEST_MODE=live for real API calls.
    /// </summary>
    public static bool IsMockMode =>
        (Environment.GetEnvironmentVariable("STRESS_TEST_MODE") ?? "mock") == "mock";

    public static StressTestConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = Load();
                    }
                }
            }
            return _instance;
        }
    }

    private static StressTestConfig Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("Configuration/appsettings.stress.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var config = new StressTestConfig();
        configuration.GetSection("StressTest").Bind(config);

        if (config.ApiKey.StartsWith("env:"))
        {
            var envVar = config.ApiKey.Substring(4);
            config.ApiKey = Environment.GetEnvironmentVariable(envVar) ?? string.Empty;
        }

        if (config.Project.StartsWith("env:"))
        {
            var envVar = config.Project.Substring(4);
            config.Project = Environment.GetEnvironmentVariable(envVar) ?? string.Empty;
        }

        return config;
    }
}

public class LoadPatternConfig
{
    public int MaxConcurrent { get; set; }
    public double RampUpMinutes { get; set; }
    public double SustainMinutes { get; set; }

    public double TotalDurationMinutes => RampUpMinutes + SustainMinutes;
}

public class LoadPatternsConfig
{
    public LoadPatternConfig Light { get; set; } = new();
    public LoadPatternConfig Medium { get; set; } = new();
    public LoadPatternConfig Heavy { get; set; } = new();
}

public class ThresholdsConfig
{
    /// <summary>
    /// Memory growth threshold for HTTP scenarios (GenerateContent, etc.)
    /// </summary>
    public double MemoryGrowthRateBytesPerRequest { get; set; } = 100;

    /// <summary>
    /// Memory growth threshold for WebSocket scenarios (Live API).
    /// WebSocket operations have higher memory overhead due to:
    /// - Connection state and buffers
    /// - Bidirectional message handling
    /// - Session management
    /// Typical baseline: 500-1000 bytes/request is acceptable.
    /// </summary>
    public double WebSocketMemoryGrowthRateBytesPerRequest { get; set; } = 1000;

    public int ConnectionLeakThreshold { get; set; } = 10;
    public int HandleLeakThreshold { get; set; } = 50;
    public int ThreadLeakThreshold { get; set; } = 20;
    public int LatencyP95Milliseconds { get; set; } = 5000;
}

public class ScenarioConfig
{
    public string Model { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
}

public class ScenariosConfig
{
    public ScenarioConfig GenerateContent { get; set; } = new();
    public ScenarioConfig GenerateContentStream { get; set; } = new();
    public ScenarioConfig LiveApi { get; set; } = new();
}

public class MonitoringConfig
{
    public int SnapshotIntervalSeconds { get; set; } = 10;
    public double CooldownMinutes { get; set; } = 5;
    public bool ForceGCBeforeFinalSnapshot { get; set; } = true;
}

public class ReportingConfig
{
    public string OutputDirectory { get; set; } = "./reports";
    public bool GenerateHtml { get; set; } = true;
    public bool GenerateMarkdown { get; set; } = true;
    public bool SaveBaseline { get; set; } = true;
}

public class MockServerConfig
{
    /// <summary>
    /// Enable or disable the mock server. When disabled, tests use real API.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Directory containing recorded response JSON files (relative to config or absolute).
    /// </summary>
    public string RecordingsDirectory { get; set; } = "./Recordings";

    /// <summary>
    /// Gets the resolved recordings directory path (relative to assembly location).
    /// </summary>
    public string ResolvedRecordingsDirectory
    {
        get
        {
            if (Path.IsPathRooted(RecordingsDirectory))
                return RecordingsDirectory;

            // Resolve relative to assembly location, not working directory
            var assemblyLocation = Path.GetDirectoryName(typeof(MockServerConfig).Assembly.Location);
            return Path.Combine(assemblyLocation ?? ".", RecordingsDirectory);
        }
    }

    /// <summary>
    /// Simulate network latency in mock responses.
    /// </summary>
    public bool SimulateLatency { get; set; } = false;

    /// <summary>
    /// Latency in milliseconds when SimulateLatency is true.
    /// </summary>
    public int LatencyMs { get; set; } = 50;
}
