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
  /// The `Status` type defines a logical error model that is suitable for different programming
  /// environments, including REST APIs and RPC APIs. It is used by gRPC (https://github.com/grpc).
  /// Each `Status` message contains three pieces of data: error code, error message, and error
  /// details. You can find out more about this error model and how to work with it in the API
  /// Design Guide (https://cloud.google.com/apis/design/errors). This data type is not supported in
  /// Gemini API.
  /// </summary>

  public record GoogleRpcStatus {
    /// <summary>
    /// The status code, which should be an enum value of google.rpc.Code.
    /// </summary>
    [JsonPropertyName("code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int ? Code { get; set; }

    /// <summary>
    /// A list of messages that carry the error details. There is a common set of message types for
    /// APIs to use.
    /// </summary>
    [JsonPropertyName("details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Dictionary<string, object>>
        ? Details {
            get; set;
          }

    /// <summary>
    /// A developer-facing error message, which should be in English. Any user-facing error message
    /// should be localized and sent in the google.rpc.Status.details field, or localized by the
    /// client.
    /// </summary>
    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Message {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a GoogleRpcStatus object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized GoogleRpcStatus object, or null if deserialization
    /// fails.</returns>
    public static GoogleRpcStatus
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString,
                                          JsonConfig.TypeInfo<GoogleRpcStatus>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
