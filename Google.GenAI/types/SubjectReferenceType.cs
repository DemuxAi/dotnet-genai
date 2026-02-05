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
  /// Enum representing the subject type of a subject reference image.
  /// </summary>

  [JsonConverter(typeof(SubjectReferenceTypeConverter))]
  public readonly record struct SubjectReferenceType : IEquatable<SubjectReferenceType> {
    public string Value { get; }

    private SubjectReferenceType(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static SubjectReferenceType SubjectTypeDefault { get; } = new("SUBJECT_TYPE_DEFAULT");

    /// <summary>
    ///
    /// </summary>
    public static SubjectReferenceType SubjectTypePerson { get; } = new("SUBJECT_TYPE_PERSON");

    /// <summary>
    ///
    /// </summary>
    public static SubjectReferenceType SubjectTypeAnimal { get; } = new("SUBJECT_TYPE_ANIMAL");

    /// <summary>
    ///
    /// </summary>
    public static SubjectReferenceType SubjectTypeProduct { get; } = new("SUBJECT_TYPE_PRODUCT");

    public static IReadOnlyList<SubjectReferenceType> AllValues {
      get;
    } = new[] { SubjectTypeDefault, SubjectTypePerson, SubjectTypeAnimal, SubjectTypeProduct };

    public static SubjectReferenceType FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new SubjectReferenceType("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new SubjectReferenceType(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator SubjectReferenceType(string value) => FromString(value);

    public bool Equals(SubjectReferenceType other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class SubjectReferenceTypeConverter : JsonConverter<SubjectReferenceType> {
    public override SubjectReferenceType Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                              JsonSerializerOptions options) {
      var value = reader.GetString();
      return SubjectReferenceType.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, SubjectReferenceType value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
