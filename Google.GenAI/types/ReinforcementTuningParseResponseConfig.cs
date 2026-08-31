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
  /// Defines how to parse sample response config for reinforcement tuning. The parsed response
  /// (i.e., substring) will be passed to the reward functions. For example, the input prompt might
  /// be: > "Perform step-by-step thoughts first to problem A, finally output answer in the
  /// &lt;ans&gt; &lt;/ans&gt; block." The sample response from the model under tuning might look
  /// like: > "&lt;ans&gt;Yes&lt;/ans&gt;" Here, users can define the following parse config: ``` {
  /// "parseType": "REGEX_EXTRACT", "regexExtractExpression": ".*(.*?)" } ``` The resulting parsed
  /// response would be `"Yes"` and will be passed to the reward functions for evaluating rewards.
  /// This data type is not supported in Gemini API.
  /// </summary>

  public record ReinforcementTuningParseResponseConfig {
    /// <summary>
    /// Defines the type for parsing sample response.
    /// </summary>
    [JsonPropertyName("parseType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseParseType ? ParseType { get; set; }

    /// <summary>
    /// Defines the regex for extracting the important part of sample response. This field is only
    /// used when parse_type is ResponseParseType.REGEX_EXTRACT.
    /// </summary>
    [JsonPropertyName("regexExtractExpression")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? RegexExtractExpression {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a ReinforcementTuningParseResponseConfig object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized ReinforcementTuningParseResponseConfig object, or null if
    /// deserialization fails.</returns>
    public static ReinforcementTuningParseResponseConfig
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(
            jsonString, JsonConfig.TypeInfo<ReinforcementTuningParseResponseConfig>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
