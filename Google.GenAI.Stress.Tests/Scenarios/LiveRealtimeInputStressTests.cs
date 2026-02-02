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
using System.Diagnostics;
using Google.GenAI.StressTests.Infrastructure;
using Google.GenAI.StressTests.ClientPatterns;
using Google.GenAI.StressTests.Configuration;
using Google.GenAI.Types;

namespace Google.GenAI.StressTests.Scenarios;

/// <summary>
/// Stress tests for Live API (WebSocket bidirectional streaming)
/// Tests the most complex scenario: concurrent WebSocket sessions with send/receive
/// CRITICAL: Tests AsyncSession disposal and WebSocket connection leak detection
/// </summary>
[TestClass]
public class LiveRealtimeInputStressTests : StressTestBase
{
    [ClassInitialize]
    public static async Task ClassInit(TestContext _)
    {
        await InitializeMockServerAsync();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        CleanupMockServer();
    }

    /// <summary>
    /// Test Pattern A (Singleton) with Light load
    /// Expected: PASS - WebSocket sessions properly closed, no connection leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Light")]
    [TestCategory("Live")]
    [TestCategory("Singleton")]
    public async Task LiveRealtimeInput_SingletonClient_Light()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up client and connection pool
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.LiveApi;
        await using (var _ = await client.Live.ConnectAsync(
             model: scenarioConfig.Model,
             config: new LiveConnectConfig { ResponseModalities = new List<Modality> { Modality.TEXT } }))
        { }

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateLiveRealtimeInputScenario(
            clientPattern,
            "LiveRealtimeInput_Singleton_Light");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Light);

        // Use WebSocket memory threshold (higher than HTTP due to WebSocket overhead)
        var metrics = await RunScenario(scenario, clientPattern, "Light",
            new RunScenarioOptions
            {
                MemoryThreshold = Config.Thresholds.WebSocketMemorySlopeThreshold
            });

        AssertNoResourceLeaksWebSocket(metrics);
    }

    /// <summary>
    /// Test Pattern B (Per-Request) with Light load
    /// Expected: May reveal WebSocket connection leaks
    /// This is CRITICAL as WebSocket resources are more expensive than HTTP
    /// </summary>
    [TestMethod]
    [TestCategory("Light")]
    [TestCategory("Live")]
    [TestCategory("PerRequest")]
    public async Task LiveRealtimeInput_ClientPerRequest_Light()
    {
        using var clientPattern = CreateClientPattern<ClientPerRequestPattern>();

        // Warm up static state
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.LiveApi;
        await using (var _ = await client.Live.ConnectAsync(
             model: scenarioConfig.Model,
             config: new LiveConnectConfig { ResponseModalities = new List<Modality> { Modality.TEXT } }))
        { }
        clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateLiveRealtimeInputScenario(
            clientPattern,
            "LiveRealtimeInput_PerRequest_Light");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Light);

        // Use WebSocket memory threshold (higher than HTTP due to WebSocket overhead)
        var metrics = await RunScenario(scenario, clientPattern, "Light",
            new RunScenarioOptions
            {
                MemoryThreshold = Config.Thresholds.WebSocketMemorySlopeThreshold
            });

        AssertNoResourceLeaksWebSocket(metrics);
    }

    /// <summary>
    /// Test Pattern A (Singleton) with Medium load
    /// Expected: PASS - many concurrent WebSocket sessions should work
    /// </summary>
    [TestMethod]
    [TestCategory("Medium")]
    [TestCategory("Live")]
    [TestCategory("Singleton")]
    public async Task LiveRealtimeInput_SingletonClient_Medium()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.LiveApi;
        await using (var _ = await client.Live.ConnectAsync(
             model: scenarioConfig.Model,
             config: new LiveConnectConfig { ResponseModalities = new List<Modality> { Modality.TEXT } }))
        { }
        ResourceMonitor?.ResetBaseline();

        var scenario = CreateLiveRealtimeInputScenario(
            clientPattern,
            "LiveRealtimeInput_Singleton_Medium");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Medium);

        // Use WebSocket memory threshold (higher than HTTP due to WebSocket overhead)
        var metrics = await RunScenario(scenario, clientPattern, "Medium",
            new RunScenarioOptions
            {
                MemoryThreshold = Config.Thresholds.WebSocketMemorySlopeThreshold
            });

        AssertNoResourceLeaksWebSocket(metrics);
    }

    /// <summary>
    /// Test Pattern C (Pool) with Medium load
    /// Expected: PASS - pooling should work with WebSocket sessions
    /// </summary>
    [TestMethod]
    [TestCategory("Medium")]
    [TestCategory("Live")]
    [TestCategory("Pool")]
    public async Task LiveRealtimeInput_ClientPool_Medium()
    {
        using var clientPattern = CreateClientPoolPattern(poolSize: 10);

        // Warm up pool
        var scenarioConfig = Config.Scenarios.LiveApi;
        var clients = new List<Client>();
        // 1. Fill pool
        for (int i = 0; i < 10; i++) clients.Add(clientPattern.GetClient());

        // 2. Warm up
        foreach (var client in clients)
        {
            await using (var _ = await client.Live.ConnectAsync(
                 model: scenarioConfig.Model,
                 config: new LiveConnectConfig { ResponseModalities = new List<Modality> { Modality.TEXT } }))
            { }
        }

        // 3. Return
        foreach (var client in clients) clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateLiveRealtimeInputScenario(
            clientPattern,
            "LiveRealtimeInput_Pool_Medium");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Medium);

        // Use WebSocket memory threshold (higher than HTTP due to WebSocket overhead)
        var metrics = await RunScenario(scenario, clientPattern, "Medium",
            new RunScenarioOptions
            {
                MemoryThreshold = Config.Thresholds.WebSocketMemorySlopeThreshold
            });

        AssertNoResourceLeaksWebSocket(metrics);
    }

    /// <summary>
    /// Test Pattern A (Singleton) with Heavy load
    /// Expected: PASS - singleton should handle heavy WebSocket load without leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("Live")]
    [TestCategory("Singleton")]
    public async Task LiveRealtimeInput_SingletonClient_Heavy()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.LiveApi;
        await using (var _ = await client.Live.ConnectAsync(
             model: scenarioConfig.Model,
             config: new LiveConnectConfig { ResponseModalities = new List<Modality> { Modality.TEXT } }))
        { }
        ResourceMonitor?.ResetBaseline();

        var scenario = CreateLiveRealtimeInputScenario(
            clientPattern,
            "LiveRealtimeInput_Singleton_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use WebSocket memory threshold (higher than HTTP due to WebSocket overhead)
        var metrics = await RunScenario(scenario, clientPattern, "Heavy",
            new RunScenarioOptions
            {
                MemoryThreshold = Config.Thresholds.WebSocketMemorySlopeThreshold
            });

        AssertNoResourceLeaksWebSocket(metrics);
    }

    /// <summary>
    /// Test Pattern B (Per-Request) with Heavy load
    /// Expected: PASS - proper disposal should prevent WebSocket leaks under extreme load
    /// CRITICAL: WebSocket resources are expensive, leaks here are severe
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("Live")]
    [TestCategory("PerRequest")]
    public async Task LiveRealtimeInput_ClientPerRequest_Heavy()
    {
        using var clientPattern = CreateClientPattern<ClientPerRequestPattern>();

        // Warm up static state
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.LiveApi;
        await using (var _ = await client.Live.ConnectAsync(
             model: scenarioConfig.Model,
             config: new LiveConnectConfig { ResponseModalities = new List<Modality> { Modality.TEXT } }))
        { }
        clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateLiveRealtimeInputScenario(
            clientPattern,
            "LiveRealtimeInput_PerRequest_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use WebSocket memory threshold (higher than HTTP due to WebSocket overhead)
        var metrics = await RunScenario(scenario, clientPattern, "Heavy",
            new RunScenarioOptions
            {
                MemoryThreshold = Config.Thresholds.WebSocketMemorySlopeThreshold
            });

        AssertNoResourceLeaksWebSocket(metrics);
    }

    /// <summary>
    /// Test Pattern C (Pool) with Heavy load
    /// Expected: PASS - pool should handle heavy WebSocket load without leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("Live")]
    [TestCategory("Pool")]
    public async Task LiveRealtimeInput_ClientPool_Heavy()
    {
        using var clientPattern = CreateClientPoolPattern(poolSize: 10);

        // Warm up pool
        var scenarioConfig = Config.Scenarios.LiveApi;
        var clients = new List<Client>();
        // 1. Fill pool
        for (int i = 0; i < 10; i++) clients.Add(clientPattern.GetClient());

        // 2. Warm up
        foreach (var client in clients)
        {
            await using (var _ = await client.Live.ConnectAsync(
                 model: scenarioConfig.Model,
                 config: new LiveConnectConfig { ResponseModalities = new List<Modality> { Modality.TEXT } }))
            { }
        }

        // 3. Return
        foreach (var client in clients) clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateLiveRealtimeInputScenario(
            clientPattern,
            "LiveRealtimeInput_Pool_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use WebSocket memory threshold (higher than HTTP due to WebSocket overhead)
        var metrics = await RunScenario(scenario, clientPattern, "Heavy",
            new RunScenarioOptions
            {
                MemoryThreshold = Config.Thresholds.WebSocketMemorySlopeThreshold
            });

        AssertNoResourceLeaksWebSocket(metrics);
    }

    /// <summary>
    /// Test rapid connect/disconnect cycles
    /// This stresses AsyncSession disposal under rapid cycling
    /// </summary>
    [TestMethod]
    [TestCategory("Light")]
    [TestCategory("Live")]
    [TestCategory("Singleton")]
    public async Task LiveRealtimeInput_RapidCycles_Light()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.LiveApi;
        await using (var _ = await client.Live.ConnectAsync(
             model: scenarioConfig.Model,
             config: new LiveConnectConfig { ResponseModalities = new List<Modality> { Modality.TEXT } }))
        { }
        ResourceMonitor?.ResetBaseline();

        var scenario = CreateRapidCycleScenario(
            clientPattern,
            "LiveRealtimeInput_RapidCycles_Light");

        // Use a faster injection rate for rapid cycling
        scenario = scenario.WithLoadSimulations(
            LoadPatterns.CreateInjectPerSecond(rate: 10, durationMinutes: 1));

        // Use WebSocket memory threshold (higher than HTTP due to WebSocket overhead)
        var metrics = await RunScenario(scenario, clientPattern, "Light",
            new RunScenarioOptions
            {
                MemoryThreshold = Config.Thresholds.WebSocketMemorySlopeThreshold
            });

        AssertNoResourceLeaksWebSocket(metrics);
    }

    /// <summary>
    /// Creates a Live API scenario with send and receive operations
    /// </summary>
    private ScenarioProps CreateLiveRealtimeInputScenario(
        IClientPattern clientPattern,
        string scenarioName)
    {
        var scenarioConfig = Config.Scenarios.LiveApi;

        var scenario = Scenario.Create(scenarioName, async context =>
        {
            Client? client = null;

            try
            {
                var stopwatch = Stopwatch.StartNew();

                // Get client from pattern
                client = clientPattern.GetClient();

                // Track request in resource monitor
                ResourceMonitor?.IncrementRequestCount();

                // Receive response
                int messagesReceived = 0;

                // Connect WebSocket with retry and manage session with `await using`.
                await using (var session = await ExecuteWithRetry(async () =>
                {
                    return await client.Live.ConnectAsync(
                        model: scenarioConfig.Model,
                        config: new LiveConnectConfig
                        {
                            ResponseModalities = new List<Modality> { Modality.TEXT }
                        });
                }))
                {
                    // Wait for setup complete message
                    var setupMsg = await session.ReceiveAsync();
                    if (setupMsg?.SetupComplete == null)
                    {
                        return Response.Fail<int>(payload: 0, statusCode: "500");
                    }

                    // Send a realtime input
                    await session.SendRealtimeInputAsync(
                        new LiveSendRealtimeInputParameters
                        {
                            Text = "Hello, this is a stress test message."
                        });

                    // Timeout must be greater than P95 assertion (20s) to avoid failing slow but valid requests
                    var timeout = TimeSpan.FromSeconds(30);
                    using var cts = new CancellationTokenSource(timeout);

                    try
                    {
                        while (true)
                        {
                            var msg = await session.ReceiveAsync(cts.Token);
                            if (msg == null)
                            {
                                break;
                            }

                            if (msg.ServerContent?.TurnComplete == true)
                            {
                                messagesReceived = 1; // Indicate success
                                break;
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        if (context.InvocationNumber < 5)
                        {
                            Console.WriteLine("Timeout waiting for response from mock server. This is now considered an error.");
                        }
                        return Response.Fail<int>(statusCode: "408", message: "Timeout waiting for turnComplete");
                    }

                    stopwatch.Stop();
                } // session.DisposeAsync() is automatically called here.

                // Capture snapshot after SDK resources released but before returning to NBomber
                ResourceMonitor?.CaptureInScenarioSnapshot();

                return Response.Ok(
                    payload: messagesReceived,
                    sizeBytes: messagesReceived * 100); // Estimate
            }
            catch (Exception e)
            {
                // Limit error logging to prevent console flooding during stress
                if (context.InvocationNumber < 5)
                {
                    Console.WriteLine($"[Scenario Error #{context.InvocationNumber}] {e.GetType().Name}: {e.Message}");
                }
                return Response.Fail<int>(payload: 0, statusCode: "500");
            }
            finally
            {
                if (client != null)
                {
                    clientPattern.ReturnClient(client);
                }
            }
        });

        return scenario;
    }

    /// <summary>
    /// Creates a scenario that rapidly connects and disconnects
    /// Tests AsyncSession disposal under pressure
    /// </summary>
    private ScenarioProps CreateRapidCycleScenario(
        IClientPattern clientPattern,
        string scenarioName)
    {
        var scenarioConfig = Config.Scenarios.LiveApi;

        var scenario = Scenario.Create(scenarioName, async context =>
        {
            Client? client = null;

            try
            {
                var stopwatch = Stopwatch.StartNew();

                client = clientPattern.GetClient();
                ResourceMonitor?.IncrementRequestCount();

                // Connect and immediately disconnect by using `await using` on a new scope.
                await using (var session = await ExecuteWithRetry(async () =>
                {
                    return await client.Live.ConnectAsync(
                        model: scenarioConfig.Model,
                        config: new LiveConnectConfig
                        {
                            ResponseModalities = new List<Modality> { Modality.TEXT }
                        });
                }))
                {
                    // Wait for setup only
                    var setupMsg = await session.ReceiveAsync();
                } // session.DisposeAsync() is automatically called here, achieving rapid cycle.

                // Capture snapshot after SDK resources released but before returning to NBomber
                ResourceMonitor?.CaptureInScenarioSnapshot();

                stopwatch.Stop();

                return Response.Ok(
                    payload: 1,
                    sizeBytes: 0);
            }
            catch (Exception e)
            {
                // Limit error logging to prevent console flooding during stress
                if (context.InvocationNumber < 5)
                {
                    Console.WriteLine($"[Scenario Error #{context.InvocationNumber}] {e.GetType().Name}: {e.Message}");
                }
                return Response.Fail<int>(payload: 0, statusCode: "500");
            }
            finally
            {
                if (client != null)
                {
                    clientPattern.ReturnClient(client);
                }
            }
        });

        return scenario;
    }

    /// <summary>
    /// Execute with exponential backoff retry for rate limit errors
    /// </summary>
    private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> action, int maxRetries = 1)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                return await action();
            }
            catch (Exception ex) when (ex.Message.Contains("429") ||
                                      ex.Message.Contains("quota") ||
                                      ex.Message.Contains("rate limit"))
            {
                if (attempt == maxRetries - 1)
                    throw;

                var backoff = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                Console.WriteLine($"Rate limit hit, backing off for {backoff.TotalSeconds}s...");
                await Task.Delay(backoff);
            }
        }

        throw new Exception("Max retries exceeded");
    }
}
