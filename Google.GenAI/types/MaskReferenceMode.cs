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
  /// Enum representing the mask mode of a mask reference image.
  /// </summary>

  [JsonConverter(typeof(MaskReferenceModeConverter))]
  public readonly record struct MaskReferenceMode : IEquatable<MaskReferenceMode> {
    public string Value { get; }

    private MaskReferenceMode(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static MaskReferenceMode MaskModeDefault { get; } = new("MASK_MODE_DEFAULT");

    /// <summary>
    ///
    /// </summary>
    public static MaskReferenceMode MaskModeUserProvided { get; } = new("MASK_MODE_USER_PROVIDED");

    /// <summary>
    ///
    /// </summary>
    public static MaskReferenceMode MaskModeBackground { get; } = new("MASK_MODE_BACKGROUND");

    /// <summary>
    ///
    /// </summary>
    public static MaskReferenceMode MaskModeForeground { get; } = new("MASK_MODE_FOREGROUND");

    /// <summary>
    ///
    /// </summary>
    public static MaskReferenceMode MaskModeSemantic { get; } = new("MASK_MODE_SEMANTIC");

    public static IReadOnlyList<MaskReferenceMode> AllValues {
      get;
    } = new[] { MaskModeDefault, MaskModeUserProvided, MaskModeBackground, MaskModeForeground,
                MaskModeSemantic };

    public static MaskReferenceMode FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new MaskReferenceMode("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new MaskReferenceMode(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator MaskReferenceMode(string value) => FromString(value);

    public bool Equals(MaskReferenceMode other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class MaskReferenceModeConverter : JsonConverter<MaskReferenceMode> {
    public override MaskReferenceMode Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                           JsonSerializerOptions options) {
      var value = reader.GetString();
      return MaskReferenceMode.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, MaskReferenceMode value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
