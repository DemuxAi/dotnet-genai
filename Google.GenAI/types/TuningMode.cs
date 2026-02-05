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
  /// Tuning mode. This enum is not supported in Gemini API.
  /// </summary>

  [JsonConverter(typeof(TuningModeConverter))]
  public readonly record struct TuningMode : IEquatable<TuningMode> {
    public string Value { get; }

    private TuningMode(string value) {
      Value = value;
    }

    /// <summary>
    /// Tuning mode is unspecified.
    /// </summary>
    public static TuningMode TuningModeUnspecified { get; } = new("TUNING_MODE_UNSPECIFIED");

    /// <summary>
    /// Full fine-tuning mode.
    /// </summary>
    public static TuningMode TuningModeFull { get; } = new("TUNING_MODE_FULL");

    /// <summary>
    /// PEFT adapter tuning mode.
    /// </summary>
    public static TuningMode TuningModePeftAdapter { get; } = new("TUNING_MODE_PEFT_ADAPTER");

    public static IReadOnlyList<TuningMode> AllValues {
      get;
    } = new[] { TuningModeUnspecified, TuningModeFull, TuningModePeftAdapter };

    public static TuningMode FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new TuningMode("TUNING_MODE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new TuningMode(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator TuningMode(string value) => FromString(value);

    public bool Equals(TuningMode other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class TuningModeConverter : JsonConverter<TuningMode> {
    public override TuningMode Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                    JsonSerializerOptions options) {
      var value = reader.GetString();
      return TuningMode.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, TuningMode value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
