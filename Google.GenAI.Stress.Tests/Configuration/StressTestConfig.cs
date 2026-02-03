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
        return new StressTestConfig
        {
            ApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") ?? string.Empty,
            Project = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT") ?? string.Empty
        };
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
    public LoadPatternConfig Light { get; set; } = new()
    {
        MaxConcurrent = 50,
        RampUpMinutes = 1,
        SustainMinutes = 1
    };

    public LoadPatternConfig Medium { get; set; } = new()
    {
        MaxConcurrent = 500,
        RampUpMinutes = 1,
        SustainMinutes = 10
    };

    public LoadPatternConfig Heavy { get; set; } = new()
    {
        MaxConcurrent = 1500,
        RampUpMinutes = 1,
        SustainMinutes = 720
    };
}

public class ThresholdsConfig
{
    // ============ Slope Thresholds (units per request) ============

    /// <summary>
    /// Memory growth slope threshold for HTTP scenarios (GenerateContent, etc.)
    /// Units: bytes per request from linear regression.
    /// </summary>
    public double MemorySlopeThreshold { get; set; } = 100;

    /// <summary>
    /// Memory growth slope threshold for WebSocket scenarios (Live API).
    /// WebSocket operations have higher memory overhead due to:
    /// - Connection state and buffers
    /// - Bidirectional message handling
    /// - Session management
    /// Units: bytes per request.
    /// </summary>
    public double WebSocketMemorySlopeThreshold { get; set; } = 1000;

    /// <summary>
    /// Connection leak slope threshold: connections per request.
    /// A slope of 0.001 means 1 connection leaked per 1000 requests.
    /// </summary>
    public double ConnectionSlopeThreshold { get; set; } = 0.001;

    /// <summary>
    /// Handle leak slope threshold: handles per request.
    /// A slope of 0.01 means 1 handle leaked per 100 requests.
    /// </summary>
    public double HandleSlopeThreshold { get; set; } = 0.01;

    /// <summary>
    /// Thread leak slope threshold: threads per request.
    /// A slope of 0.001 means 1 thread leaked per 1000 requests.
    /// </summary>
    public double ThreadSlopeThreshold { get; set; } = 0.001;

    // ============ R² Threshold (trend reliability) ============

    /// <summary>
    /// Minimum R² (coefficient of determination) required to consider a trend reliable.
    /// R² measures how well the linear model fits the data:
    /// - R² = 0: No linear relationship (pure noise)
    /// - R² = 0.3: Weak trend, might be noise
    /// - R² = 0.5: Moderate trend
    /// - R² = 1.0: Perfect linear relationship
    ///
    /// A leak is only flagged when: slope > threshold AND R² >= MinRSquared
    /// Default 0.3 filters out random fluctuations while catching real trends.
    /// </summary>
    public double MinRSquared { get; set; } = 0.3;

    // ============ Other Thresholds ============

    public int LatencyP95Milliseconds { get; set; } = 5000;
}

public class ScenarioConfig
{
    public string Model { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
}

public class ScenariosConfig
{
    public ScenarioConfig GenerateContent { get; set; } = new()
    {
        Model = "gemini-2.0-flash",
        Prompt = "What is 2+2? Please provide a brief answer."
    };

    public ScenarioConfig GenerateContentStream { get; set; } = new()
    {
        Model = "gemini-2.0-flash",
        Prompt = "Count from 1 to 10, one number per line."
    };

    public ScenarioConfig LiveApi { get; set; } = new()
    {
        Model = "gemini-2.0-flash-live-preview-04-09"
    };
}

public class MonitoringConfig
{
    public int SnapshotIntervalSeconds { get; set; } = 10;
    public double CooldownMinutes { get; set; } = 0.2;
    public bool ForceGCBeforeFinalSnapshot { get; set; } = true;
}

public class ReportingConfig
{
    public string OutputDirectory { get; set; } = "../../../Reports";
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
