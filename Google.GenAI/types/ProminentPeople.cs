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
  /// Controls whether prominent people (celebrities) generation is allowed. If used with
  /// personGeneration, personGeneration enum would take precedence. For instance, if ALLOW_NONE is
  /// set, all person generation would be blocked. If this field is unspecified, the default
  /// behavior is to allow prominent people. This enum is not supported in Gemini API.
  /// </summary>

  [JsonConverter(typeof(ProminentPeopleConverter))]
  public readonly record struct ProminentPeople : IEquatable<ProminentPeople> {
    public string Value { get; }

    private ProminentPeople(string value) {
      Value = value;
    }

    /// <summary>
    /// Unspecified value. The model will proceed with the default behavior, which is to allow
    /// generation of prominent people.
    /// </summary>
    public static ProminentPeople ProminentPeopleUnspecified {
      get;
    } = new("PROMINENT_PEOPLE_UNSPECIFIED");

    /// <summary>
    /// Allows the model to generate images of prominent people.
    /// </summary>
    public static ProminentPeople AllowProminentPeople { get; } = new("ALLOW_PROMINENT_PEOPLE");

    /// <summary>
    /// Prevents the model from generating images of prominent people.
    /// </summary>
    public static ProminentPeople BlockProminentPeople { get; } = new("BLOCK_PROMINENT_PEOPLE");

    public static IReadOnlyList<ProminentPeople> AllValues {
      get;
    } = new[] { ProminentPeopleUnspecified, AllowProminentPeople, BlockProminentPeople };

    public static ProminentPeople FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new ProminentPeople("PROMINENT_PEOPLE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new ProminentPeople(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator ProminentPeople(string value) => FromString(value);

    public bool Equals(ProminentPeople other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class ProminentPeopleConverter : JsonConverter<ProminentPeople> {
    public override ProminentPeople Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                         JsonSerializerOptions options) {
      var value = reader.GetString();
      return ProminentPeople.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, ProminentPeople value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
