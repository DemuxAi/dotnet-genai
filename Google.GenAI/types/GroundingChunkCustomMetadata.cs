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

// Auto-generated code. Do not edit.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.GenAI.Serialization;

namespace Google.GenAI.Types {
  /// <summary>
  /// User provided metadata about the GroundingFact. This data type is not supported in Vertex AI.
  /// </summary>

  public record GroundingChunkCustomMetadata {
    /// <summary>
    /// The key of the metadata.
    /// </summary>
    [JsonPropertyName("key")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ? Key { get; set; }

    /// <summary>
    /// Optional. The numeric value of the metadata. The expected range for this value depends on
    /// the specific `key` used.
    /// </summary>
    [JsonPropertyName("numericValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? NumericValue {
            get; set;
          }

    /// <summary>
    /// Optional. A list of string values for the metadata.
    /// </summary>
    [JsonPropertyName("stringListValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GroundingChunkStringList
        ? StringListValue {
            get; set;
          }

    /// <summary>
    /// Optional. The string value of the metadata.
    /// </summary>
    [JsonPropertyName("stringValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? StringValue {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a GroundingChunkCustomMetadata object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized GroundingChunkCustomMetadata object, or null if deserialization
    /// fails.</returns>
    public static GroundingChunkCustomMetadata
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize<GroundingChunkCustomMetadata>(jsonString, options);
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
