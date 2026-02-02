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

using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Google.GenAI.StressTests.Infrastructure;

/// <summary>
/// Generates HTML reports with resource usage charts for stress test results.
/// Chart.js is embedded for offline viewing - no internet required.
/// </summary>
public static class ReportGenerator
{
    private static readonly Lazy<string> ChartJsSource = new(() =>
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Google.GenAI.Stress.Tests.Infrastructure.Resources.chart.min.js");
        if (stream == null)
            throw new InvalidOperationException("chart.min.js embedded resource not found");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    });

    public static string GenerateHtml(MetricsCollector metrics, List<ResourceSnapshot> snapshots, LeakAnalysis leakAnalysis)
    {
        var chartJs = ChartJsSource.Value;
        var snapshotsJson = JsonSerializer.Serialize(snapshots.Select(s => new
        {
            s.RequestCount,
            s.ManagedMemoryBytes,
            ManagedMemoryMB = s.ManagedMemoryMB,
            s.TcpConnectionCount,
            s.HandleCount,
            s.ThreadCount,
            Timestamp = s.Timestamp.ToString("HH:mm:ss")
        }));

        var thresholds = metrics.AppliedThresholds;

        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Stress Test Report - {metrics.TestName}</title>
    <script>{chartJs}</script>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, monospace; padding: 20px; max-width: 1200px; margin: 0 auto; background: #fafafa; }}
        h1 {{ margin-bottom: 20px; font-size: 1.5em; }}
        h2 {{ margin: 30px 0 15px; font-size: 1.2em; border-bottom: 1px solid #ddd; padding-bottom: 5px; }}
        table {{ border-collapse: collapse; width: 100%; margin-bottom: 20px; background: white; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 0.9em; }}
        th {{ background: #f5f5f5; }}
        .pass {{ color: #2e7d32; font-weight: bold; }}
        .fail {{ color: #c62828; font-weight: bold; }}
        .chart-container {{ background: white; padding: 15px; margin-bottom: 20px; border: 1px solid #ddd; }}
        canvas {{ max-height: 300px; }}
        details {{ margin: 20px 0; }}
        summary {{ cursor: pointer; padding: 10px; background: #f5f5f5; border: 1px solid #ddd; }}
        pre {{ background: #f5f5f5; padding: 15px; overflow-x: auto; font-size: 0.8em; max-height: 400px; overflow-y: auto; }}
        button {{ padding: 8px 16px; margin: 10px 5px 10px 0; cursor: pointer; background: #1976d2; color: white; border: none; font-size: 0.9em; }}
        button:hover {{ background: #1565c0; }}
        .meta {{ color: #666; font-size: 0.85em; margin-bottom: 20px; }}
    </style>
</head>
<body>
    <h1>Stress Test Report: {metrics.TestName}</h1>
    <p class=""meta"">Pattern: {metrics.ClientPattern} | Load: {metrics.LoadPattern} | Duration: {metrics.Duration.TotalMinutes:F1} min | Requests: {metrics.TotalRequests:N0} | Snapshots: {snapshots.Count}</p>

    <h2>Leak Detection Results</h2>
    <table>
        <tr>
            <th>Resource</th>
            <th>Slope</th>
            <th>R²</th>
            <th>Threshold</th>
            <th>Min R²</th>
            <th>Result</th>
        </tr>
        <tr>
            <td>Memory</td>
            <td>{metrics.MemorySlope:F2} bytes/req</td>
            <td>{metrics.MemoryRSquared:F3}</td>
            <td>{thresholds?.MemorySlopeThreshold ?? 100} bytes/req</td>
            <td>{thresholds?.MinRSquared ?? 0.3}</td>
            <td class=""{(metrics.MemoryLeakDetected ? "fail" : "pass")}"">{(metrics.MemoryLeakDetected ? "LEAK" : "OK")}</td>
        </tr>
        <tr>
            <td>Connections</td>
            <td>{metrics.ConnectionSlope:F6} conn/req</td>
            <td>{metrics.ConnectionRSquared:F3}</td>
            <td>{thresholds?.ConnectionSlopeThreshold ?? 0.001} conn/req</td>
            <td>{thresholds?.MinRSquared ?? 0.3}</td>
            <td class=""{(metrics.ConnectionLeakDetected ? "fail" : "pass")}"">{(metrics.ConnectionLeakDetected ? "LEAK" : "OK")}</td>
        </tr>
        <tr>
            <td>Handles</td>
            <td>{metrics.HandleSlope:F6} handles/req</td>
            <td>{metrics.HandleRSquared:F3}</td>
            <td>{thresholds?.HandleSlopeThreshold ?? 0.01} handles/req</td>
            <td>{thresholds?.MinRSquared ?? 0.3}</td>
            <td class=""{(metrics.HandleLeakDetected ? "fail" : "pass")}"">{(metrics.HandleLeakDetected ? "LEAK" : "OK")}</td>
        </tr>
        <tr>
            <td>Threads</td>
            <td>{metrics.ThreadSlope:F6} threads/req</td>
            <td>{metrics.ThreadRSquared:F3}</td>
            <td>{thresholds?.ThreadSlopeThreshold ?? 0.001} threads/req</td>
            <td>{thresholds?.MinRSquared ?? 0.3}</td>
            <td class=""{(metrics.ThreadLeakDetected ? "fail" : "pass")}"">{(metrics.ThreadLeakDetected ? "LEAK" : "OK")}</td>
        </tr>
    </table>

    <h2>Resource Usage Summary</h2>
    <table>
        <tr><th>Metric</th><th>Start</th><th>End</th><th>Peak</th></tr>
        <tr><td>Memory (MB)</td><td>{metrics.MemoryStartBytes / 1024.0 / 1024.0:F1}</td><td>{metrics.MemoryEndBytes / 1024.0 / 1024.0:F1}</td><td>{metrics.MemoryPeakBytes / 1024.0 / 1024.0:F1}</td></tr>
        <tr><td>Connections</td><td>{metrics.ConnectionsStart}</td><td>{metrics.ConnectionsEnd}</td><td>{metrics.ConnectionsPeak}</td></tr>
        <tr><td>Handles</td><td>{metrics.HandlesStart}</td><td>{metrics.HandlesEnd}</td><td>-</td></tr>
        <tr><td>Threads</td><td>{metrics.ThreadsStart}</td><td>{metrics.ThreadsEnd}</td><td>-</td></tr>
    </table>

    <h2>Memory vs Requests</h2>
    <div class=""chart-container""><canvas id=""memoryChart""></canvas></div>

    <h2>Connections vs Requests</h2>
    <div class=""chart-container""><canvas id=""connectionChart""></canvas></div>

    <h2>Handles vs Requests</h2>
    <div class=""chart-container""><canvas id=""handleChart""></canvas></div>

    <h2>Threads vs Requests</h2>
    <div class=""chart-container""><canvas id=""threadChart""></canvas></div>

    <h2>Raw Data</h2>
    <button onclick=""downloadCsv()"">Download CSV</button>
    <details>
        <summary>View JSON ({snapshots.Count} snapshots)</summary>
        <pre id=""rawJson"">{snapshotsJson}</pre>
    </details>

    <script>
        const snapshots = {snapshotsJson};

        function createChart(canvasId, label, yKey, color) {{
            const ctx = document.getElementById(canvasId).getContext('2d');
            new Chart(ctx, {{
                type: 'scatter',
                data: {{
                    datasets: [{{
                        label: label,
                        data: snapshots.map(s => ({{ x: s.RequestCount, y: s[yKey] }})),
                        backgroundColor: color,
                        pointRadius: 4
                    }}]
                }},
                options: {{
                    responsive: true,
                    plugins: {{ legend: {{ display: false }} }},
                    scales: {{
                        x: {{ title: {{ display: true, text: 'Request Count' }} }},
                        y: {{ title: {{ display: true, text: label }}, beginAtZero: false }}
                    }}
                }}
            }});
        }}

        createChart('memoryChart', 'Memory (MB)', 'ManagedMemoryMB', '#1976d2');
        createChart('connectionChart', 'Connections', 'TcpConnectionCount', '#388e3c');
        createChart('handleChart', 'Handles', 'HandleCount', '#f57c00');
        createChart('threadChart', 'Threads', 'ThreadCount', '#7b1fa2');

        function downloadCsv() {{
            const headers = ['RequestCount', 'Timestamp', 'ManagedMemoryMB', 'TcpConnectionCount', 'HandleCount', 'ThreadCount'];
            const rows = snapshots.map(s => [s.RequestCount, s.Timestamp, s.ManagedMemoryMB.toFixed(2), s.TcpConnectionCount, s.HandleCount, s.ThreadCount]);
            const csv = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
            const blob = new Blob([csv], {{ type: 'text/csv' }});
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = '{metrics.TestName}-snapshots.csv';
            a.click();
            URL.revokeObjectURL(url);
        }}
    </script>
</body>
</html>";
    }
}
