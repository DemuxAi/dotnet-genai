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

using System.Collections.Concurrent;
using Google.GenAI.StressTests.Configuration;
using Google.GenAI.StressTests.MockServer;
using Google.GenAI.Types;

namespace Google.GenAI.StressTests.ClientPatterns;

/// <summary>
/// Pattern B: Client Per Request (Python SDK Issue Pattern)
/// Creates a new client for each request and disposes it immediately after
/// Expected: May reveal leaks if HttpClient disposal is problematic
/// This is the pattern that caused memory leaks in the Python SDK
/// </summary>
public class ClientPerRequestPattern : IClientPattern
{
    // Track only active clients for cleanup.
    // Key is the client instance, Value is a dummy byte (using as a Set)
    private readonly ConcurrentDictionary<Client, byte> _activeClients = new();
    private int _clientCount = 0;
    private string? _mockServerUrl;

    public string PatternName => "PerRequest";

    public void Configure(string? mockServerUrl)
    {
        _mockServerUrl = mockServerUrl;
    }

    public Client GetClient()
    {
        var config = StressTestConfig.Instance;
        var httpOptions = _mockServerUrl != null
            ? new HttpOptions { BaseUrl = _mockServerUrl }
            : null;

        Client client;

        if (config.VertexAI)
        {
            client = new Client(
                project: config.Project,
                location: config.Location,
                vertexAI: true,
                credential: _mockServerUrl != null ? new MockCredential() : null,
                httpOptions: httpOptions);
        }
        else
        {
            client = new Client(
                apiKey: _mockServerUrl != null ? "mock-api-key" : config.ApiKey,
                httpOptions: httpOptions);
        }

        _activeClients.TryAdd(client, 0);
        Interlocked.Increment(ref _clientCount);

        return client;
    }

    public void ReturnClient(Client client)
    {
        // Remove from tracking first
        _activeClients.TryRemove(client, out _);

        // Dispose immediately after use (per-request pattern)
        // Use DisposeAsync and block to ensure full async cleanup.
        try
        {
            client.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disposing client: {ex.Message}");
        }
    }

    public int ClientsCreated => _clientCount;

    public void Dispose()
    {
        // Dispose any active clients that weren't returned
        foreach (var client in _activeClients.Keys)
        {
            try
            {
                client.Dispose();
            }
            catch
            {
                // Ignore disposal errors during cleanup
            }
        }
        _activeClients.Clear();
    }
}
