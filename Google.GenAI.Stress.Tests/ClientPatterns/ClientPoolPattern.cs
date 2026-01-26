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
/// Pattern C: Client Pool
/// Maintains a pool of reusable client instances
/// Expected: Should pass, demonstrates viable alternative to singleton
/// </summary>
public class ClientPoolPattern : IClientPattern
{
    private readonly ConcurrentBag<Client> _pool;
    private readonly int _maxPoolSize;
    private int _currentSize = 0;
    private string? _mockServerUrl;

    public ClientPoolPattern(int maxPoolSize = 10)
    {
        _pool = new ConcurrentBag<Client>();
        _maxPoolSize = maxPoolSize;
    }

    public string PatternName => $"Pool({_maxPoolSize})";

    public void Configure(string? mockServerUrl)
    {
        _mockServerUrl = mockServerUrl;
    }

    public Client GetClient()
    {
        // Try to get from pool first
        if (_pool.TryTake(out var client))
        {
            return client;
        }

        // If pool is empty and we haven't reached max size, try to create new
        // Use atomic compare-exchange to avoid race condition
        while (true)
        {
            var current = _currentSize;
            if (current >= _maxPoolSize)
                break;
            if (Interlocked.CompareExchange(ref _currentSize, current + 1, current) == current)
                return CreateClient();
        }

        // Pool is at max size, wait and retry
        // In production, you might want a timeout here
        Thread.SpinWait(100);

        if (_pool.TryTake(out client))
        {
            return client;
        }

        // Fallback: create new client (pool is at max but all in use)
        return CreateClient();
    }

    public void ReturnClient(Client client)
    {
        // Return to pool for reuse (don't dispose)
        _pool.Add(client);
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

    public void Dispose()
    {
        // Dispose all pooled clients
        while (_pool.TryTake(out var client))
        {
            try
            {
                client.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
            catch
            {
                // Ignore disposal errors during cleanup
            }
        }
    }
}
