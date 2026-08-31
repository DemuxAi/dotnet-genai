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
  /// Optional configuration for cached content creation.
  /// </summary>

  public record CreateCachedContentConfig {
    /// <summary>
    /// Used to override HTTP request options.
    /// </summary>
    [JsonPropertyName("httpOptions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HttpOptions ? HttpOptions { get; set; }

    /// <summary>
    /// The TTL for this resource. The expiration time is computed: now + TTL. It is a duration
    /// string, with up to nine fractional digits, terminated by 's'. Example: "3.5s".
    /// </summary>
    [JsonPropertyName("ttl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Ttl {
            get; set;
          }

    /// <summary>
    /// Timestamp of when this resource is considered expired. Uses RFC 3339 format, Example:
    /// 2014-10-02T15:01:23Z.
    /// </summary>
    [JsonPropertyName("expireTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime
        ? ExpireTime {
            get; set;
          }

    /// <summary>
    /// The user-generated meaningful display name of the cached content.
    /// </summary>
    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? DisplayName {
            get; set;
          }

    /// <summary>
    /// The content to cache.
    /// </summary>
    [JsonPropertyName("contents")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Content>
        ? Contents {
            get; set;
          }

    /// <summary>
    /// Developer set system instruction.
    /// </summary>
    [JsonPropertyName("systemInstruction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Content
        ? SystemInstruction {
            get; set;
          }

    /// <summary>
    /// A list of `Tools` the model may use to generate the next response.
    /// </summary>
    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Tool>
        ? Tools {
            get; set;
          }

    /// <summary>
    /// Configuration for the tools to use. This config is shared for all tools.
    /// </summary>
    [JsonPropertyName("toolConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ToolConfig
        ? ToolConfig {
            get; set;
          }

    /// <summary>
    /// The Cloud KMS resource identifier of the customer managed encryption key used to protect a
    /// resource. The key needs to be in the same region as where the compute resource is created.
    /// See https://cloud.google.com/vertex-ai/docs/general/cmek for more details. If this is set,
    /// then all created CachedContent objects will be encrypted with the provided encryption key.
    /// Allowed formats:
    /// projects/{project}/locations/{location}/keyRings/{key_ring}/cryptoKeys/{crypto_key}
    /// </summary>
    [JsonPropertyName("kmsKeyName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? KmsKeyName {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a CreateCachedContentConfig object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized CreateCachedContentConfig object, or null if deserialization
    /// fails.</returns>
    public static CreateCachedContentConfig
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString,
                                          JsonConfig.TypeInfo<CreateCachedContentConfig>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
