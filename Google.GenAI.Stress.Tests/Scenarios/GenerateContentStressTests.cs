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
using Google.GenAI.Types;

namespace Google.GenAI.StressTests.Scenarios;

/// <summary>
/// Stress tests for GenerateContent API (non-streaming)
/// This is the CRITICAL test that replicates the Python SDK memory leak scenario
/// </summary>
[TestClass]
public class GenerateContentStressTests : StressTestBase
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
    /// Expected: PASS with no leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Light")]
    [TestCategory("GenerateContent")]
    [TestCategory("Singleton")]
    public async Task GenerateContent_SingletonClient_Light()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up the client to initialize connections and handles.
        // This ensures the baseline snapshot includes the resources held by the singleton.
        // Without this, the initial connection pool creation is flagged as a leak.
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContent;
        await client.Models.GenerateContentAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt);

        // Reset baseline after warm-up
        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentScenario(
            clientPattern,
            "GenerateContent_Singleton_Light");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Light);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Light");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern B (Per-Request) with Light load
    /// Expected: PASS - proper disposal should prevent leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Light")]
    [TestCategory("GenerateContent")]
    [TestCategory("PerRequest")]
    public async Task GenerateContent_ClientPerRequest_Light()
    {
        using var clientPattern = CreateClientPattern<ClientPerRequestPattern>();

        // Warm up static state
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContent;
        await client.Models.GenerateContentAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt);
        clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentScenario(
            clientPattern,
            "GenerateContent_PerRequest_Light");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Light);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Light");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern B (Per-Request) with Medium load
    /// Expected: PASS - proper disposal should prevent leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Medium")]
    [TestCategory("GenerateContent")]
    [TestCategory("PerRequest")]
    public async Task GenerateContent_ClientPerRequest_Medium()
    {
        using var clientPattern = CreateClientPattern<ClientPerRequestPattern>();

        // Warm up static state
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContent;
        await client.Models.GenerateContentAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt);
        clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentScenario(
            clientPattern,
            "GenerateContent_PerRequest_Medium");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Medium);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Medium");

        Console.WriteLine($"\nClients created during test: {clientPattern.ClientsCreated}");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern C (Pool) with Medium load
    /// Expected: PASS - demonstrates viable alternative to singleton
    /// </summary>
    [TestMethod]
    [TestCategory("Medium")]
    [TestCategory("GenerateContent")]
    [TestCategory("Pool")]
    public async Task GenerateContent_ClientPool_Medium()
    {
        using var clientPattern = CreateClientPoolPattern(poolSize: 10);

        // Warm up the pool: create and use all clients to initialize connections
        var scenarioConfig = Config.Scenarios.GenerateContent;
        var clients = new List<Client>();
        // 1. Drain/Fill the pool
        for (int i = 0; i < 10; i++)
        {
            clients.Add(clientPattern.GetClient());
        }

        // 2. Warm up each client (initialize connections)
        foreach (var client in clients)
        {
            await client.Models.GenerateContentAsync(
                 model: scenarioConfig.Model,
                 contents: scenarioConfig.Prompt);
        }

        // 3. Return to pool
        foreach (var client in clients)
        {
            clientPattern.ReturnClient(client);
        }

        // Reset baseline after full pool warm-up
        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentScenario(
            clientPattern,
            "GenerateContent_Pool_Medium");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Medium);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Medium");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern A (Singleton) with Heavy load
    /// Expected: PASS - singleton should handle heavy load without leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("GenerateContent")]
    [TestCategory("Singleton")]
    public async Task GenerateContent_SingletonClient_Heavy()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up the client
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContent;
        await client.Models.GenerateContentAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt);
        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentScenario(
            clientPattern,
            "GenerateContent_Singleton_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Heavy");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern B (Per-Request) with Heavy load
    /// Expected: PASS - proper disposal should prevent leaks even under extreme load
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("GenerateContent")]
    [TestCategory("PerRequest")]
    public async Task GenerateContent_ClientPerRequest_Heavy()
    {
        using var clientPattern = CreateClientPattern<ClientPerRequestPattern>();

        // Warm up static state
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContent;
        await client.Models.GenerateContentAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt);
        clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentScenario(
            clientPattern,
            "GenerateContent_PerRequest_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Heavy");

        Console.WriteLine($"\nClients created during test: {clientPattern.ClientsCreated}");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern C (Pool) with Heavy load
    /// Expected: PASS - pool should handle heavy load without leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("GenerateContent")]
    [TestCategory("Pool")]
    public async Task GenerateContent_ClientPool_Heavy()
    {
        using var clientPattern = CreateClientPoolPattern(poolSize: 10);

        // Warm up the pool: create and use all clients to initialize connections
        var scenarioConfig = Config.Scenarios.GenerateContent;
        var clients = new List<Client>();
        // 1. Drain/Fill the pool
        for (int i = 0; i < 10; i++)
        {
            clients.Add(clientPattern.GetClient());
        }

        // 2. Warm up each client (initialize connections)
        foreach (var client in clients)
        {
            await client.Models.GenerateContentAsync(
                 model: scenarioConfig.Model,
                 contents: scenarioConfig.Prompt);
        }

        // 3. Return to pool
        foreach (var client in clients)
        {
            clientPattern.ReturnClient(client);
        }

        // Reset baseline after full pool warm-up
        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentScenario(
            clientPattern,
            "GenerateContent_Pool_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Heavy");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Creates a GenerateContent scenario with the specified client pattern
    /// </summary>
    private ScenarioProps CreateGenerateContentScenario(
        IClientPattern clientPattern,
        string scenarioName)
    {
        var scenarioConfig = Config.Scenarios.GenerateContent;

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

                // Make API call with retry logic for rate limits
                var response = await ExecuteWithRetry(async () =>
                {
                    return await client.Models.GenerateContentAsync(
                        model: scenarioConfig.Model,
                        contents: scenarioConfig.Prompt);
                });

                stopwatch.Stop();

                // Validate response
                if (response?.Candidates == null || response.Candidates.Count == 0)
                {
                    return Response.Fail<int>(payload: 0, statusCode: "500");
                }

                // Capture snapshot after SDK operation completes but before returning to NBomber
                ResourceMonitor?.CaptureInScenarioSnapshot();

                var textLength = response.Candidates[0].Content?.Parts?[0].Text?.Length ?? 0;
                return Response.Ok(
                    payload: textLength,
                    sizeBytes: textLength);
            }
            catch (Exception ex)
            {
                // Log first few exceptions to diagnose issues
                if (context.InvocationNumber < 5)
                {
                    Console.WriteLine($"[Scenario Error #{context.InvocationNumber}] {ex.GetType().Name}: {ex.Message}");
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
    /// Execute with exponential backoff retry for rate limit errors (429)
    /// </summary>
    private async Task<T> ExecuteWithRetry<T>(Func<Task<T>> action, int maxRetries = 1)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                return await action();
            }
            catch (Exception ex) when (ex.Message.Contains("429") || ex.Message.Contains("quota"))
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
