# Google GenAI .NET SDK - Stress Tests

## Overview

This project provides a comprehensive stress testing suite for the Google GenAI .NET SDK. Its primary purpose is to mathematically prove that the SDK manages resources correctly under sustained, high-concurrency load.

**Primary Goal:** Verify that the .NET SDK properly manages resources (memory, connections, handles, threads) across all usage patterns and prevents regressions.

---

## 1. Test Philosophy & Methodology

### Resources Monitored
We strictly monitor four critical OS resources:
1.  **Memory:** Managed heap + unmanaged allocations.
2.  **Connections:** Active TCP connections (critical for HTTP/WebSocket).
3.  **Handles:** OS file descriptors/handles.
4.  **Threads:** ThreadPool usage and background threads.

### The "Plateau Pattern" Hypothesis
A healthy SDK should demonstrate a **Plateau Pattern** in resource consumption.
- Resources should grow during ramp-up as concurrency increases.
- Once max concurrency is reached, resource usage should **plateau** (fluctuate around a constant mean).
- Resources should **not** grow proportionally to the *cumulative* number of requests.
- **Any linear growth trend after the plateau indicates a leak.**

```
Resource Usage (Memory, Connections, Handles)
  ^
  |         ┌─────────────────────┐  <- Plateau (proportional to max concurrency)
  |        /                       \
  |       /   sawtooth from GC      \
  |      /                           \
  |     /                             \
  |────/                               \────  <- Returns near baseline
  +──────────────────────────────────────────> Time
       ^ ramp-up    ^ sustain         ^ cooldown
```

### Accurate Measurement: In-Scenario Snapshots
**CRITICAL:** We do **not** rely on the load testing framework (NBomber) for resource measurements. NBomber's own tracking objects and timers introduce significant noise and false positives.

Instead, we use **In-Scenario Snapshots**:
1.  **Inside the Test Lambda:** We execute the SDK operation.
2.  **After Disposal:** We explicitly dispose the SDK resources.
3.  **Snapshot:** We capture the resource snapshot **immediately**, before returning control to the test framework.
4.  **Forced GC:** We force a full Garbage Collection before every snapshot to measure *surviving* objects, not garbage.

This ensures we measure the *SDK's* residual footprint, not the *Test Runner's* overhead.

### Leak Detection Algorithm: Slope + R²
We use statistical linear regression (Slope + R²) as a **loose, automated assertion** to flag potential regressions.

However, **binary Pass/Fail results are not fully insightful.**
True leak analysis requires human inspection of the raw data.
*   **The automated check** filters out obvious noise (low R²) and obvious leaks (high slope + high R²).
*   **The definitive analysis** is found in the **HTML Report Scatter Plots** generated in the `Reports/` directory.
    *   Look for the **shape** of the data.
    *   A healthy SDK shows a flat plateau (random noise around a mean).
    *   A leak shows a clear, consistent upward trajectory in the scatter plot, which is often visible to the eye even if the statistical model is borderline.

### STRICT No-Logging Policy
**Unnecessary logs (Console.WriteLine) are STRICTLY FORBIDDEN inside the test scenarios.**
- Console I/O is expensive, consumes locks/handles, and allocates memory.
- Excessive logging alters the timing and resource profile of the test, creating false positives (heisenbugs).
- Logs are only allowed for fatal errors or final summary reports.

---

## 2. Test Dimensions

We test across a matrix of **Client Patterns** and **API Scenarios**.

### Client Patterns
We validate three distinct lifecycle patterns to ensure the SDK works in all architectural styles:

| Pattern | Description | Use Case |
| :--- | :--- | :--- |
| **A: Singleton** | Single shared `Client` instance. | **Recommended** for production. Checks for contention/thread-safety. |
| **B: Per-Request** | New `Client` per request, disposed immediately. | **Stress Test**. Checks `Dispose()` logic and HTTP connection pooling cleanup. |
| **C: Pool** | Pool of reusable `Client` instances. | Alternative architecture. Checks reuse logic. |

### API Scenarios
We test the full range of API modalities:
1.  **GenerateContent:** Unary HTTP requests. Standard request/response.
2.  **GenerateContentStream:** Server-sent events (streaming). Tests stream reader disposal.
3.  **Live API:** Bidirectional WebSocket streaming. Tests complex `AsyncSession` lifecycles and connection state.

---

## 3. Running Tests

### Quick Start
```bash
# Run the critical "Per-Request" pattern (most likely to leak)
dotnet test --filter "TestCategory=PerRequest"
```

### Common Commands
```bash
# Run all tests with "Light" load (Fastest validation)
dotnet test --filter "TestCategory=Light"

# Run all WebSocket/Live tests
dotnet test --filter "TestCategory=Live"

# Run a specific scenario combination
dotnet test --filter "TestCategory=GenerateContent&TestCategory=Singleton"
```

### Mock Mode vs. Live Mode
By default, tests run against a local **Mock Server** (included) to ensure zero-cost, high-concurrency reproducibility.
To run against the real Google Cloud API:
```bash
export STRESS_TEST_MODE=live
export GOOGLE_API_KEY="your-key"
dotnet test
```

---

## 4. Interpreting Results

The test output provides a "Trend Analysis" for each resource.

**Example Healthy Result:**
```
Trend Analysis (slope + R2):
  Memory:     slope=0.15 bytes/req, R2=0.045   <-- Low R2 = Noise (Good)
  Connection: slope=0.00 conn/req,  R2=0.012   <-- Zero slope = Flat (Good)
```

**Example Leak:**
```
Trend Analysis (slope + R2):
  Memory:     slope=150.5 bytes/req, R2=0.95   <-- High Slope + High R2 = LEAK!
```
*Action:* If a leak is detected, check the `Dispose()` logic of the Client or the specific API resource (e.g., `GenerativeModel`, `IAsyncEnumerator`).

---

## 5. Configuration & Load Profiles

While defaults are provided, all thresholds and load profiles are configurable in `Configuration/StressTestConfig.cs`.

### Load Profiles
We define three standard load profiles to simulate different intensities. Note that these are configurable and numbers may vary:
*   **Light:** Short duration, low concurrency. For quick CI/CD checks.
*   **Medium:** Longer duration. For standard release validation.
*   **Heavy:** High concurrency, sustained duration. For deep regression testing.

### Thresholds
Thresholds are defined per-resource (e.g., `MemorySlopeThreshold`).
*   **Note:** WebSocket tests use higher memory thresholds due to the inherent overhead of maintaining open socket buffers.

---
