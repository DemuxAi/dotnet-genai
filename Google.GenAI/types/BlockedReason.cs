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

using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;

namespace Google.GenAI.Types {
  /// <summary>
  /// Output only. The reason why the prompt was blocked.
  /// </summary>

  [JsonConverter(typeof(BlockedReasonConverter))]
  public readonly record struct BlockedReason : IEquatable<BlockedReason> {
    public string Value { get; }

    private BlockedReason(string value) {
      Value = value;
    }

    /// <summary>
    /// The blocked reason is unspecified.
    /// </summary>
    public static BlockedReason BlockedReasonUnspecified {
      get;
    } = new("BLOCKED_REASON_UNSPECIFIED");

    /// <summary>
    /// The prompt was blocked for safety reasons.
    /// </summary>
    public static BlockedReason Safety { get; } = new("SAFETY");

    /// <summary>
    /// The prompt was blocked for other reasons. For example, it may be due to the prompt's
    /// language, or because it contains other harmful content.
    /// </summary>
    public static BlockedReason Other { get; } = new("OTHER");

    /// <summary>
    /// The prompt was blocked because it contains a term from the terminology blocklist.
    /// </summary>
    public static BlockedReason Blocklist { get; } = new("BLOCKLIST");

    /// <summary>
    /// The prompt was blocked because it contains prohibited content.
    /// </summary>
    public static BlockedReason ProhibitedContent { get; } = new("PROHIBITED_CONTENT");

    /// <summary>
    /// The prompt was blocked because it contains content that is unsafe for image generation.
    /// </summary>
    public static BlockedReason ImageSafety { get; } = new("IMAGE_SAFETY");

    /// <summary>
    /// The prompt was blocked by Model Armor. This enum value is not supported in Gemini API.
    /// </summary>
    public static BlockedReason ModelArmor { get; } = new("MODEL_ARMOR");

    /// <summary>
    /// The prompt was blocked as a jailbreak attempt. This enum value is not supported in Gemini
    /// API.
    /// </summary>
    public static BlockedReason Jailbreak { get; } = new("JAILBREAK");

    public static IReadOnlyList<BlockedReason> AllValues {
      get;
    } = new[] { BlockedReasonUnspecified, Safety,      Other,      Blocklist,
                ProhibitedContent,        ImageSafety, ModelArmor, Jailbreak };

    public static BlockedReason FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new BlockedReason("BLOCKED_REASON_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new BlockedReason(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator BlockedReason(string value) => FromString(value);

    public bool Equals(BlockedReason other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class BlockedReasonConverter : JsonConverter<BlockedReason> {
    public override BlockedReason Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                       JsonSerializerOptions options) {
      var value = reader.GetString();
      return BlockedReason.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, BlockedReason value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
