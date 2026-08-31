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
  /// A file uploaded to the API.
  /// </summary>

  public record File {
    /// <summary>
    /// The `File` resource name. The ID (name excluding the "files/" prefix) can contain up to 40
    /// characters that are lowercase alphanumeric or dashes (-). The ID cannot start or end with a
    /// dash. If the name is empty on create, a unique name will be generated. Example:
    /// `files/123-456`
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ? Name { get; set; }

    /// <summary>
    /// Optional. The human-readable display name for the `File`. The display name must be no more
    /// than 512 characters in length, including spaces. Example: 'Welcome Image'
    /// </summary>
    [JsonPropertyName("displayName")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? DisplayName {
            get; set;
          }

    /// <summary>
    /// Output only. MIME type of the file.
    /// </summary>
    [JsonPropertyName("mimeType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? MimeType {
            get; set;
          }

    /// <summary>
    /// Output only. Size of the file in bytes.
    /// </summary>
    [JsonPropertyName("sizeBytes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(StringToNullableLongConverter))]
    public long
        ? SizeBytes {
            get; set;
          }

    /// <summary>
    /// Output only. The timestamp of when the `File` was created.
    /// </summary>
    [JsonPropertyName("createTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime
        ? CreateTime {
            get; set;
          }

    /// <summary>
    /// Output only. The timestamp of when the `File` will be deleted. Only set if the `File` is
    /// scheduled to expire.
    /// </summary>
    [JsonPropertyName("expirationTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime
        ? ExpirationTime {
            get; set;
          }

    /// <summary>
    /// Output only. The timestamp of when the `File` was last updated.
    /// </summary>
    [JsonPropertyName("updateTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime
        ? UpdateTime {
            get; set;
          }

    /// <summary>
    /// Output only. SHA-256 hash of the uploaded bytes. The hash value is encoded in base64 format.
    /// </summary>
    [JsonPropertyName("sha256Hash")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Sha256Hash {
            get; set;
          }

    /// <summary>
    /// Output only. The URI of the `File`.
    /// </summary>
    [JsonPropertyName("uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? Uri {
            get; set;
          }

    /// <summary>
    /// Output only. The URI of the `File`, only set for downloadable (generated) files.
    /// </summary>
    [JsonPropertyName("downloadUri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string
        ? DownloadUri {
            get; set;
          }

    /// <summary>
    /// Output only. Processing state of the File.
    /// </summary>
    [JsonPropertyName("state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FileState
        ? State {
            get; set;
          }

    /// <summary>
    /// Output only. The source of the `File`.
    /// </summary>
    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FileSource
        ? Source {
            get; set;
          }

    /// <summary>
    /// Output only. Metadata for a video.
    /// </summary>
    [JsonPropertyName("videoMetadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>
        ? VideoMetadata {
            get; set;
          }

    /// <summary>
    /// Output only. Error status if File processing failed.
    /// </summary>
    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FileStatus
        ? Error {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a File object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized File object, or null if deserialization fails.</returns>
    public static File ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(jsonString, JsonConfig.TypeInfo<File>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
