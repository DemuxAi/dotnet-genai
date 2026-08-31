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
  /// A pre-tuned model for continuous tuning. This data type is not supported in Gemini API.
  /// </summary>

  public record PreTunedModel {
    /// <summary>
    /// Output only. The name of the base model this PreTunedModel was tuned from.
    /// </summary>
    [JsonPropertyName("baseModel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ? BaseModel { get; set; }

    /// <summary>
    /// Optional. The source checkpoint id. If not specified, the default checkpoint will be used.
    /// </summary>
    [JsonPropertyName("checkpointId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? CheckpointId {
            get; set;
          }

    /// <summary>
    /// The resource name of the Model. E.g., a model resource name with a specified version id or
    /// alias: `projects/{project}/locations/{location}/models/{model}@{version_id}`
    /// `projects/{project}/locations/{location}/models/{model}@{alias}` Or, omit the version id to
    /// use the default version: `projects/{project}/locations/{location}/models/{model}`
    /// </summary>
    [JsonPropertyName("tunedModelName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? TunedModelName {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a PreTunedModel object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized PreTunedModel object, or null if deserialization fails.</returns>
    public static PreTunedModel
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString, JsonConfig.TypeInfo<PreTunedModel>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
