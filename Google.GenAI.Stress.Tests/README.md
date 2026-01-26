# Google GenAI .NET SDK - Stress Tests

## Overview

This project contains comprehensive stress tests for the Google GenAI .NET SDK to identify resource leaks (memory, connections, handles) and measure performance under various load conditions.

**Primary Goal:** Verify that the .NET SDK properly manages resources (memory, connections, handles) across all usage patterns, including highly concurrent environments.

## Prerequisites

### Required Environment Variables

```bash
# For Gemini API
export GOOGLE_API_KEY="your-api-key-here"

# For Vertex AI (optional)
export GOOGLE_CLOUD_PROJECT="your-project-id"
export GOOGLE_CLOUD_LOCATION="us-central1"
```

### Authentication for Vertex AI

If testing against Vertex AI:

```bash
gcloud auth application-default login
```

## Project Structure

```
Google.GenAI.StressTests/
├── Configuration/
│   ├── StressTestConfig.cs          # Configuration model
│   └── appsettings.stress.json      # Load patterns, thresholds, scenarios
├── Infrastructure/
│   ├── ResourceMonitor.cs           # Tracks memory, connections, handles
│   ├── MetricsCollector.cs          # Aggregates performance metrics
│   ├── StressTestBase.cs            # Base class for all tests
│   └── LoadPatterns.cs              # Light/Medium/Heavy load definitions
├── ClientPatterns/
│   ├── IClientPattern.cs            # Pattern interface
│   ├── SingletonClientPattern.cs    # Pattern A: Recommended (shared client)
│   ├── ClientPerRequestPattern.cs   # Pattern B: Python SDK issue pattern
│   └── ClientPoolPattern.cs         # Pattern C: Alternative pattern
└── Scenarios/
    └── GenerateContentStressTests.cs # Critical tests for GenerateContent API
```

## Client Usage Patterns

### Pattern A: Singleton (Recommended)
- **What:** Single shared Client instance for all requests
- **Expected:** No leaks, optimal performance
- **Use Case:** Production recommended pattern

### Pattern B: Client Per Request
- **What:** Creates new Client for each request, disposes immediately
- **Expected:** No leaks (if disposal is correct)
- **Use Case:** Stress testing proper disposal logic and garbage collection

### Pattern C: Client Pool
- **What:** Pool of reusable Client instances
- **Expected:** No leaks, demonstrates viable alternative
- **Use Case:** Alternative to singleton for certain architectures

## Load Patterns

### Light Load
- **Concurrent Users:** 10-50
- **Duration:** 7 minutes (2 min ramp-up + 5 min sustained)
- **Estimated Requests:** ~21,000
- **Use Case:** Quick validation, initial leak detection

### Medium Load
- **Concurrent Users:** 100-500
- **Duration:** 15 minutes (5 min ramp-up + 10 min sustained)
- **Estimated Requests:** ~450,000
- **Use Case:** Moderate sustained load testing

### Heavy Load
- **Concurrent Users:** 1000-1500
- **Duration:** 25 minutes (10 min ramp-up + 15 min sustained)
- **Estimated Requests:** ~2,250,000
- **Use Case:** Extreme stress, reveals hidden leaks

**Total cost for full suite:** ~$13.60 per run

## Running Tests

### Run All Light Tests
```bash
cd Google.GenAI.StressTests
dotnet test --filter "TestCategory=Light"
```

### Run Specific Pattern
```bash
# Singleton pattern tests
dotnet test --filter "TestCategory=Singleton"

# Per-Request pattern tests (the critical one!)
dotnet test --filter "TestCategory=PerRequest"

# Pool pattern tests
dotnet test --filter "TestCategory=Pool"
```

### Run Specific Scenario
```bash
# All GenerateContent tests (non-streaming)
dotnet test --filter "TestCategory=GenerateContent"

# All GenerateContentStream tests (streaming)
dotnet test --filter "TestCategory=GenerateContentStream"

# All Live API tests (WebSocket)
dotnet test --filter "TestCategory=Live"

# Specific test method
dotnet test --filter "FullyQualifiedName~GenerateContent_ClientPerRequest_Light"
```

### Run by Load Level
```bash
dotnet test --filter "TestCategory=Light"
dotnet test --filter "TestCategory=Medium"
dotnet test --filter "TestCategory=Heavy"
```

