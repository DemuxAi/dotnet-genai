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
  /// Reinforcement tuning autorater scorer.
  /// </summary>

  public record ReinforcementTuningAutoraterScorer {
    /// <summary>
    /// Autorater config for evaluation.
    /// </summary>
    [JsonPropertyName("autoraterConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AutoraterConfig ? AutoraterConfig { get; set; }

    /// <summary>
    /// The prompt for an autorater to scorer the parsed sample response. This field supports the
    /// following placeholders that will be replaced before scoring: - {{prompt}} - {{response}} -
    /// {{system_instruction}} - {{references.key}}
    /// </summary>
    [JsonPropertyName("autoraterPrompt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? AutoraterPrompt {
            get; set;
          }

    /// <summary>
    /// Parses autorater returned response for scoring. For example, if the autorater response has
    /// reward stored in the `2.0` block, defining a parsing response config using regex `".*(.*?)"`
    /// will return a score `"2.0"`.
    /// </summary>
    [JsonPropertyName("autoraterResponseParseConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ReinforcementTuningParseResponseConfig
        ? AutoraterResponseParseConfig {
            get; set;
          }

    /// <summary>
    /// Scores autorater responses by directly converting parsed autorater response to a float
    /// reward. Note: Reward is clipped to be within `[-1, 1]`, i.e., `reward =
    /// max(min(reward, 1.0), -1.0)`.
    /// </summary>
    [JsonPropertyName("parsedResponseConversionScorer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ReinforcementTuningAutoraterScorerParsedResponseConversionScorer
        ? ParsedResponseConversionScorer {
            get; set;
          }

    /// <summary>
    /// Scores autorater responses by using string match reward scorer.
    /// </summary>
    [JsonPropertyName("exactMatchScorer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ReinforcementTuningAutoraterScorerExactMatchScorer
        ? ExactMatchScorer {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a ReinforcementTuningAutoraterScorer object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized ReinforcementTuningAutoraterScorer object, or null if
    /// deserialization fails.</returns>
    public static ReinforcementTuningAutoraterScorer
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(
            jsonString, JsonConfig.TypeInfo<ReinforcementTuningAutoraterScorer>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
