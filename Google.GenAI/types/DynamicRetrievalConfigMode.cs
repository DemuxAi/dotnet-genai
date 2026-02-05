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
  /// The mode of the predictor to be used in dynamic retrieval.
  /// </summary>

  [JsonConverter(typeof(DynamicRetrievalConfigModeConverter))]
  public readonly record struct DynamicRetrievalConfigMode
      : IEquatable<DynamicRetrievalConfigMode> {
    public string Value { get; }

    private DynamicRetrievalConfigMode(string value) {
      Value = value;
    }

    /// <summary>
    /// Always trigger retrieval.
    /// </summary>
    public static DynamicRetrievalConfigMode ModeUnspecified { get; } = new("MODE_UNSPECIFIED");

    /// <summary>
    /// Run retrieval only when system decides it is necessary.
    /// </summary>
    public static DynamicRetrievalConfigMode ModeDynamic { get; } = new("MODE_DYNAMIC");

    public static IReadOnlyList<DynamicRetrievalConfigMode> AllValues {
      get;
    } = new[] { ModeUnspecified, ModeDynamic };

    public static DynamicRetrievalConfigMode FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new DynamicRetrievalConfigMode("MODE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new DynamicRetrievalConfigMode(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator DynamicRetrievalConfigMode(string value) => FromString(value);

    public bool Equals(DynamicRetrievalConfigMode other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class DynamicRetrievalConfigModeConverter : JsonConverter<DynamicRetrievalConfigMode> {
    public override DynamicRetrievalConfigMode Read(ref Utf8JsonReader reader,
                                                    System.Type typeToConvert,
                                                    JsonSerializerOptions options) {
      var value = reader.GetString();
      return DynamicRetrievalConfigMode.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, DynamicRetrievalConfigMode value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
