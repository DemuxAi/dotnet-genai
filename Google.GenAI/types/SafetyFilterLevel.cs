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
  /// Enum that controls the safety filter level for objectionable content.
  /// </summary>

  [JsonConverter(typeof(SafetyFilterLevelConverter))]
  public readonly record struct SafetyFilterLevel : IEquatable<SafetyFilterLevel> {
    public string Value { get; }

    private SafetyFilterLevel(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static SafetyFilterLevel BlockLowAndAbove { get; } = new("BLOCK_LOW_AND_ABOVE");

    /// <summary>
    ///
    /// </summary>
    public static SafetyFilterLevel BlockMediumAndAbove { get; } = new("BLOCK_MEDIUM_AND_ABOVE");

    /// <summary>
    ///
    /// </summary>
    public static SafetyFilterLevel BlockOnlyHigh { get; } = new("BLOCK_ONLY_HIGH");

    /// <summary>
    ///
    /// </summary>
    public static SafetyFilterLevel BlockNone { get; } = new("BLOCK_NONE");

    public static IReadOnlyList<SafetyFilterLevel> AllValues {
      get;
    } = new[] { BlockLowAndAbove, BlockMediumAndAbove, BlockOnlyHigh, BlockNone };

    public static SafetyFilterLevel FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new SafetyFilterLevel("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new SafetyFilterLevel(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator SafetyFilterLevel(string value) => FromString(value);

    public bool Equals(SafetyFilterLevel other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class SafetyFilterLevelConverter : JsonConverter<SafetyFilterLevel> {
    public override SafetyFilterLevel Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                           JsonSerializerOptions options) {
      var value = reader.GetString();
      return SafetyFilterLevel.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, SafetyFilterLevel value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
