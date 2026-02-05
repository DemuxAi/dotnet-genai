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
  /// Programming language of the `code`.
  /// </summary>

  [JsonConverter(typeof(LanguageConverter))]
  public readonly record struct Language : IEquatable<Language> {
    public string Value { get; }

    private Language(string value) {
      Value = value;
    }

    /// <summary>
    /// Unspecified language. This value should not be used.
    /// </summary>
    public static Language LanguageUnspecified { get; } = new("LANGUAGE_UNSPECIFIED");

    /// <summary>
    /// Python greater than or equal to 3.10, with numpy and simpy available.
    /// </summary>
    public static Language Python { get; } = new("PYTHON");

    public static IReadOnlyList<Language> AllValues {
      get;
    } = new[] { LanguageUnspecified, Python };

    public static Language FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new Language("LANGUAGE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new Language(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator Language(string value) => FromString(value);

    public bool Equals(Language other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class LanguageConverter : JsonConverter<Language> {
    public override Language Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                  JsonSerializerOptions options) {
      var value = reader.GetString();
      return Language.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, Language value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
