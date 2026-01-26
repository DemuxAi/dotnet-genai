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
using System.Text.Json;

namespace Google.GenAI.StressTests.MockServer;

/// <summary>
/// Thread-safe provider for mock API responses loaded from JSON files.
/// </summary>
public class MockResponseProvider
{
    private readonly ConcurrentDictionary<string, string> _responses = new();
    private readonly ConcurrentDictionary<string, string[]> _streamingResponses = new();

    /// <summary>
    /// Load recorded responses from the specified directory.
    /// </summary>
    /// <param name="directory">Directory containing response JSON files</param>
    public void LoadRecordings(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Console.WriteLine($"[MockResponseProvider] Recordings directory not found: {directory}");
            Console.WriteLine("[MockResponseProvider] Using default embedded responses");
            LoadDefaultResponses();
            return;
        }

        // Load non-streaming GenerateContent response
        var generateContentPath = Path.Combine(directory, "GenerateContent_Response.json");
        if (File.Exists(generateContentPath))
        {
            var content = File.ReadAllText(generateContentPath);
            _responses["generateContent"] = content;
            Console.WriteLine($"[MockResponseProvider] Loaded: {generateContentPath}");
        }
        else
        {
            Console.WriteLine($"[MockResponseProvider] Not found: {generateContentPath}, using default");
            _responses["generateContent"] = GetDefaultGenerateContentResponse();
        }

        // Load streaming GenerateContentStream response
        var streamPath = Path.Combine(directory, "GenerateContentStream_Response.json");
        if (File.Exists(streamPath))
        {
            var content = File.ReadAllText(streamPath);
            try
            {
                // Parse as JSON array of chunks
                var chunks = JsonSerializer.Deserialize<JsonElement[]>(content);
                if (chunks != null)
                {
                    _streamingResponses["streamGenerateContent"] = chunks
                        .Select(c => c.GetRawText())
                        .ToArray();
                    Console.WriteLine($"[MockResponseProvider] Loaded streaming: {streamPath} ({chunks.Length} chunks)");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"[MockResponseProvider] Failed to parse streaming response: {ex.Message}");
                LoadDefaultStreamingResponse();
            }
        }
        else
        {
            Console.WriteLine($"[MockResponseProvider] Not found: {streamPath}, using default");
            LoadDefaultStreamingResponse();
        }
    }

    /// <summary>
    /// Get a non-streaming response for the specified endpoint.
    /// </summary>
    public string? GetResponse(string endpointKey)
    {
        return _responses.TryGetValue(endpointKey, out var response) ? response : null;
    }

    /// <summary>
    /// Get streaming response chunks for the specified endpoint.
    /// </summary>
    public string[]? GetStreamingResponse(string endpointKey)
    {
        return _streamingResponses.TryGetValue(endpointKey, out var chunks) ? chunks : null;
    }

    private void LoadDefaultResponses()
    {
        _responses["generateContent"] = GetDefaultGenerateContentResponse();
        LoadDefaultStreamingResponse();
    }

    private void LoadDefaultStreamingResponse()
    {
        _streamingResponses["streamGenerateContent"] = new[]
        {
            """{"candidates":[{"content":{"parts":[{"text":"1"}],"role":"model"}}]}""",
            """{"candidates":[{"content":{"parts":[{"text":", 2"}],"role":"model"}}]}""",
            """{"candidates":[{"content":{"parts":[{"text":", 3"}],"role":"model"}}]}""",
            """{"candidates":[{"content":{"parts":[{"text":", 4"}],"role":"model"}}]}""",
            """{"candidates":[{"content":{"parts":[{"text":", 5"}],"role":"model"},"finishReason":"STOP"}],"usageMetadata":{"promptTokenCount":10,"candidatesTokenCount":5,"totalTokenCount":15}}"""
        };
    }

    private static string GetDefaultGenerateContentResponse()
    {
        return """
        {
          "candidates": [{
            "content": {
              "parts": [{"text": "4"}],
              "role": "model"
            },
            "finishReason": "STOP",
            "safetyRatings": [
              {"category": "HARM_CATEGORY_HATE_SPEECH", "probability": "NEGLIGIBLE"},
              {"category": "HARM_CATEGORY_DANGEROUS_CONTENT", "probability": "NEGLIGIBLE"},
              {"category": "HARM_CATEGORY_HARASSMENT", "probability": "NEGLIGIBLE"},
              {"category": "HARM_CATEGORY_SEXUALLY_EXPLICIT", "probability": "NEGLIGIBLE"}
            ]
          }],
          "usageMetadata": {
            "promptTokenCount": 8,
            "candidatesTokenCount": 1,
            "totalTokenCount": 9
          },
          "modelVersion": "gemini-2.0-flash"
        }
        """;
    }
}
