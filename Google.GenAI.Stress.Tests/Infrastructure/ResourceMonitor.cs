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

using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Google.GenAI.StressTests.Infrastructure;

/// <summary>
/// Monitors system resources (memory, connections, handles) to detect leaks
/// </summary>
public class ResourceMonitor
{
    private readonly Process _currentProcess;
    private readonly List<ResourceSnapshot> _snapshots;
    private readonly Timer? _snapshotTimer;
    private int _requestCount;
    private int? _mockServerPort;

    public ResourceMonitor()
    {
        _currentProcess = Process.GetCurrentProcess();
        _snapshots = new List<ResourceSnapshot>();
    }

    public ResourceMonitor(int snapshotIntervalSeconds) : this()
    {
        _snapshotTimer = new Timer(
            _ => CaptureSnapshot(),
            null,
            TimeSpan.FromSeconds(snapshotIntervalSeconds),  // First snapshot after interval
            TimeSpan.FromSeconds(snapshotIntervalSeconds));
    }

    public void IncrementRequestCount() => Interlocked.Increment(ref _requestCount);

    /// <summary>
    /// Set the mock server port for connection tracking in mock mode.
    /// </summary>
    public void SetMockServerPort(int port) => _mockServerPort = port;

    public ResourceSnapshot CaptureSnapshot()
    {
        _currentProcess.Refresh();

        // snapshot resources used by this .NET application, and tcp connections to Google's API or mock server port
        var snapshot = new ResourceSnapshot
        {
            Timestamp = DateTime.UtcNow,
            WorkingSetBytes = _currentProcess.WorkingSet64,
            PrivateMemoryBytes = _currentProcess.PrivateMemorySize64,
            ManagedMemoryBytes = GC.GetTotalMemory(forceFullCollection: false),
            ThreadCount = _currentProcess.Threads.Count,
            HandleCount = _currentProcess.HandleCount,
            TcpConnectionCount = GetActiveTcpConnections(),
            RequestCount = _requestCount
        };

        lock (_snapshots)
        {
            _snapshots.Add(snapshot);
        }

        return snapshot;
    }

    public ResourceSnapshot CaptureBaselineSnapshot()
    {
        // Force GC to get clean baseline
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);

