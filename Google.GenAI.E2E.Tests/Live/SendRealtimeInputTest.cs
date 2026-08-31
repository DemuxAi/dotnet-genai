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
using System.IO;
using System.Threading.Tasks;
using Google.GenAI;
using GoogleType = Google.GenAI.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestServerSdk;

[TestClass]
public class SendRealtimeInputTest {
  private Client vertexClient;
  private Client geminiClient;
  private string vertexModelName;
  private string geminiModelName;
  public TestContext TestContext { get; set; }

  [TestInitialize]
  public void TestInit() {
    // Test server specific setup
    var geminiClientHttpOptions = new GoogleType.HttpOptions {
      Headers = new Dictionary<string, string> { { "Test-Name",
                                                   $"{GetType().Name}.{TestContext.TestName}" } },
      BaseUrl = "http://localhost:1453"
    };
    var vertexClientHttpOptions = new GoogleType.HttpOptions {
      Headers = new Dictionary<string, string> { { "Test-Name",
                                                   $"{GetType().Name}.{TestContext.TestName}" } },
      BaseUrl = "http://localhost:1454"
    };

    // Common setup for both clients
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
    vertexModelName = "gemini-2.0-flash-live-preview-04-09";
    geminiModelName = "gemini-live-2.5-flash-preview";
  }

  [TestMethod]
  [Timeout(60000)]
  public async Task SendRealtimeInputTextGeminiTest() {
    var config = new GoogleType.LiveConnectConfig {
      ResponseModalities = new List<GoogleType.Modality> { GoogleType.Modality.Text }
    };
    var geminiSession = new SessionWithQueue(geminiClient, geminiModelName, config);
    await geminiSession.InitializeSessionAsync();

    await geminiSession.SendRealtimeInputAsync(
        new GoogleType.LiveSendRealtimeInputParameters { Text = "Are you there Gemini?" });

    var setupMessage = await geminiSession.ReceiveAsync();
    Assert.IsNotNull(setupMessage.SetupComplete);

    var message = await geminiSession.ReceiveAsync();
    Assert.IsNotNull(message.ServerContent);
    Assert.IsNotNull(message.ServerContent.ModelTurn);

    await geminiSession.CloseAsync();
  }

  [TestMethod]
  [Timeout(60000)]
  public async Task SendRealtimeInputTextVertexTest() {
    var config = new GoogleType.LiveConnectConfig {
      ResponseModalities = new List<GoogleType.Modality> { GoogleType.Modality.Text }
    };
    var vertexSession = new SessionWithQueue(vertexClient, vertexModelName, config);
    await vertexSession.InitializeSessionAsync();

    await vertexSession.SendRealtimeInputAsync(
        new GoogleType.LiveSendRealtimeInputParameters { Text = "Are you there Gemini?" });

    var setupMessage = await vertexSession.ReceiveAsync();
    Assert.IsNotNull(setupMessage.SetupComplete);

    var message = await vertexSession.ReceiveAsync();
    Assert.IsNotNull(message.ServerContent);
    Assert.IsNotNull(message.ServerContent.ModelTurn);

    await vertexSession.CloseAsync();
  }

  [TestMethod]
  [Timeout(60000)]
  public async Task SendRealtimeInputAudioGeminiTest() {
    var audioBytes = await File.ReadAllBytesAsync("TestAssets/hello_are_you_there.pcm");

    var config = new GoogleType.LiveConnectConfig {
      ResponseModalities = new List<GoogleType.Modality> { GoogleType.Modality.Text }
    };
    var geminiSession = new SessionWithQueue(geminiClient, geminiModelName, config);
    await geminiSession.InitializeSessionAsync();

    await geminiSession.SendRealtimeInputAsync(new GoogleType.LiveSendRealtimeInputParameters {
      Audio = new GoogleType.Blob { Data = audioBytes, MimeType = "audio/pcm;rate=16000" }
    });

    var setupMessage = await geminiSession.ReceiveAsync();
    Assert.IsNotNull(setupMessage.SetupComplete);

    var message = await geminiSession.ReceiveAsync();
    Assert.IsNotNull(message.ServerContent);
    Assert.IsNotNull(message.ServerContent.ModelTurn);

    await geminiSession.CloseAsync();
  }

  [TestMethod]
  [Timeout(60000)]
  public async Task SendRealtimeInputAudioVertexTest() {
    var audioBytes = await File.ReadAllBytesAsync("TestAssets/hello_are_you_there.pcm");

    var config = new GoogleType.LiveConnectConfig {
      ResponseModalities = new List<GoogleType.Modality> { GoogleType.Modality.Text }
    };
    var vertexSession = new SessionWithQueue(vertexClient, vertexModelName, config);
    await vertexSession.InitializeSessionAsync();

    await vertexSession.SendRealtimeInputAsync(new GoogleType.LiveSendRealtimeInputParameters {
      Audio = new GoogleType.Blob { Data = audioBytes, MimeType = "audio/pcm;rate=16000" }
    });

    var setupMessage = await vertexSession.ReceiveAsync();
    Assert.IsNotNull(setupMessage.SetupComplete);

    var message = await vertexSession.ReceiveAsync();
    Assert.IsNotNull(message.ServerContent);
    Assert.IsNotNull(message.ServerContent.ModelTurn);

    await vertexSession.CloseAsync();
  }
}
