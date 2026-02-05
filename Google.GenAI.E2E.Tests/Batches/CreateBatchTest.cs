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
public class CreateBatchTest
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
        string apiKey = System.Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
        vertexClient = new Client(project: project, location: location, vertexAI: true,
                                  credential: TestServer.GetCredentialForTestMode(),
                                  httpOptions: vertexClientHttpOptions);
        geminiClient =
            new Client(apiKey: apiKey, vertexAI: false, httpOptions: geminiClientHttpOptions);

        // Specific setup for this test class
        modelName = "gemini-2.5-flash";
    }

    [TestMethod]
    public async Task CreateBatchWithInlinedRequestsGeminiTest()
    {
        var safetySettings = new List<SafetySetting>
        {
            new SafetySetting { Category = HarmCategory.HarmCategoryHateSpeech, Threshold = HarmBlockThreshold.BlockOnlyHigh },
            new SafetySetting { Category = HarmCategory.HarmCategoryDangerousContent, Threshold = HarmBlockThreshold.BlockLowAndAbove },
        };
        var inlineRequest = new InlinedRequest
        {
            Contents = new List<Content>
            {
                new Content
                {
                    Parts = new List<Part> { new Part { Text = "Hello!" } },
                    Role = "user"
                }
            },
            Metadata = new Dictionary<string, string> { { "key", "request-1" } },
            Config = new GenerateContentConfig
            {
                SafetySettings = safetySettings
            }
        };
        var src = new BatchJobSource
        {
            InlinedRequests = new List<InlinedRequest> { inlineRequest }
        };
        var config = new CreateBatchJobConfig
        {
            DisplayName = "test_batch_inlined"
        };

        var response = await geminiClient.Batches.CreateAsync(modelName, src, config);
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Name.StartsWith("batches/"));
    }

    [TestMethod]
    public async Task CreateBatchWithInlinedRequestsVertexTest()
    {
        var src = new BatchJobSource
        {
            InlinedRequests = new List<InlinedRequest> { new InlinedRequest { Contents = new List<Content> { new Content { Parts = new List<Part> { new Part { Text = "Hello!" } } } } } }
        };
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
        {
            await vertexClient.Batches.CreateAsync(modelName, src, null);
        });
    }

    [TestMethod]
    public async Task CreateBatchWithGcsVertexTest()
    {
        var src = new BatchJobSource
        {
            GcsUri = new List<string> { "gs://unified-genai-tests/batches/input/generate_content_requests.jsonl" },
            Format = "jsonl"
        };
        var config = new CreateBatchJobConfig
        {
            DisplayName = "test_batch_gcs",
            Dest = new BatchJobDestination
            {
                GcsUri = "gs://unified-genai-tests/batches/output",
                Format = "jsonl"
            }
        };
        var response = await vertexClient.Batches.CreateAsync(modelName, src, config);
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Name.StartsWith("projects/"));
    }

    [TestMethod]
    public async Task CreateBatchWithGcsGeminiTest()
    {
        var src = new BatchJobSource
        {
            GcsUri = new List<string> { "gs://unified-genai-tests/batches/input/generate_content_requests.jsonl" },
            Format = "jsonl"
        };
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
        {
            await geminiClient.Batches.CreateAsync(modelName, src, null);
        });
    }

    [TestMethod]
    public async Task CreateBatchWithBigQueryVertexTest()
    {
        if (TestServer.IsReplayMode)
        {
            Assert.Inconclusive("Vertex BigQuery batch tests run in record mode only.");
        }
        var src = new BatchJobSource
        {
            BigqueryUri = "bq://storage-samples.generative_ai.batch_requests_for_multimodal_input",
            Format = "bigquery"
        };
        var config = new CreateBatchJobConfig
        {
            DisplayName = "test_batch_bigquery",
            Dest = new BatchJobDestination
            {
                BigqueryUri = "bq://REDACTED.unified_genai_tests_batches.generate_content_output",
                Format = "bigquery"
            }
        };
        var response = await vertexClient.Batches.CreateAsync(modelName, src, config);
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Name.StartsWith("projects/"));
    }

    [TestMethod]
    public async Task CreateBatchWithBigQueryGeminiTest()
    {
        var src = new BatchJobSource
        {
            BigqueryUri = "bq://storage-samples.generative_ai.batch_requests_for_multimodal_input",
            Format = "bigquery"
        };
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
        {
            await geminiClient.Batches.CreateAsync(modelName, src, null);
        });
    }

    [TestMethod]
    public async Task CreateBatchWithFileVertexTest()
    {
        var src = new BatchJobSource
        {
            FileName = "files/s0pa54alni6w"
        };
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
        {
            await vertexClient.Batches.CreateAsync(modelName, src, null);
        });
    }

    [TestMethod]
    public async Task CreateBatchWithFileGeminiTest()
    {
        var src = new BatchJobSource
        {
            FileName = "files/s0pa54alni6w"
        };
        var config = new CreateBatchJobConfig
        {
            DisplayName = "test_batch_file"
        };
        var response = await geminiClient.Batches.CreateAsync(modelName, src, config);
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Name.StartsWith("batches/"));
    }
}
