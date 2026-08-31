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
public class GetCacheTest
{
    private static TestServerProcess? _server;
    private Client vertexClient;
    private Client geminiClient;
    private string modelName;
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
        var geminiClientHttpOptions = new HttpOptions {
            Headers = new Dictionary<string, string> { { "Test-Name",
                                                   $"{GetType().Name}.{TestContext.TestName}" } },
            BaseUrl = "http://localhost:1453"
        };
        var vertexClientHttpOptions = new HttpOptions {
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

        // Specific setup for this test class
        modelName = "gemini-2.5-flash";
    }

    [TestMethod]
    public async Task GetCacheVertexTest()
    {
        // TODO(b/462527852): resolve test failure (temporarily skipped) in replay mode
        if (TestServer.IsReplayMode)
        {
            Assert.Inconclusive("Vertex cache tests run in record mode only.");
        }
        var config = new CreateCachedContentConfig
        {
            Contents = new List<Content>
            {
                new Content
                {
                    Role = "user",
                    Parts = new List<Part>
                    {
                        new Part { FileData = new FileData { FileUri = "gs://cloud-samples-data/generative-ai/pdf/2312.11805v3.pdf", MimeType = "application/pdf" } },
                        new Part { FileData = new FileData { FileUri = "gs://cloud-samples-data/generative-ai/pdf/2403.05530.pdf", MimeType = "application/pdf" } }
                    }
                }
            },
            DisplayName = "test-get-cache-vertex",
            Ttl = "600s"
        };
        var created = await vertexClient.Caches.CreateAsync(modelName, config);
        Assert.IsNotNull(created);

        var retrieved = await vertexClient.Caches.GetAsync(created.Name, config: null);
        Assert.IsNotNull(retrieved);
    }

    [TestMethod]
    public async Task GetCacheGeminiTest()
    {
        var part = new Part { FileData = new FileData { MimeType = "application/pdf", FileUri = "https://generativelanguage.googleapis.com/v1beta/files/gwpix29tac28" } };
        var config = new CreateCachedContentConfig
        {
            Contents = new List<Content>
            {
                new Content
                {
                    Role = "user",
                    Parts = Enumerable.Repeat(part, 5).ToList()
                }
            },
            DisplayName = "test-get-cache-gemini",
            Ttl = "600s"
        };
        var created = await geminiClient.Caches.CreateAsync(modelName, config);
        Assert.IsNotNull(created);

        var retrieved = await geminiClient.Caches.GetAsync(created.Name, config: null);
        Assert.IsNotNull(retrieved);
    }
}
