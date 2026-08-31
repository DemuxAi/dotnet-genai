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
  /// A trained machine learning model.
  /// </summary>

  public record Model {
    /// <summary>
    /// Resource name of the model.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ? Name { get; set; }

    /// <summary>
    /// Display name of the model.
    /// </summary>
    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? DisplayName {
            get; set;
          }

    /// <summary>
    /// Description of the model.
    /// </summary>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Description {
            get; set;
          }

    /// <summary>
    /// Version ID of the model. A new version is committed when a new model version is uploaded or
    /// trained under an existing model ID. The version ID is an auto-incrementing decimal number in
    /// string representation.
    /// </summary>
    [JsonPropertyName("version")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Version {
            get; set;
          }

    /// <summary>
    /// List of deployed models created from this base model. Note that a model could have been
    /// deployed to endpoints in different locations.
    /// </summary>
    [JsonPropertyName("endpoints")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Endpoint>
        ? Endpoints {
            get; set;
          }

    /// <summary>
    /// Labels with user-defined metadata to organize your models.
    /// </summary>
    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>
        ? Labels {
            get; set;
          }

    /// <summary>
    /// Information about the tuned model from the base model.
    /// </summary>
    [JsonPropertyName("tunedModelInfo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TunedModelInfo
        ? TunedModelInfo {
            get; set;
          }

    /// <summary>
    /// The maximum number of input tokens that the model can handle.
    /// </summary>
    [JsonPropertyName("inputTokenLimit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int
        ? InputTokenLimit {
            get; set;
          }

    /// <summary>
    /// The maximum number of output tokens that the model can generate.
    /// </summary>
    [JsonPropertyName("outputTokenLimit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int
        ? OutputTokenLimit {
            get; set;
          }

    /// <summary>
    /// List of actions that are supported by the model.
    /// </summary>
    [JsonPropertyName("supportedActions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>
        ? SupportedActions {
            get; set;
          }

    /// <summary>
    /// The default checkpoint id of a model version.
    /// </summary>
    [JsonPropertyName("defaultCheckpointId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? DefaultCheckpointId {
            get; set;
          }

    /// <summary>
    /// The checkpoints of the model.
    /// </summary>
    [JsonPropertyName("checkpoints")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Checkpoint>
        ? Checkpoints {
            get; set;
          }

    /// <summary>
    /// Temperature value used for sampling set when the dataset was saved. This value is used to
    /// tune the degree of randomness.
    /// </summary>
    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? Temperature {
            get; set;
          }

    /// <summary>
    /// The maximum temperature value used for sampling set when the dataset was saved. This value
    /// is used to tune the degree of randomness.
    /// </summary>
    [JsonPropertyName("maxTemperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? MaxTemperature {
            get; set;
          }

    /// <summary>
    /// Optional. Specifies the nucleus sampling threshold. The model considers only the smallest
    /// set of tokens whose cumulative probability is at least `top_p`. This helps generate more
    /// diverse and less repetitive responses. For example, a `top_p` of 0.9 means the model
    /// considers tokens until the cumulative probability of the tokens to select from reaches 0.9.
    /// It's recommended to adjust either temperature or `top_p`, but not both.
    /// </summary>
    [JsonPropertyName("topP")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? TopP {
            get; set;
          }

    /// <summary>
    /// Optional. Specifies the top-k sampling threshold. The model considers only the top k most
    /// probable tokens for the next token. This can be useful for generating more coherent and less
    /// random text. For example, a `top_k` of 40 means the model will choose the next word from the
    /// 40 most likely words.
    /// </summary>
    [JsonPropertyName("topK")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int
        ? TopK {
            get; set;
          }

    /// <summary>
    /// Whether the model supports thinking features. If true, thoughts are returned only if the
    /// model supports thought and thoughts are available.
    /// </summary>
    [JsonPropertyName("thinking")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool
        ? Thinking {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a Model object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized Model object, or null if deserialization fails.</returns>
    public static Model ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString, JsonConfig.TypeInfo<Model>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
