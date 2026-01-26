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

using Google.GenAI.StressTests.Configuration;
using Google.GenAI.StressTests.MockServer;
using Google.GenAI.Types;

namespace Google.GenAI.StressTests.ClientPatterns;

/// <summary>
/// Pattern A: Singleton Client (Recommended)
/// Single shared client instance for all requests
/// Expected: No leaks, optimal performance
/// </summary>
public class SingletonClientPattern : IClientPattern
{
    private Client? _client;
    private string? _mockServerUrl;
    private readonly object _lock = new();

    public string PatternName => "Singleton";

    public void Configure(string? mockServerUrl)
    {
        _mockServerUrl = mockServerUrl;
    }

    public Client GetClient()
    {
        if (_client == null)
        {
            lock (_lock)
            {
                if (_client == null)
                {
                    _client = CreateClient();
                }
            }
        }
        return _client;
    }

    private Client CreateClient()
    {
        var config = StressTestConfig.Instance;
        var httpOptions = _mockServerUrl != null
            ? new HttpOptions { BaseUrl = _mockServerUrl }
            : null;

        if (config.VertexAI)
        {
            return new Client(
                project: config.Project,
                location: config.Location,
                vertexAI: true,
                credential: _mockServerUrl != null ? new MockCredential() : null,
                httpOptions: httpOptions);
        }
        else
        {
            return new Client(
                apiKey: _mockServerUrl != null ? "mock-api-key" : config.ApiKey,
                httpOptions: httpOptions);
        }
    }

    public void ReturnClient(Client client)
    {
        // No-op: shared singleton, don't dispose
    }

    public void Dispose()
    {
        _client?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        _client = null;
    }
}
