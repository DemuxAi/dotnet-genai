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
  /// Tuning spec for Partner models. This data type is not supported in Gemini API.
  /// </summary>

  public record PartnerModelTuningSpec {
    /// <summary>
    /// Hyperparameters for tuning. The accepted hyper_parameters and their valid range of values
    /// will differ depending on the base model.
    /// </summary>
    [JsonPropertyName("hyperParameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object> ? HyperParameters { get; set; }

    /// <summary>
    /// Cloud Storage path to file containing training dataset for tuning. The dataset must be
    /// formatted as a JSONL file.
    /// </summary>
    [JsonPropertyName("trainingDatasetUri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? TrainingDatasetUri {
            get; set;
          }

    /// <summary>
    /// Optional. Cloud Storage path to file containing validation dataset for tuning. The dataset
    /// must be formatted as a JSONL file.
    /// </summary>
    [JsonPropertyName("validationDatasetUri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? ValidationDatasetUri {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a PartnerModelTuningSpec object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized PartnerModelTuningSpec object, or null if deserialization
    /// fails.</returns>
    public static PartnerModelTuningSpec
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString,
                                          JsonConfig.TypeInfo<PartnerModelTuningSpec>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