### Run Recommended Test Sequence
```bash
# Start with Light GenerateContent to validate basic setup
dotnet test --filter "TestCategory=GenerateContent&TestCategory=Light"

# Test streaming with Light load
dotnet test --filter "TestCategory=GenerateContentStream&TestCategory=Light"

# Test Live API (most complex)
dotnet test --filter "TestCategory=Live&TestCategory=Light"

# If all pass, run Medium load tests
dotnet test --filter "TestCategory=Medium"
```

## Understanding Results

The following output is exemplary, actual numbers can vary based on test config.

### Console Output

During test execution, you'll see:
- Real-time resource snapshots every 10 seconds
- NBomber progress indicators
- Final summary with leak detection

### Example Output

```
Test: GenerateContent_ClientPerRequest_Light
Pattern: PerRequest
Load: Light
Duration: 7.2 minutes

Performance:
  Total Requests: 21,000
  Success Rate: 99.80%
  Throughput: 48.6 req/sec
  Latency P95: 1,250 ms

Resource Usage:
  Memory Start: 50.0 MB
  Memory End: 155.0 MB
  Memory Growth Rate: 5000 bytes/request ⚠️
  Connections: 5 → 15
  Threads: 12 → 14

Leak Detection:
  Memory Leak: ⚠️  YES (exceeds 100 bytes/request threshold)
  Connection Leak: ⚠️  YES (10 leaked connections)
  Handle Leak: ✅ No
  Thread Leak: ✅ No
```

### Reports

After each test run, reports are generated in `./reports/`:
- **HTML Report:** Interactive report with graphs (latency over time, throughput, etc.)
- **Markdown Report:** Text summary of results
- **Baseline JSON:** Saved in `./Baselines/` for regression detection

## Configuration

Edit `Configuration/appsettings.stress.json` to customize:

### Load Patterns
```json
"LoadPatterns": {
  "Light": {
    "MaxConcurrent": 50,
    "RampUpMinutes": 2,
    "SustainMinutes": 5
  }
}
```

### Leak Detection Thresholds
```json
"Thresholds": {
  "MemoryGrowthRateBytesPerRequest": 100,
  "ConnectionLeakThreshold": 10,
  "HandleLeakThreshold": 50,
  "LatencyP95Milliseconds": 5000
}
```

### Test Scenarios
```json
"Scenarios": {
  "GenerateContent": {
    "Model": "gemini-2.0-flash",
    "Prompt": "What is 2+2? Please provide a brief answer."
  }
}
```

## Interpreting Leak Detection

### Memory Leak
- **Threshold:** >100 bytes per request
- **Indicates:** Client/HttpClient not properly disposed, buffers accumulating
- **Action:** Review disposal patterns, check HttpClient lifecycle

### Connection Leak
- **Threshold:** Base threshold + Dynamic buffer (scales with concurrency)
- **Indicates:** Network connections not closed properly
- **Action:** Review HttpClient disposal, check for hanging requests

### Handle Leak
- **Threshold:** Dynamic (scales with concurrency)
- **Indicates:** File handles, mutex handles, or other OS resources not released
- **Action:** Review IDisposable implementation

### Thread Leak
- **Threshold:** Base threshold + Dynamic buffer (scales with concurrency)
- **Indicates:** Background threads not terminated, tasks not cleaned up
- **Action:** Review Task/Thread lifecycle, check for abandoned threads

## Expected Outcomes

**All Patterns (Singleton, Per-Request, Pool) are expected to PASS.**

### If Any Pattern Fails
**Critical Issue:** Resource leak detected in Client/ApiClient.
**Action Required:**
- Fix disposal implementation
- Review HttpClient lifecycle
- Check for unconsumed streams or abandoned tasks

## Troubleshooting

### "API Key not found"
Ensure `GOOGLE_API_KEY` environment variable is set:
```bash
export GOOGLE_API_KEY="your-key"
```

### "Rate limit errors (429)"
Tests include exponential backoff, but if persistent:
- Reduce concurrent users in `appsettings.stress.json`
- Increase ramp-up time
- Check your API quota limits

### "Out of Memory"
This might actually indicate a memory leak! Check:
- Test results for memory leak detection
- Consider running smaller load levels first

