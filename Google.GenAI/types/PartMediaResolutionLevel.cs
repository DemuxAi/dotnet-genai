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
  /// The tokenization quality used for given media.
  /// </summary>

  [JsonConverter(typeof(PartMediaResolutionLevelConverter))]
  public readonly record struct PartMediaResolutionLevel : IEquatable<PartMediaResolutionLevel> {
    public string Value { get; }

    private PartMediaResolutionLevel(string value) {
      Value = value;
    }

    /// <summary>
    /// Media resolution has not been set.
    /// </summary>
    public static PartMediaResolutionLevel MediaResolutionUnspecified {
      get;
    } = new("MEDIA_RESOLUTION_UNSPECIFIED");

    /// <summary>
    /// Media resolution set to low.
    /// </summary>
    public static PartMediaResolutionLevel MediaResolutionLow {
      get;
    } = new("MEDIA_RESOLUTION_LOW");

    /// <summary>
    /// Media resolution set to medium.
    /// </summary>
    public static PartMediaResolutionLevel MediaResolutionMedium {
      get;
    } = new("MEDIA_RESOLUTION_MEDIUM");

    /// <summary>
    /// Media resolution set to high.
    /// </summary>
    public static PartMediaResolutionLevel MediaResolutionHigh {
      get;
    } = new("MEDIA_RESOLUTION_HIGH");

    /// <summary>
    /// Media resolution set to ultra high.
    /// </summary>
    public static PartMediaResolutionLevel MediaResolutionUltraHigh {
      get;
    } = new("MEDIA_RESOLUTION_ULTRA_HIGH");

    public static IReadOnlyList<PartMediaResolutionLevel> AllValues {
      get;
    } = new[] { MediaResolutionUnspecified, MediaResolutionLow, MediaResolutionMedium,
                MediaResolutionHigh, MediaResolutionUltraHigh };

    public static PartMediaResolutionLevel FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new PartMediaResolutionLevel("MEDIA_RESOLUTION_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new PartMediaResolutionLevel(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator PartMediaResolutionLevel(string value) => FromString(value);

    public bool Equals(PartMediaResolutionLevel other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class PartMediaResolutionLevelConverter : JsonConverter<PartMediaResolutionLevel> {
    public override PartMediaResolutionLevel Read(ref Utf8JsonReader reader,
                                                  System.Type typeToConvert,
                                                  JsonSerializerOptions options) {
      var value = reader.GetString();
      return PartMediaResolutionLevel.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, PartMediaResolutionLevel value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
