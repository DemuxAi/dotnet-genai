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
  /// ReinforcementTuningStringMatchRewardScorer is used to score parsed responses for string
  /// matching use cases. For example, for math problems, users can use string match scorer to check
  /// if the correct exact answer is generated. Note: Reward returned by the string match reward
  /// function is clipped to be within `[-1, 1]` if wrongAnswerReward or correctAnswerReward are
  /// beyond the range, i.e., `reward = max(min(reward, 1.0), -1.0)`. This data type is not
  /// supported in Gemini API.
  /// </summary>

  public record ReinforcementTuningStringMatchRewardScorer {
    /// <summary>
    /// Wrong answer reward is returned if the parsed response is evaluated as `false`. All wrong
    /// answers get the same reward.
    /// </summary>
    [JsonPropertyName("wrongAnswerReward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double ? WrongAnswerReward { get; set; }

    /// <summary>
    /// Correct answer rewawrd is returned if the parsed response is evaluated as `true`. All
    /// correct answers get the same reward.
    /// </summary>
    [JsonPropertyName("correctAnswerReward")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? CorrectAnswerReward {
            get; set;
          }

    /// <summary>
    /// Uses string match expression to evaluate parsed response.
    /// </summary>
    [JsonPropertyName("stringMatchExpression")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ReinforcementTuningStringMatchRewardScorerStringMatchExpression
        ? StringMatchExpression {
            get; set;
          }

    /// <summary>
    /// Uses json match expression to evaluate parsed response.
    /// </summary>
    [JsonPropertyName("jsonMatchExpression")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ReinforcementTuningStringMatchRewardScorerJsonMatchExpression
        ? JsonMatchExpression {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a ReinforcementTuningStringMatchRewardScorer object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized ReinforcementTuningStringMatchRewardScorer object, or null if
    /// deserialization fails.</returns>
    public static ReinforcementTuningStringMatchRewardScorer
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(
            jsonString, JsonConfig.TypeInfo<ReinforcementTuningStringMatchRewardScorer>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
