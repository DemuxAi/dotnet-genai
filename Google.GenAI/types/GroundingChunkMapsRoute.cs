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
  /// Route information from Google Maps. This data type is not supported in Gemini API.
  /// </summary>

  public record GroundingChunkMapsRoute {
    /// <summary>
    /// The total distance of the route, in meters.
    /// </summary>
    [JsonPropertyName("distanceMeters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int ? DistanceMeters { get; set; }

    /// <summary>
    /// The total duration of the route.
    /// </summary>
    [JsonPropertyName("duration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Duration {
            get; set;
          }

    /// <summary>
    /// An encoded polyline of the route. See
    /// https://developers.google.com/maps/documentation/utilities/polylinealgorithm
    /// </summary>
    [JsonPropertyName("encodedPolyline")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? EncodedPolyline {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a GroundingChunkMapsRoute object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized GroundingChunkMapsRoute object, or null if deserialization
    /// fails.</returns>
    public static GroundingChunkMapsRoute
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize<GroundingChunkMapsRoute>(jsonString, options);
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
