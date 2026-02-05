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
  /// Enum that controls the generation of people.
  /// </summary>

  [JsonConverter(typeof(PersonGenerationConverter))]
  public readonly record struct PersonGeneration : IEquatable<PersonGeneration> {
    public string Value { get; }

    private PersonGeneration(string value) {
      Value = value;
    }

    /// <summary>
    /// Block generation of images of people.
    /// </summary>
    public static PersonGeneration DontAllow { get; } = new("DONT_ALLOW");

    /// <summary>
    /// Generate images of adults, but not children.
    /// </summary>
    public static PersonGeneration AllowAdult { get; } = new("ALLOW_ADULT");

    /// <summary>
    /// Generate images that include adults and children.
    /// </summary>
    public static PersonGeneration AllowAll { get; } = new("ALLOW_ALL");

    public static IReadOnlyList<PersonGeneration> AllValues {
      get;
    } = new[] { DontAllow, AllowAdult, AllowAll };

    public static PersonGeneration FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new PersonGeneration("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new PersonGeneration(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator PersonGeneration(string value) => FromString(value);

    public bool Equals(PersonGeneration other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class PersonGenerationConverter : JsonConverter<PersonGeneration> {
    public override PersonGeneration Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                          JsonSerializerOptions options) {
      var value = reader.GetString();
      return PersonGeneration.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, PersonGeneration value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
