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
  /// Enum that represents the segmentation mode.
  /// </summary>

  [JsonConverter(typeof(SegmentModeConverter))]
  public readonly record struct SegmentMode : IEquatable<SegmentMode> {
    public string Value { get; }

    private SegmentMode(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static SegmentMode Foreground { get; } = new("FOREGROUND");

    /// <summary>
    ///
    /// </summary>
    public static SegmentMode Background { get; } = new("BACKGROUND");

    /// <summary>
    ///
    /// </summary>
    public static SegmentMode Prompt { get; } = new("PROMPT");

    /// <summary>
    ///
    /// </summary>
    public static SegmentMode Semantic { get; } = new("SEMANTIC");

    /// <summary>
    ///
    /// </summary>
    public static SegmentMode Interactive { get; } = new("INTERACTIVE");

    public static IReadOnlyList<SegmentMode> AllValues {
      get;
    } = new[] { Foreground, Background, Prompt, Semantic, Interactive };

    public static SegmentMode FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new SegmentMode("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new SegmentMode(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator SegmentMode(string value) => FromString(value);

    public bool Equals(SegmentMode other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class SegmentModeConverter : JsonConverter<SegmentMode> {
    public override SegmentMode Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                     JsonSerializerOptions options) {
      var value = reader.GetString();
      return SegmentMode.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, SegmentMode value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
