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

using NBomber.Contracts.Stats;

namespace Google.GenAI.StressTests.Infrastructure;

/// <summary>
/// Collects and aggregates performance and resource metrics from stress tests
/// </summary>
public class MetricsCollector
{
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public double RequestsPerSecond { get; set; }
    public TimeSpan LatencyP50 { get; set; }
    public TimeSpan LatencyP95 { get; set; }
    public TimeSpan LatencyP99 { get; set; }
    public TimeSpan LatencyMax { get; set; }
    public TimeSpan LatencyMin { get; set; }
    public TimeSpan LatencyMean { get; set; }

    // Resource metrics
    public long MemoryStartBytes { get; set; }
    public long MemoryEndBytes { get; set; }
    public long MemoryPeakBytes { get; set; }

    public int ConnectionsStart { get; set; }
    public int ConnectionsEnd { get; set; }
    public int ConnectionsPeak { get; set; }

    public int ThreadsStart { get; set; }
    public int ThreadsEnd { get; set; }

    public int HandlesStart { get; set; }
    public int HandlesEnd { get; set; }

    // Slope-based metrics (from linear regression: units per request)
    public double MemorySlope { get; set; } // bytes per request
    public double ConnectionSlope { get; set; } // connections per request
    public double HandleSlope { get; set; } // handles per request
    public double ThreadSlope { get; set; } // threads per request

    // R² values (coefficient of determination: 0 = no trend, 1 = perfect trend)
    public double MemoryRSquared { get; set; }
    public double ConnectionRSquared { get; set; }
    public double HandleRSquared { get; set; }
    public double ThreadRSquared { get; set; }

    // Error tracking
    public int RateLimitErrors { get; set; }
    public int TimeoutErrors { get; set; }
    public int OtherErrors { get; set; }

    // Leak detection flags
    public bool MemoryLeakDetected { get; set; }
    public bool ConnectionLeakDetected { get; set; }
    public bool HandleLeakDetected { get; set; }
    public bool ThreadLeakDetected { get; set; }

    // Thresholds used for analysis
    public Configuration.ThresholdsConfig? AppliedThresholds { get; set; }

