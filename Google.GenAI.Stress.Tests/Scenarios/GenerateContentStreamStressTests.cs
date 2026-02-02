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
/// Stress tests for GenerateContentStream API (streaming responses)
/// Tests async enumerable handling and stream disposal under load
/// </summary>
[TestClass]
public class GenerateContentStreamStressTests : StressTestBase
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
    /// Expected: PASS with no leaks, streams properly disposed
    /// </summary>
    [TestMethod]
    [TestCategory("Light")]
    [TestCategory("GenerateContentStream")]
    [TestCategory("Singleton")]
    public async Task GenerateContentStream_SingletonClient_Light()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up the client
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContentStream;
        await foreach (var _ in client.Models.GenerateContentStreamAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt))
        {
            // Consume stream to ensure connection established
        }

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentStreamScenario(
            clientPattern,
            "GenerateContentStream_Singleton_Light");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Light);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Light");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern B (Per-Request) with Medium load
    /// Expected: May reveal leaks if streams aren't properly disposed
    /// </summary>
    [TestMethod]
    [TestCategory("Medium")]
    [TestCategory("GenerateContentStream")]
    [TestCategory("PerRequest")]
    public async Task GenerateContentStream_ClientPerRequest_Medium()
    {
        using var clientPattern = CreateClientPattern<ClientPerRequestPattern>();

        // Warm up static state
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContentStream;
        await foreach (var _ in client.Models.GenerateContentStreamAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt))
        { }
        clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentStreamScenario(
            clientPattern,
            "GenerateContentStream_PerRequest_Medium");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Medium);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Medium");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern A (Singleton) with Medium load
    /// Expected: PASS - singleton should handle streaming without leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Medium")]
    [TestCategory("GenerateContentStream")]
    [TestCategory("Singleton")]
    public async Task GenerateContentStream_SingletonClient_Medium()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up the client
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContentStream;
        await foreach (var _ in client.Models.GenerateContentStreamAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt))
        {
            // Consume stream
        }

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentStreamScenario(
            clientPattern,
            "GenerateContentStream_Singleton_Medium");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Medium);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Medium");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern C (Pool) with Medium load
    /// Expected: PASS - demonstrates pooling works with streaming
    /// </summary>
    [TestMethod]
    [TestCategory("Medium")]
    [TestCategory("GenerateContentStream")]
    [TestCategory("Pool")]
    public async Task GenerateContentStream_ClientPool_Medium()
    {
        using var clientPattern = CreateClientPoolPattern(poolSize: 10);

        // Warm up the pool
        var scenarioConfig = Config.Scenarios.GenerateContentStream;
        var clients = new List<Client>();
        // 1. Fill pool
        for (int i = 0; i < 10; i++) clients.Add(clientPattern.GetClient());

        // 2. Warm up
        foreach (var client in clients)
        {
            await foreach (var _ in client.Models.GenerateContentStreamAsync(
                 model: scenarioConfig.Model,
                 contents: scenarioConfig.Prompt))
            { }
        }

        // 3. Return
        foreach (var client in clients) clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentStreamScenario(
            clientPattern,
            "GenerateContentStream_Pool_Medium");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Medium);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Medium");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern A (Singleton) with Heavy load
    /// Expected: PASS - singleton should handle heavy streaming load without leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("GenerateContentStream")]
    [TestCategory("Singleton")]
    public async Task GenerateContentStream_SingletonClient_Heavy()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up the client
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContentStream;
        await foreach (var _ in client.Models.GenerateContentStreamAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt))
        {
            // Consume stream
        }

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentStreamScenario(
            clientPattern,
            "GenerateContentStream_Singleton_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Heavy");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern B (Per-Request) with Heavy load
    /// Expected: PASS - proper disposal should prevent leaks even under extreme streaming load
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("GenerateContentStream")]
    [TestCategory("PerRequest")]
    public async Task GenerateContentStream_ClientPerRequest_Heavy()
    {
        using var clientPattern = CreateClientPattern<ClientPerRequestPattern>();

        // Warm up static state
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContentStream;
        await foreach (var _ in client.Models.GenerateContentStreamAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt))
        { }
        clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentStreamScenario(
            clientPattern,
            "GenerateContentStream_PerRequest_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Heavy");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test Pattern C (Pool) with Heavy load
    /// Expected: PASS - pool should handle heavy streaming load without leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Heavy")]
    [TestCategory("GenerateContentStream")]
    [TestCategory("Pool")]
    public async Task GenerateContentStream_ClientPool_Heavy()
    {
        using var clientPattern = CreateClientPoolPattern(poolSize: 10);

        // Warm up the pool
        var scenarioConfig = Config.Scenarios.GenerateContentStream;
        var clients = new List<Client>();
        // 1. Fill pool
        for (int i = 0; i < 10; i++) clients.Add(clientPattern.GetClient());

        // 2. Warm up
        foreach (var client in clients)
        {
            await foreach (var _ in client.Models.GenerateContentStreamAsync(
                 model: scenarioConfig.Model,
                 contents: scenarioConfig.Prompt))
            { }
        }

        // 3. Return
        foreach (var client in clients) clientPattern.ReturnClient(client);

        ResourceMonitor?.ResetBaseline();

        var scenario = CreateGenerateContentStreamScenario(
            clientPattern,
            "GenerateContentStream_Pool_Heavy");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Heavy);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Heavy");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Test incomplete consumption of stream (early break)
    /// This tests if unconsumed streams cause leaks
    /// </summary>
    [TestMethod]
    [TestCategory("Light")]
    [TestCategory("GenerateContentStream")]
    [TestCategory("Singleton")]
    public async Task GenerateContentStream_PartialConsumption_Light()
    {
        using var clientPattern = CreateClientPattern<SingletonClientPattern>();

        // Warm up
        var client = clientPattern.GetClient();
        var scenarioConfig = Config.Scenarios.GenerateContentStream;
        await foreach (var _ in client.Models.GenerateContentStreamAsync(
             model: scenarioConfig.Model,
             contents: scenarioConfig.Prompt))
        { }
        ResourceMonitor?.ResetBaseline();

        var scenario = CreatePartialConsumptionScenario(
            clientPattern,
            "GenerateContentStream_PartialConsumption_Light");

        scenario = scenario.WithLoadSimulations(LoadPatterns.Light);

        // Use default slope-based thresholds
        var metrics = await RunScenario(scenario, clientPattern, "Light");

        AssertNoResourceLeaks(metrics);
    }

    /// <summary>
    /// Creates a GenerateContentStream scenario that fully consumes the stream
    /// </summary>
    private ScenarioProps CreateGenerateContentStreamScenario(
        IClientPattern clientPattern,
        string scenarioName)
    {
        var scenarioConfig = Config.Scenarios.GenerateContentStream;

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

                int chunkCount = 0;
                int totalBytes = 0;

                // Make streaming API call with retry logic
                await foreach (var chunk in ExecuteStreamWithRetry(() =>
                    client.Models.GenerateContentStreamAsync(
                        model: scenarioConfig.Model,
                        contents: scenarioConfig.Prompt)))
                {
                    chunkCount++;
                    var text = chunk?.Candidates?[0]?.Content?.Parts?[0]?.Text;
                    if (text != null)
                    {
                        totalBytes += text.Length;
                    }
                }

                stopwatch.Stop();

                // Validate we received chunks
                if (chunkCount == 0)
                {
                    return Response.Fail<int>(payload: 0, statusCode: "500");
                }

                // Capture snapshot after SDK operation completes but before returning to NBomber
                ResourceMonitor?.CaptureInScenarioSnapshot();

                return Response.Ok(
                    payload: chunkCount,
                    sizeBytes: totalBytes);
            }
            catch (Exception)
            {
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
    /// Creates a scenario that only partially consumes the stream (tests early break)
    /// </summary>
    private ScenarioProps CreatePartialConsumptionScenario(
        IClientPattern clientPattern,
        string scenarioName)
    {
        var scenarioConfig = Config.Scenarios.GenerateContentStream;

        var scenario = Scenario.Create(scenarioName, async context =>
        {
            Client? client = null;

            try
            {
                var stopwatch = Stopwatch.StartNew();

                client = clientPattern.GetClient();
                ResourceMonitor?.IncrementRequestCount();

                int chunkCount = 0;
                int totalBytes = 0;
                const int maxChunks = 3; // Only consume first 3 chunks

                await foreach (var chunk in ExecuteStreamWithRetry(() =>
                    client.Models.GenerateContentStreamAsync(
                        model: scenarioConfig.Model,
                        contents: scenarioConfig.Prompt)))
                {
                    chunkCount++;
                    var text = chunk?.Candidates?[0]?.Content?.Parts?[0]?.Text;
                    if (text != null)
                    {
                        totalBytes += text.Length;
                    }

                    // Break early - don't consume entire stream
                    if (chunkCount >= maxChunks)
                    {
                        break;
                    }
                }

                stopwatch.Stop();

                // Capture snapshot after SDK operation completes but before returning to NBomber
                ResourceMonitor?.CaptureInScenarioSnapshot();

                return Response.Ok(
                    payload: chunkCount,
                    sizeBytes: totalBytes);
            }
            catch (Exception)
            {
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
    /// Execute streaming call with exponential backoff retry for rate limits
    /// </summary>
    private async IAsyncEnumerable<GenerateContentResponse> ExecuteStreamWithRetry(
        Func<IAsyncEnumerable<GenerateContentResponse>> action,
        int maxRetries = 1)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            bool rateLimited = false;

            await foreach (var chunk in action())
            {
                if (chunk == null) continue;

                // Check if this chunk indicates rate limiting
                var blockReason = chunk.PromptFeedback?.BlockReason;
                if (blockReason != null)
                {
                    var reason = blockReason.ToString()!;
                    if (reason.Contains("429") || reason.ToLower().Contains("quota"))
                    {
                        rateLimited = true;
                        break;
                    }
                }

                yield return chunk;
            }

            if (!rateLimited)
            {
                yield break; // Success, exit
            }

            // Rate limited, backoff and retry
            if (attempt < maxRetries - 1)
            {
                var backoff = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                Console.WriteLine($"Rate limit hit on stream, backing off for {backoff.TotalSeconds}s...");
                await Task.Delay(backoff);
            }
        }

        throw new Exception("Max retries exceeded for streaming request");
    }
}
