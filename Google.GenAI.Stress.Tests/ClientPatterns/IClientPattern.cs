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

namespace Google.GenAI.StressTests.ClientPatterns;

/// <summary>
/// Interface for different client usage patterns
/// </summary>
public interface IClientPattern : IDisposable
{
    /// <summary>
    /// Get a client instance (may create new or return existing)
    /// </summary>
    Client GetClient();

    /// <summary>
    /// Return/release a client instance after use
    /// </summary>
    void ReturnClient(Client client);

    /// <summary>
    /// Name of the pattern for reporting
    /// </summary>
    string PatternName { get; }

    /// <summary>
    /// Configure the pattern with optional mock server URL.
    /// When mockServerUrl is provided, clients will connect to the mock server instead of the real API.
    /// </summary>
    /// <param name="mockServerUrl">The mock server URL (e.g., "http://127.0.0.1:12345") or null for real API</param>
    void Configure(string? mockServerUrl);
}
