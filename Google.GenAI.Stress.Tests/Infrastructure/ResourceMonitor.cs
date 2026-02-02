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
    private readonly List<ResourceSnapshot> _inScenarioSnapshots = new();
    private readonly Timer? _snapshotTimer;
    private int _requestCount;
    private int? _mockServerPort;
    private long _lastCapturedMilestone = 0;

    /// <summary>
    /// When true, forces a full GC before every resource snapshot.
    /// This ensures accurate memory measurements by reclaiming garbage before each measurement.
    /// </summary>
    public bool ForceGCBeforeSnapshots { get; set; } = true;

    /// <summary>
    /// The "test end" snapshot captured immediately after the test completes,
    /// before any post-processing overhead. Used for accurate leak analysis.
    /// </summary>
    private ResourceSnapshot? _testEndSnapshot;

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
        if (ForceGCBeforeSnapshots)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
        }

        _currentProcess.Refresh();

        // snapshot resources used by this .NET application, and tcp connections to Google's API or mock server port
        var snapshot = new ResourceSnapshot
        {
            Timestamp = DateTime.UtcNow,
            WorkingSetBytes = _currentProcess.WorkingSet64,
            PrivateMemoryBytes = _currentProcess.PrivateMemorySize64,
            ManagedMemoryBytes = GC.GetTotalMemory(forceFullCollection: false),
            ThreadCount = GetThreadCountSafe(),
            HandleCount = GetHandleCountSafe(),
            TcpConnectionCount = GetActiveTcpConnections(),
            RequestCount = _requestCount
        };

        lock (_snapshots)
        {
            _snapshots.Add(snapshot);
        }

        return snapshot;
    }

    /// <summary>
    /// Gets thread count safely.
    /// </summary>
    private int GetThreadCountSafe()
    {
        try
        {
            if (OperatingSystem.IsLinux())
            {
                // On Linux, counting /proc/self/task is much faster and more stable than Process.Threads
                // which creates a ProcessThread object for every thread and can fail under load.
                if (Directory.Exists("/proc/self/task"))
                {
                    return Directory.GetDirectories("/proc/self/task").Length;
                }
            }
            return _currentProcess.Threads.Count;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Gets handle/file descriptor count - cross-platform.
    /// Windows: Process.HandleCount
    /// Linux: Count entries in /proc/self/fd
    /// macOS: Falls back to 0
    /// </summary>
    private int GetHandleCountSafe()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                return _currentProcess.HandleCount;
            }
            else if (OperatingSystem.IsLinux())
            {
                // Count open file descriptors from /proc/self/fd
                // This is more reliable than Process.HandleCount which can crash with NRE on some Linux environments
                var fdPath = "/proc/self/fd";
                if (Directory.Exists(fdPath))
                {
                    return Directory.GetFiles(fdPath).Length;
                }
            }
            // Fallback for other OS (e.g., macOS)
            return 0;
        }
        catch
        {
            return 0;
        }
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
        lock (_inScenarioSnapshots)
        {
            _inScenarioSnapshots.Clear();
        }
        _lastCapturedMilestone = 0;
        _testEndSnapshot = null;
        CaptureBaselineSnapshot();
    }

    /// <summary>
    /// Capture a snapshot from within the scenario at specific milestones.
    /// Thread-safe - only one thread captures at each milestone.
    /// This captures measurements AFTER SDK operations complete but BEFORE returning to NBomber,
    /// providing more accurate measurements that exclude NBomber's internal tracking overhead.
    /// </summary>
    /// <param name="snapshotInterval">Capture a snapshot every N requests (default: 100)</param>
    public void CaptureInScenarioSnapshot(int snapshotInterval = 100)
    {
        var currentRequest = _requestCount;
        var milestone = (currentRequest / snapshotInterval) * snapshotInterval;

        // Only capture at milestones (100, 200, 300...) and only once per milestone
        if (milestone == 0 || milestone <= Interlocked.Read(ref _lastCapturedMilestone))
            return;

        // Atomically claim this milestone
        var expectedPrevious = milestone - snapshotInterval;
        if (Interlocked.CompareExchange(ref _lastCapturedMilestone, milestone, expectedPrevious) != expectedPrevious)
            return;

        // Force GC and capture
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);

        _currentProcess.Refresh();

        var snapshot = new ResourceSnapshot
        {
            Timestamp = DateTime.UtcNow,
            WorkingSetBytes = _currentProcess.WorkingSet64,
            PrivateMemoryBytes = _currentProcess.PrivateMemorySize64,
            ManagedMemoryBytes = GC.GetTotalMemory(forceFullCollection: false),
            ThreadCount = _currentProcess.Threads.Count,
            HandleCount = _currentProcess.HandleCount,
            TcpConnectionCount = GetActiveTcpConnections(),
            RequestCount = currentRequest
        };

        lock (_inScenarioSnapshots)
        {
            _inScenarioSnapshots.Add(snapshot);
        }
    }

    /// <summary>
    /// Captures the "test end" snapshot immediately after the test completes.
    /// This should be called right after NBomber.Run() returns, before any post-processing.
    /// The snapshot is stored separately and used for leak analysis to exclude test infrastructure overhead.
    /// </summary>
    public ResourceSnapshot CaptureTestEndSnapshot()
    {
        // Stop the periodic timer to prevent further snapshots during post-processing
        _snapshotTimer?.Change(Timeout.Infinite, Timeout.Infinite);

        // Force full GC to reclaim SDK resources and get accurate measurement
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);

        _currentProcess.Refresh();

        _testEndSnapshot = new ResourceSnapshot
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

        return _testEndSnapshot;
    }

    /// <summary>
    /// Gets the test end snapshot if captured, otherwise null.
    /// </summary>
    public ResourceSnapshot? TestEndSnapshot => _testEndSnapshot;

    public List<ResourceSnapshot> GetSnapshots()
    {
        lock (_snapshots)
        {
            return new List<ResourceSnapshot>(_snapshots);
        }
    }

    public List<ResourceSnapshot> GetInScenarioSnapshots()
    {
        lock (_inScenarioSnapshots)
        {
            return new List<ResourceSnapshot>(_inScenarioSnapshots);
        }
    }

    public LeakAnalysis AnalyzeLeaks(int totalRequests)
    {
        List<ResourceSnapshot> allSnapshots;

        // Use in-scenario snapshots (captured after SDK operations, before NBomber tracking)
        lock (_inScenarioSnapshots)
        {
            allSnapshots = new List<ResourceSnapshot>(_inScenarioSnapshots);
        }

        // Need at least 2 snapshots for meaningful analysis
        if (allSnapshots.Count < 2)
        {
            return new LeakAnalysis { InsufficientData = true };
        }

        // Analyze only the last 50% of snapshots to focus on the "sustain" phase and ignore ramp-up.
        // For Light (1m ramp + 1m sustain), this analyzes the sustain phase.
        // For Heavy (1m ramp + 10m sustain), this analyzes the last 5m of sustain.
        // This avoids false positives where the ramp-up slope is interpreted as a leak.
        var snapshotsToAnalyze = allSnapshots.Count >= 4 
            ? allSnapshots.Skip(allSnapshots.Count / 2).ToList() 
            : allSnapshots;

        var first = allSnapshots.First(); // Report full range stats based on all data
        var last = allSnapshots.Last();

        var requestCounts = snapshotsToAnalyze.Select(s => (double)s.RequestCount).ToList();

        var (memorySlope, memoryR2) = CalculateLinearRegression(
            requestCounts,
            snapshotsToAnalyze.Select(s => (double)s.ManagedMemoryBytes).ToList());

        var (connectionSlope, connectionR2) = CalculateLinearRegression(
            requestCounts,
            snapshotsToAnalyze.Select(s => (double)s.TcpConnectionCount).ToList());

        var (handleSlope, handleR2) = CalculateLinearRegression(
            requestCounts,
            snapshotsToAnalyze.Select(s => (double)s.HandleCount).ToList());

        var (threadSlope, threadR2) = CalculateLinearRegression(
            requestCounts,
            snapshotsToAnalyze.Select(s => (double)s.ThreadCount).ToList());

        return new LeakAnalysis
        {
            MemoryTrendSlope = memorySlope,
            MemoryRSquared = memoryR2,
            ConnectionTrendSlope = connectionSlope,
            ConnectionRSquared = connectionR2,
            HandleTrendSlope = handleSlope,
            HandleRSquared = handleR2,
            ThreadTrendSlope = threadSlope,
            ThreadRSquared = threadR2,
            FirstSnapshot = first,
            LastSnapshot = last,
            Snapshots = allSnapshots,
            SnapshotCount = allSnapshots.Count
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
    /// Calculate linear regression slope and R² (coefficient of determination) for trend analysis.
    /// R² indicates how well the linear model fits the data (0 = no fit, 1 = perfect fit).
    /// </summary>
    private (double Slope, double RSquared) CalculateLinearRegression(List<double> x, List<double> y)
    {
        if (x.Count != y.Count || x.Count < 2)
            return (0, 0);

        var n = x.Count;
        var sumX = x.Sum();
        var sumY = y.Sum();
        var sumXY = x.Zip(y, (xi, yi) => xi * yi).Sum();
        var sumX2 = x.Sum(xi => xi * xi);
        var sumY2 = y.Sum(yi => yi * yi);

        var denominator = n * sumX2 - sumX * sumX;
        if (Math.Abs(denominator) < double.Epsilon)
            return (0, 0); // No variance in X data

        // Calculate slope
        var slope = (n * sumXY - sumX * sumY) / denominator;

        // Calculate R² (coefficient of determination)
        // R² = (n * sumXY - sumX * sumY)² / [(n * sumX2 - sumX²) * (n * sumY2 - sumY²)]
        var numeratorR = n * sumXY - sumX * sumY;
        var denominatorY = n * sumY2 - sumY * sumY;

        double rSquared = 0;
        if (Math.Abs(denominatorY) > double.Epsilon)
        {
            var r = numeratorR / Math.Sqrt(denominator * denominatorY);
            rSquared = r * r;
        }

        return (slope, rSquared);
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

    // Slope values (units per request from linear regression)
    public double MemoryTrendSlope { get; set; } // Bytes per request
    public double ConnectionTrendSlope { get; set; } // Connections per request
    public double HandleTrendSlope { get; set; } // Handles per request
    public double ThreadTrendSlope { get; set; } // Threads per request

    // R² values (coefficient of determination: 0 = no trend, 1 = perfect trend)
    public double MemoryRSquared { get; set; }
    public double ConnectionRSquared { get; set; }
    public double HandleRSquared { get; set; }
    public double ThreadRSquared { get; set; }

    public ResourceSnapshot? FirstSnapshot { get; set; }
    public ResourceSnapshot? LastSnapshot { get; set; }
    public List<ResourceSnapshot>? Snapshots { get; set; }
    public int SnapshotCount { get; set; }

    /// <summary>
    /// Checks for memory leak: slope exceeds threshold AND R² indicates reliable trend.
    /// </summary>
    public bool HasMemoryLeak(double slopeThreshold, double minRSquared) =>
        MemoryTrendSlope > slopeThreshold && MemoryRSquared >= minRSquared;

    /// <summary>
    /// Checks for connection leak: slope exceeds threshold AND R² indicates reliable trend.
    /// </summary>
    public bool HasConnectionLeak(double slopeThreshold, double minRSquared) =>
        ConnectionTrendSlope > slopeThreshold && ConnectionRSquared >= minRSquared;

    /// <summary>
    /// Checks for handle leak: slope exceeds threshold AND R² indicates reliable trend.
    /// </summary>
    public bool HasHandleLeak(double slopeThreshold, double minRSquared) =>
        HandleTrendSlope > slopeThreshold && HandleRSquared >= minRSquared;

    /// <summary>
    /// Checks for thread leak: slope exceeds threshold AND R² indicates reliable trend.
    /// </summary>
    public bool HasThreadLeak(double slopeThreshold, double minRSquared) =>
        ThreadTrendSlope > slopeThreshold && ThreadRSquared >= minRSquared;

    /// <summary>
    /// Checks for any leak using slope + R² analysis.
    /// </summary>
    public bool HasAnyLeak(double memorySlopeThreshold, double connectionSlopeThreshold,
        double handleSlopeThreshold, double threadSlopeThreshold, double minRSquared)
    {
        return HasMemoryLeak(memorySlopeThreshold, minRSquared) ||
               HasConnectionLeak(connectionSlopeThreshold, minRSquared) ||
               HasHandleLeak(handleSlopeThreshold, minRSquared) ||
               HasThreadLeak(threadSlopeThreshold, minRSquared);
    }
}
