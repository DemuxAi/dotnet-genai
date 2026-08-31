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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestServerSdk;


[TestClass]
public class ListBatchTest
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
    public async Task ListBatchJobVertexTest()
    {
        var src = new BatchJobSource
        {
            GcsUri = new List<string> { "gs://unified-genai-tests/batches/input/generate_content_requests.jsonl" },
            Format = "jsonl"
        };
        var config = new CreateBatchJobConfig
        {
            DisplayName = "test_batch_gcs_list",
            Dest = new BatchJobDestination
            {
                GcsUri = "gs://unified-genai-tests/batches/output",
                Format = "jsonl"
            }
        };
        var createBatchJobOne = await vertexClient.Batches.CreateAsync("gemini-2.5-flash", src, config);
        var createBatchJobTwo = await vertexClient.Batches.CreateAsync("gemini-2.5-flash", src, config);

        var pager = await vertexClient.Batches.ListAsync(new ListBatchJobsConfig { PageSize = 1 });

        int count = 0;
        await foreach (var item in pager)
        {
            count++;
            if (count >= 2) {
                break;
            }
        }

        Assert.IsTrue(count >= 1);
        Assert.AreEqual(1, pager.PageSize);
    }

    [TestMethod]
    public async Task ListBatchJobGeminiTest()
    {
        var pager = await geminiClient.Batches.ListAsync(new ListBatchJobsConfig { PageSize = 1 });

        int count = 0;
        await foreach (var item in pager)
        {
            count++;
            if (count >= 3) {
                break;
            }
        }

        Assert.IsTrue(count >= 1);
        Assert.AreEqual(1, pager.PageSize);
    }
}

