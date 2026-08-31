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
  /// Class that represents the parameters for generating videos.
  /// </summary>

  internal record GenerateVideosParameters {
    /// <summary>
    /// ID of the model to use. For a list of models, see Google models
    /// (https://cloud.google.com/vertex-ai/generative-ai/docs/learn/models).
    /// </summary>
    [JsonPropertyName("model")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ? Model { get; set; }

    /// <summary>
    /// The text prompt for generating the videos. Optional if image or video is provided.
    /// </summary>
    [JsonPropertyName("prompt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Prompt {
            get; set;
          }

    /// <summary>
    /// The input image for generating the videos. Optional if prompt is provided. Not allowed if
    /// video is provided.
    /// </summary>
    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Image
        ? Image {
            get; set;
          }

    /// <summary>
    /// The input video for video extension use cases. Optional if prompt is provided. Not allowed
    /// if image is provided.
    /// </summary>
    [JsonPropertyName("video")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Video
        ? Video {
            get; set;
          }

    /// <summary>
    /// A set of source input(s) for video generation.
    /// </summary>
    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GenerateVideosSource
        ? Source {
            get; set;
          }

    /// <summary>
    /// Configuration for generating videos.
    /// </summary>
    [JsonPropertyName("config")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GenerateVideosConfig
        ? Config {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a GenerateVideosParameters object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized GenerateVideosParameters object, or null if deserialization
    /// fails.</returns>
    public static GenerateVideosParameters
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString,
                                          JsonConfig.TypeInfo<GenerateVideosParameters>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