    // Test metadata
    public string TestName { get; set; } = string.Empty;
    public string ClientPattern { get; set; } = string.Empty;
    public string LoadPattern { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;

    public double SuccessRate => TotalRequests > 0 ? (SuccessfulRequests * 100.0 / TotalRequests) : 0;

    public static MetricsCollector FromNBomberStats(NodeStats stats, LeakAnalysis leakAnalysis,
        Configuration.ThresholdsConfig globalThresholds, string testName, string clientPattern, string loadPattern,
        double? customMemoryThreshold = null,
        double? customConnectionThreshold = null,
        double? customHandleThreshold = null,
        double? customThreadThreshold = null)
    {
        var scenarioStats = stats.ScenarioStats.FirstOrDefault();
        if (scenarioStats == null)
        {
            return new MetricsCollector { TestName = testName };
        }

        var okStats = scenarioStats.Ok;
        var failStats = scenarioStats.Fail;

        var metrics = new MetricsCollector
        {
            TestName = testName,
            ClientPattern = clientPattern,
            LoadPattern = loadPattern,
            StartTime = stats.TestInfo.SessionId.StartsWith("stress_test_")
                ? DateTime.UtcNow.AddMinutes(-scenarioStats.Duration.TotalMinutes)
                : DateTime.UtcNow,
            EndTime = DateTime.UtcNow
        };

        // Performance metrics from NBomber
        if (okStats != null)
        {
            metrics.TotalRequests = okStats.Request.Count + (failStats?.Request.Count ?? 0);
            metrics.SuccessfulRequests = okStats.Request.Count;
            metrics.FailedRequests = failStats?.Request.Count ?? 0;
            metrics.RequestsPerSecond = okStats.Request.RPS;

            metrics.LatencyP50 = TimeSpan.FromMilliseconds(okStats.Latency.Percent50);
            metrics.LatencyP95 = TimeSpan.FromMilliseconds(okStats.Latency.Percent95);
            metrics.LatencyP99 = TimeSpan.FromMilliseconds(okStats.Latency.Percent99);
            metrics.LatencyMax = TimeSpan.FromMilliseconds(okStats.Latency.MaxMs);
            metrics.LatencyMin = TimeSpan.FromMilliseconds(okStats.Latency.MinMs);
            metrics.LatencyMean = TimeSpan.FromMilliseconds(okStats.Latency.MeanMs);
        }

        if (!leakAnalysis.InsufficientData && leakAnalysis.FirstSnapshot != null && leakAnalysis.LastSnapshot != null)
        {
            metrics.MemoryStartBytes = leakAnalysis.FirstSnapshot.ManagedMemoryBytes;
            metrics.MemoryEndBytes = leakAnalysis.LastSnapshot.ManagedMemoryBytes;

            metrics.ConnectionsStart = leakAnalysis.FirstSnapshot.TcpConnectionCount;
            metrics.ConnectionsEnd = leakAnalysis.LastSnapshot.TcpConnectionCount;

            metrics.ThreadsStart = leakAnalysis.FirstSnapshot.ThreadCount;
            metrics.ThreadsEnd = leakAnalysis.LastSnapshot.ThreadCount;

            metrics.HandlesStart = leakAnalysis.FirstSnapshot.HandleCount;
            metrics.HandlesEnd = leakAnalysis.LastSnapshot.HandleCount;

            if (leakAnalysis.Snapshots?.Count > 0)
            {
                metrics.MemoryPeakBytes = leakAnalysis.Snapshots.Max(s => s.ManagedMemoryBytes);
                metrics.ConnectionsPeak = leakAnalysis.Snapshots.Max(s => s.TcpConnectionCount);
            }

            // Slope-based metrics from linear regression
            metrics.MemorySlope = leakAnalysis.MemoryTrendSlope;
            metrics.ConnectionSlope = leakAnalysis.ConnectionTrendSlope;
            metrics.HandleSlope = leakAnalysis.HandleTrendSlope;
            metrics.ThreadSlope = leakAnalysis.ThreadTrendSlope;

            // R² values from linear regression
            metrics.MemoryRSquared = leakAnalysis.MemoryRSquared;
            metrics.ConnectionRSquared = leakAnalysis.ConnectionRSquared;
            metrics.HandleRSquared = leakAnalysis.HandleRSquared;
            metrics.ThreadRSquared = leakAnalysis.ThreadRSquared;

            // Determine effective thresholds
            var memoryThreshold = customMemoryThreshold ?? globalThresholds.MemorySlopeThreshold;
            var connectionThreshold = customConnectionThreshold ?? globalThresholds.ConnectionSlopeThreshold;
            var handleThreshold = customHandleThreshold ?? globalThresholds.HandleSlopeThreshold;
            var threadThreshold = customThreadThreshold ?? globalThresholds.ThreadSlopeThreshold;
            var minRSquared = globalThresholds.MinRSquared;

            // Store effective thresholds for reference
            metrics.AppliedThresholds = new Configuration.ThresholdsConfig
            {
                MemorySlopeThreshold = memoryThreshold,
                ConnectionSlopeThreshold = connectionThreshold,
                HandleSlopeThreshold = handleThreshold,
                ThreadSlopeThreshold = threadThreshold,
                MinRSquared = minRSquared,
                // Copy others
                WebSocketMemorySlopeThreshold = globalThresholds.WebSocketMemorySlopeThreshold,
                LatencyP95Milliseconds = globalThresholds.LatencyP95Milliseconds
            };

            // Leak detection: slope > threshold AND R² >= minRSquared
            metrics.MemoryLeakDetected = leakAnalysis.HasMemoryLeak(memoryThreshold, minRSquared);
            metrics.ConnectionLeakDetected = leakAnalysis.HasConnectionLeak(connectionThreshold, minRSquared);
            metrics.HandleLeakDetected = leakAnalysis.HasHandleLeak(handleThreshold, minRSquared);
            metrics.ThreadLeakDetected = leakAnalysis.HasThreadLeak(threadThreshold, minRSquared);
        }

        return metrics;
    }

    public bool HasAnyLeak()
    {
        return MemoryLeakDetected || ConnectionLeakDetected ||
               HandleLeakDetected || ThreadLeakDetected;
    }

    public string GetSummary()
    {
        var minR2 = AppliedThresholds?.MinRSquared ?? 0.3;
        return $"""
            Test: {TestName}
            Pattern: {ClientPattern}
            Load: {LoadPattern}
            Duration: {Duration.TotalMinutes:F1} minutes

            Performance:
              Total Requests: {TotalRequests:N0}
              Success Rate: {SuccessRate:F2}%
              Throughput: {RequestsPerSecond:F2} req/sec
              Latency P50: {LatencyP50.TotalMilliseconds:F0} ms
              Latency P95: {LatencyP95.TotalMilliseconds:F0} ms
              Latency P99: {LatencyP99.TotalMilliseconds:F0} ms

            Resource Usage:
              Memory: {MemoryStartBytes / 1024.0 / 1024.0:F1} MB -> {MemoryEndBytes / 1024.0 / 1024.0:F1} MB (Peak: {MemoryPeakBytes / 1024.0 / 1024.0:F1} MB)
              Connections: {ConnectionsStart} -> {ConnectionsEnd} (Peak: {ConnectionsPeak})
              Threads: {ThreadsStart} -> {ThreadsEnd}
              Handles: {HandlesStart} -> {HandlesEnd}

            Trend Analysis (slope + R2):
              Memory:     slope={MemorySlope:F2} bytes/req, R2={MemoryRSquared:F3}
              Connection: slope={ConnectionSlope:F6} conn/req, R2={ConnectionRSquared:F3}
              Handle:     slope={HandleSlope:F6} handles/req, R2={HandleRSquared:F3}
              Thread:     slope={ThreadSlope:F6} threads/req, R2={ThreadRSquared:F3}

            Leak Detection (slope > threshold AND R2 >= {minR2}):
              Memory Leak: {(MemoryLeakDetected ? "YES" : "No")}
              Connection Leak: {(ConnectionLeakDetected ? "YES" : "No")}
              Handle Leak: {(HandleLeakDetected ? "YES" : "No")}
              Thread Leak: {(ThreadLeakDetected ? "YES" : "No")}
            """;
    }
}
