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
  /// TunedModel for the Tuned Model of a Tuning Job.
  /// </summary>

  public record TunedModel {
    /// <summary>
    /// Output only. The resource name of the TunedModel. Format:
    /// `projects/{project}/locations/{location}/models/{model}@{version_id}` When tuning from a
    /// base model, the version_id will be 1. For continuous tuning, the version id will be
    /// incremented by 1 from the last version id in the parent model. E.g.,
    /// `projects/{project}/locations/{location}/models/{model}@{last_version_id + 1}`
    /// </summary>
    [JsonPropertyName("model")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ? Model { get; set; }

    /// <summary>
    /// Output only. A resource name of an Endpoint. Format:
    /// `projects/{project}/locations/{location}/endpoints/{endpoint}`.
    /// </summary>
    [JsonPropertyName("endpoint")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Endpoint {
            get; set;
          }

    /// <summary>
    /// The checkpoints associated with this TunedModel. This field is only populated for tuning
    /// jobs that enable intermediate checkpoints.
    /// </summary>
    [JsonPropertyName("checkpoints")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TunedModelCheckpoint>
        ? Checkpoints {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a TunedModel object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized TunedModel object, or null if deserialization fails.</returns>
    public static TunedModel ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString, JsonConfig.TypeInfo<TunedModel>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
