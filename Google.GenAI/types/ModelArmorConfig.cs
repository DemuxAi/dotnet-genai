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
  /// Configuration for Model Armor integrations of prompt and responses. This data type is not
  /// supported in Gemini API.
  /// </summary>

  public record ModelArmorConfig {
    /// <summary>
    /// Optional. The name of the Model Armor template to use for prompt sanitization.
    /// </summary>
    [JsonPropertyName("promptTemplateName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ? PromptTemplateName { get; set; }

    /// <summary>
    /// Optional. The name of the Model Armor template to use for response sanitization.
    /// </summary>
    [JsonPropertyName("responseTemplateName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? ResponseTemplateName {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a ModelArmorConfig object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized ModelArmorConfig object, or null if deserialization
    /// fails.</returns>
    public static ModelArmorConfig
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize<ModelArmorConfig>(jsonString, options);
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
