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
  /// Sites with confidence level chosen &amp; above this value will be blocked from the search
  /// results. This enum is not supported in Gemini API.
  /// </summary>

  [JsonConverter(typeof(PhishBlockThresholdConverter))]
  public readonly record struct PhishBlockThreshold : IEquatable<PhishBlockThreshold> {
    public string Value { get; }

    private PhishBlockThreshold(string value) {
      Value = value;
    }

    /// <summary>
    /// Defaults to unspecified.
    /// </summary>
    public static PhishBlockThreshold PhishBlockThresholdUnspecified {
      get;
    } = new("PHISH_BLOCK_THRESHOLD_UNSPECIFIED");

    /// <summary>
    /// Blocks Low and above confidence URL that is risky.
    /// </summary>
    public static PhishBlockThreshold BlockLowAndAbove { get; } = new("BLOCK_LOW_AND_ABOVE");

    /// <summary>
    /// Blocks Medium and above confidence URL that is risky.
    /// </summary>
    public static PhishBlockThreshold BlockMediumAndAbove { get; } = new("BLOCK_MEDIUM_AND_ABOVE");

    /// <summary>
    /// Blocks High and above confidence URL that is risky.
    /// </summary>
    public static PhishBlockThreshold BlockHighAndAbove { get; } = new("BLOCK_HIGH_AND_ABOVE");

    /// <summary>
    /// Blocks Higher and above confidence URL that is risky.
    /// </summary>
    public static PhishBlockThreshold BlockHigherAndAbove { get; } = new("BLOCK_HIGHER_AND_ABOVE");

    /// <summary>
    /// Blocks Very high and above confidence URL that is risky.
    /// </summary>
    public static PhishBlockThreshold BlockVeryHighAndAbove {
      get;
    } = new("BLOCK_VERY_HIGH_AND_ABOVE");

    /// <summary>
    /// Blocks Extremely high confidence URL that is risky.
    /// </summary>
    public static PhishBlockThreshold BlockOnlyExtremelyHigh {
      get;
    } = new("BLOCK_ONLY_EXTREMELY_HIGH");

    public static IReadOnlyList<PhishBlockThreshold> AllValues {
      get;
    } = new[] { PhishBlockThresholdUnspecified,
                BlockLowAndAbove,
                BlockMediumAndAbove,
                BlockHighAndAbove,
                BlockHigherAndAbove,
                BlockVeryHighAndAbove,
                BlockOnlyExtremelyHigh };

    public static PhishBlockThreshold FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new PhishBlockThreshold("PHISH_BLOCK_THRESHOLD_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new PhishBlockThreshold(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator PhishBlockThreshold(string value) => FromString(value);

    public bool Equals(PhishBlockThreshold other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class PhishBlockThresholdConverter : JsonConverter<PhishBlockThreshold> {
    public override PhishBlockThreshold Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                             JsonSerializerOptions options) {
      var value = reader.GetString();
      return PhishBlockThreshold.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, PhishBlockThreshold value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
