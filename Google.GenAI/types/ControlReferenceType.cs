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
  /// Enum representing the control type of a control reference image.
  /// </summary>

  [JsonConverter(typeof(ControlReferenceTypeConverter))]
  public readonly record struct ControlReferenceType : IEquatable<ControlReferenceType> {
    public string Value { get; }

    private ControlReferenceType(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static ControlReferenceType ControlTypeDefault { get; } = new("CONTROL_TYPE_DEFAULT");

    /// <summary>
    ///
    /// </summary>
    public static ControlReferenceType ControlTypeCanny { get; } = new("CONTROL_TYPE_CANNY");

    /// <summary>
    ///
    /// </summary>
    public static ControlReferenceType ControlTypeScribble { get; } = new("CONTROL_TYPE_SCRIBBLE");

    /// <summary>
    ///
    /// </summary>
    public static ControlReferenceType ControlTypeFaceMesh { get; } = new("CONTROL_TYPE_FACE_MESH");

    public static IReadOnlyList<ControlReferenceType> AllValues {
      get;
    } = new[] { ControlTypeDefault, ControlTypeCanny, ControlTypeScribble, ControlTypeFaceMesh };

    public static ControlReferenceType FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new ControlReferenceType("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new ControlReferenceType(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator ControlReferenceType(string value) => FromString(value);

    public bool Equals(ControlReferenceType other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class ControlReferenceTypeConverter : JsonConverter<ControlReferenceType> {
    public override ControlReferenceType Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                              JsonSerializerOptions options) {
      var value = reader.GetString();
      return ControlReferenceType.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, ControlReferenceType value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