### Tests take too long
Start with Light tests:
```bash
dotnet test --filter "TestCategory=Light"
```

## Development

### Adding New Scenarios

1. Create a new test class in `Scenarios/`:
```csharp
[TestClass]
public class MyApiStressTests : StressTestBase
{
    [TestMethod]
    [TestCategory("Light")]
    [TestCategory("MyApi")]
    public async Task MyApi_Singleton_Light()
    {
        // Implementation
    }
}
```

2. Create scenario using NBomber:
```csharp
var scenario = Scenario.Create("MyScenario", async context =>
{
    // Your API call logic
    return Response.Ok();
});
```

3. Run with load pattern:
```csharp
var metrics = await RunScenario(scenario, clientPattern, "Light");
AssertNoResourceLeaks(metrics);
```

## Best Practices

1. **Start with Light tests** - Validate basic functionality before expensive runs
2. **Monitor quota** - Each test consumes API quota, plan accordingly
3. **Compare baselines** - Save baselines after each SDK version change
4. **Run periodically** - Monthly or before major releases
5. **Investigate failures** - Don't ignore leak warnings, even small ones compound

## Mock Server Mode (Default)

Stress tests now support two modes: **mock mode** (default) and **live mode**.

### Mock Mode (Default) - Free & Fast

By default, tests run against a local ASP.NET Core mock server that replays recorded responses.

**Benefits:**
- Free, unlimited test runs (no API costs)
- High concurrency (500+ concurrent requests) without rate limiting
- Consistent, reproducible results
- Fast feedback loop

**Running in mock mode:**
```bash
# Default - no environment variable needed
dotnet test Google.GenAI.StressTests --filter "TestCategory=GenerateContent&TestCategory=Light"

# Or explicitly set
export STRESS_TEST_MODE=mock
dotnet test Google.GenAI.StressTests
```

### Live Mode - Real API Testing

For validation against the actual API (occasionally recommended):

```bash
export STRESS_TEST_MODE=live
export GOOGLE_API_KEY="your-api-key"
dotnet test Google.GenAI.StressTests --filter "TestCategory=Light"
```

**Note:** Live API tests incur costs (~$13.60 for full suite). Live mode is recommended for:
- Initial validation of mock server behavior
- Periodic validation against real API

### Live API Tests (WebSocket)

The mock server includes WebSocket support for Live API tests. The WebSocket mock provides:
- `SetupComplete` response on connection
- Simple echo responses for realtime input
- `TurnComplete` signals
- Proper WebSocket close handling

This is sufficient for stress testing WebSocket lifecycle and disposal:

```bash
# Run Live API tests in mock mode (free)
dotnet test Google.GenAI.StressTests --filter "TestCategory=Live"

# Or in live mode for real API validation
export STRESS_TEST_MODE=live
export GOOGLE_API_KEY="your-api-key"
dotnet test Google.GenAI.StressTests --filter "TestCategory=Live"
```

---

## Mock Server Architecture

```
Recording Phase (one-time):
  Test Runner → E2E Test Server (port 1453) → Real Gemini API
                     ↓
              Recordings/*.json

Stress Test Phase (repeatable, free):
  NBomber → ASP.NET Mock Server (dynamic port) → Recorded JSON responses
                     ↓
              SDK Resource Leak Detection
```

### Why This Works for SDK Leak Detection

The stress tests measure **SDK behavior**, not API behavior:
- Memory leaks when creating/disposing Client instances
- Connection leaks (HttpClient not properly released)
- Handle/thread leaks

The mock server exercises the same SDK code paths:
- HTTP request construction
- Response deserialization
- Client lifecycle management

We don't need real API latency or varied responses - we need **consistent, high-volume** request/response cycles to detect leaks.

### Mock Server Configuration

Configure in `appsettings.stress.json`:

```json
"MockServer": {
  "Enabled": true,
  "RecordingsDirectory": "./Recordings",
  "SimulateLatency": false,
  "LatencyMs": 50
}
```

### Updating Recordings

To update mock response recordings:

1. Run E2E tests in record mode to capture real API responses
2. Copy relevant responses to `Recordings/` directory
3. Format as:
   - `GenerateContent_Response.json` - Single JSON object
   - `GenerateContentStream_Response.json` - JSON array of chunks

---