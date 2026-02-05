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
  /// The type of the voice activity signal.
  /// </summary>

  [JsonConverter(typeof(VoiceActivityTypeConverter))]
  public readonly record struct VoiceActivityType : IEquatable<VoiceActivityType> {
    public string Value { get; }

    private VoiceActivityType(string value) {
      Value = value;
    }

    /// <summary>
    /// The default is VOICE_ACTIVITY_TYPE_UNSPECIFIED.
    /// </summary>
    public static VoiceActivityType TypeUnspecified { get; } = new("TYPE_UNSPECIFIED");

    /// <summary>
    /// Start of sentence signal.
    /// </summary>
    public static VoiceActivityType ActivityStart { get; } = new("ACTIVITY_START");

    /// <summary>
    /// End of sentence signal.
    /// </summary>
    public static VoiceActivityType ActivityEnd { get; } = new("ACTIVITY_END");

    public static IReadOnlyList<VoiceActivityType> AllValues {
      get;
    } = new[] { TypeUnspecified, ActivityStart, ActivityEnd };

    public static VoiceActivityType FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new VoiceActivityType("TYPE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new VoiceActivityType(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator VoiceActivityType(string value) => FromString(value);

    public bool Equals(VoiceActivityType other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class VoiceActivityTypeConverter : JsonConverter<VoiceActivityType> {
    public override VoiceActivityType Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                           JsonSerializerOptions options) {
      var value = reader.GetString();
      return VoiceActivityType.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, VoiceActivityType value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
