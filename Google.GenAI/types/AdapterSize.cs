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
  /// Adapter size for tuning. This enum is not supported in Gemini API.
  /// </summary>

  [JsonConverter(typeof(AdapterSizeConverter))]
  public readonly record struct AdapterSize : IEquatable<AdapterSize> {
    public string Value { get; }

    private AdapterSize(string value) {
      Value = value;
    }

    /// <summary>
    /// Adapter size is unspecified.
    /// </summary>
    public static AdapterSize AdapterSizeUnspecified { get; } = new("ADAPTER_SIZE_UNSPECIFIED");

    /// <summary>
    /// Adapter size 1.
    /// </summary>
    public static AdapterSize AdapterSizeOne { get; } = new("ADAPTER_SIZE_ONE");

    /// <summary>
    /// Adapter size 2.
    /// </summary>
    public static AdapterSize AdapterSizeTwo { get; } = new("ADAPTER_SIZE_TWO");

    /// <summary>
    /// Adapter size 4.
    /// </summary>
    public static AdapterSize AdapterSizeFour { get; } = new("ADAPTER_SIZE_FOUR");

    /// <summary>
    /// Adapter size 8.
    /// </summary>
    public static AdapterSize AdapterSizeEight { get; } = new("ADAPTER_SIZE_EIGHT");

    /// <summary>
    /// Adapter size 16.
    /// </summary>
    public static AdapterSize AdapterSizeSixteen { get; } = new("ADAPTER_SIZE_SIXTEEN");

    /// <summary>
    /// Adapter size 32.
    /// </summary>
    public static AdapterSize AdapterSizeThirtyTwo { get; } = new("ADAPTER_SIZE_THIRTY_TWO");

    public static IReadOnlyList<AdapterSize> AllValues {
      get;
    } = new[] { AdapterSizeUnspecified, AdapterSizeOne,     AdapterSizeTwo,      AdapterSizeFour,
                AdapterSizeEight,       AdapterSizeSixteen, AdapterSizeThirtyTwo };

    public static AdapterSize FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new AdapterSize("ADAPTER_SIZE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new AdapterSize(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator AdapterSize(string value) => FromString(value);

    public bool Equals(AdapterSize other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class AdapterSizeConverter : JsonConverter<AdapterSize> {
    public override AdapterSize Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                     JsonSerializerOptions options) {
      var value = reader.GetString();
      return AdapterSize.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, AdapterSize value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