        return CaptureSnapshot();
    }

    /// <summary>
    /// Clears all previous snapshots and captures a new baseline.
    /// Useful when initialization (like Client creation) should be excluded from leak detection.
    /// </summary>
    public void ResetBaseline()
    {
        lock (_snapshots)
        {
            _snapshots.Clear();
        }
        CaptureBaselineSnapshot();
    }

    public List<ResourceSnapshot> GetSnapshots()
    {
        lock (_snapshots)
        {
            return new List<ResourceSnapshot>(_snapshots);
        }
    }

    public LeakAnalysis AnalyzeLeaks(int totalRequests)
    {
        List<ResourceSnapshot> snapshots;
        lock (_snapshots)
        {
            snapshots = new List<ResourceSnapshot>(_snapshots);
        }

        if (snapshots.Count < 2)
        {
            return new LeakAnalysis { InsufficientData = true };
        }

        var first = snapshots.First();
        var last = snapshots.Last();

        var memoryGrowth = last.ManagedMemoryBytes - first.ManagedMemoryBytes;
        var memoryGrowthRate = totalRequests > 0 ? (double)memoryGrowth / totalRequests : 0;

        var memorySlope = CalculateLinearRegressionSlope(
            snapshots.Select(s => (double)s.RequestCount).ToList(),
            snapshots.Select(s => (double)s.ManagedMemoryBytes).ToList());

        var connectionLeak = last.TcpConnectionCount - first.TcpConnectionCount;

        var handleLeak = last.HandleCount - first.HandleCount;

        var threadLeak = last.ThreadCount - first.ThreadCount;

        return new LeakAnalysis
        {
            MemoryGrowthBytes = memoryGrowth,
            MemoryGrowthRate = memoryGrowthRate,
            MemoryTrendSlope = memorySlope,
            ConnectionLeak = connectionLeak,
            HandleLeak = handleLeak,
            ThreadLeak = threadLeak,
            FirstSnapshot = first,
            LastSnapshot = last,
            Snapshots = snapshots,
            SnapshotCount = snapshots.Count
        };
    }

    private int GetActiveTcpConnections()
    {
        try
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = properties.GetActiveTcpConnections();

            return connections.Count(c =>
            {
                if (c.State != TcpState.Established)
                    return false;

                var endpoint = c.RemoteEndPoint.ToString() ?? "";

                if (endpoint.Contains("googleapis.com") || endpoint.Contains("generativelanguage"))
                    return true;

                if (_mockServerPort.HasValue)
                {
                    // Check port and loopback address (handles both IPv4 127.0.0.1 and IPv6 [::1])
                    if (c.RemoteEndPoint.Port == _mockServerPort.Value)
                    {
                        if (System.Net.IPAddress.IsLoopback(c.RemoteEndPoint.Address))
                            return true;
                    }
                }

                return false;
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ResourceMonitor] Warning: Failed to get active TCP connections: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Calculate linear regression slope for trend analysis
    /// </summary>
    private double CalculateLinearRegressionSlope(List<double> x, List<double> y)
    {
        if (x.Count != y.Count || x.Count < 2)
            return 0;

        var n = x.Count;
        var sumX = x.Sum();
        var sumY = y.Sum();
        var sumXY = x.Zip(y, (xi, yi) => xi * yi).Sum();
        var sumX2 = x.Sum(xi => xi * xi);

        var denominator = n * sumX2 - sumX * sumX;
        if (Math.Abs(denominator) < double.Epsilon)
            return 0.0; // No variance in data
        var slope = (n * sumXY - sumX * sumY) / denominator;
        return slope;
    }

    public void Dispose()
    {
        _snapshotTimer?.Dispose();
    }
}

public class ResourceSnapshot
{
    public DateTime Timestamp { get; set; }
    public long WorkingSetBytes { get; set; }
    public long PrivateMemoryBytes { get; set; }
    public long ManagedMemoryBytes { get; set; }
    public int ThreadCount { get; set; }
    public int HandleCount { get; set; }
    public int TcpConnectionCount { get; set; }
    public int RequestCount { get; set; }

    public double WorkingSetMB => WorkingSetBytes / 1024.0 / 1024.0;
    public double PrivateMemoryMB => PrivateMemoryBytes / 1024.0 / 1024.0;
    public double ManagedMemoryMB => ManagedMemoryBytes / 1024.0 / 1024.0;

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] Memory: {ManagedMemoryMB:F1}MB, " +
               $"Connections: {TcpConnectionCount}, Threads: {ThreadCount}, " +
               $"Handles: {HandleCount}, Requests: {RequestCount}";
    }
}

public class LeakAnalysis
{
    public bool InsufficientData { get; set; }
    public long MemoryGrowthBytes { get; set; }
    public double MemoryGrowthRate { get; set; } // Bytes per request
    public double MemoryTrendSlope { get; set; }
    public int ConnectionLeak { get; set; }
    public int HandleLeak { get; set; }
    public int ThreadLeak { get; set; }
    public ResourceSnapshot? FirstSnapshot { get; set; }
    public ResourceSnapshot? LastSnapshot { get; set; }
    public List<ResourceSnapshot>? Snapshots { get; set; }
    public int SnapshotCount { get; set; }

    public bool HasMemoryLeak(double threshold) => MemoryGrowthRate > threshold;
    public bool HasConnectionLeak(int threshold) => ConnectionLeak > threshold;
    public bool HasHandleLeak(int threshold) => HandleLeak > threshold;
    public bool HasThreadLeak(int threshold) => ThreadLeak > threshold;

    public bool HasAnyLeak(double memoryThreshold, int connectionThreshold,
        int handleThreshold, int threadThreshold)
    {
        return HasMemoryLeak(memoryThreshold) ||
               HasConnectionLeak(connectionThreshold) ||
               HasHandleLeak(handleThreshold) ||
               HasThreadLeak(threadThreshold);
    }
}
