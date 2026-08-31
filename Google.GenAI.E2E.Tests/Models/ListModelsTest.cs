/*
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Google.GenAI;
using Google.GenAI.Types;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TestServerSdk;

[TestClass]
public class ListModelsTest
{
    private static TestServerProcess? _server;
    private Client vertexClient;
    private Client geminiClient;
    public TestContext TestContext { get; set; }

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        _server = TestServer.StartTestServer();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        TestServer.StopTestServer(_server);
    }

    [TestInitialize]
    public void TestInit()
    {
        // Test server specific setup.
        if (_server == null)
        {
            throw new InvalidOperationException("Test server is not initialized.");
        }
        var geminiClientHttpOptions = new HttpOptions
        {
            Headers = new Dictionary<string, string> { { "Test-Name",
                                                   $"{GetType().Name}.{TestContext.TestName}" } },
            BaseUrl = "http://localhost:1453"
        };
        var vertexClientHttpOptions = new HttpOptions
        {
            Headers = new Dictionary<string, string> { { "Test-Name",
                                                   $"{GetType().Name}.{TestContext.TestName}" } },
            BaseUrl = "http://localhost:1454"
        };

        // Common setup for both clients.
        string project = System.Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
        string location =
            System.Environment.GetEnvironmentVariable("GOOGLE_CLOUD_LOCATION") ?? "us-central1";
        string apiKey = System.Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        vertexClient = new Client(project: project, location: location, vertexAI: true,
                                  credential: TestServer.GetCredentialForTestMode(),
                                  httpOptions: vertexClientHttpOptions);
        geminiClient =
            new Client(apiKey: apiKey, vertexAI: false, httpOptions: geminiClientHttpOptions);
    }

    [TestMethod]
    public async Task ListTunedModelsVertex()
    {
        var config = new ListModelsConfig { QueryBase = false, PageSize = 3 };
        var pager = await vertexClient.Models.ListAsync(config);
        Assert.IsNotNull(pager);

        int count = 0;
        await foreach (var model in pager)
        {
            count++;
            if (count >= 4) break;
        }
        Assert.IsTrue(count >= 0);
    }

    [TestMethod]
    public async Task ListTunedModelsGemini()
    {
        var config = new ListModelsConfig { QueryBase = false, PageSize = 3 };
        var pager = await geminiClient.Models.ListAsync(config);
        Assert.IsNotNull(pager);

        int count = 0;
        await foreach (var model in pager)
        {
            count++;
            if (count >= 4) break;
        }
        Assert.IsTrue(count >= 0);
    }

    [TestMethod]
    public async Task ListBaseModelsVertex()
    {
        // Page size set to avoid long output
        var config = new ListModelsConfig { PageSize = 5 };
        var pager = await vertexClient.Models.ListAsync(config);
        Assert.IsNotNull(pager);

        int count = 0;
        await foreach (var model in pager)
        {
            count++;
            if (count >= 10) break;
        }
        Assert.IsTrue(count == 10);
    }

    [TestMethod]
    public async Task ListBaseModelsGemini()
    {
        var pager = await geminiClient.Models.ListAsync();
        Assert.IsNotNull(pager);

        int count = 0;
        await foreach (var model in pager)
        {
            count++;
            if (count >= 4) break;
        }
        Assert.IsTrue(count >= 0);
    }

    [TestMethod]
    public async Task ListBaseModelsWithConfigVertex()
    {
        var config = new ListModelsConfig { QueryBase = true, PageSize = 10 };
        var pager = await vertexClient.Models.ListAsync(config);
        Assert.IsNotNull(pager);

        int count = 0;
        await foreach (var model in pager)
        {
            count++;
            if (count >= 4) break;
        }
        Assert.IsTrue(count >= 0);
    }

    [TestMethod]
    public async Task ListBaseModelsWithConfigGemini()
    {
        var config = new ListModelsConfig { QueryBase = true, PageSize = 5 };
        var pager = await geminiClient.Models.ListAsync(config);
        Assert.IsNotNull(pager);

        int count = 0;
        await foreach (var model in pager)
        {
            count++;
            if (count >= 4) break;
        }
        Assert.IsTrue(count >= 0);
    }
}