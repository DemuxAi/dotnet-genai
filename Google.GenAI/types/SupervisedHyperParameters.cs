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
  /// Hyperparameters for SFT. This data type is not supported in Gemini API.
  /// </summary>

  public record SupervisedHyperParameters {
    /// <summary>
    /// Optional. Adapter size for tuning.
    /// </summary>
    [JsonPropertyName("adapterSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AdapterSize ? AdapterSize { get; set; }

    /// <summary>
    /// Optional. Batch size for tuning. This feature is only available for open source models.
    /// </summary>
    [JsonPropertyName("batchSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(StringToNullableLongConverter))]
    public long
        ? BatchSize {
            get; set;
          }

    /// <summary>
    /// Optional. Number of complete passes the model makes over the entire training dataset during
    /// training.
    /// </summary>
    [JsonPropertyName("epochCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(StringToNullableLongConverter))]
    public long
        ? EpochCount {
            get; set;
          }

    /// <summary>
    /// Optional. Learning rate for tuning. Mutually exclusive with `learning_rate_multiplier`. This
    /// feature is only available for open source models.
    /// </summary>
    [JsonPropertyName("learningRate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? LearningRate {
            get; set;
          }

    /// <summary>
    /// Optional. Multiplier for adjusting the default learning rate. Mutually exclusive with
    /// `learning_rate`. This feature is only available for 1P models.
    /// </summary>
    [JsonPropertyName("learningRateMultiplier")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? LearningRateMultiplier {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a SupervisedHyperParameters object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized SupervisedHyperParameters object, or null if deserialization
    /// fails.</returns>
    public static SupervisedHyperParameters
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString,
                                          JsonConfig.TypeInfo<SupervisedHyperParameters>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
