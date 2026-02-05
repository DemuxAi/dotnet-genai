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
  /// Options for feature selection preference.
  /// </summary>

  [JsonConverter(typeof(FeatureSelectionPreferenceConverter))]
  public readonly record struct FeatureSelectionPreference
      : IEquatable<FeatureSelectionPreference> {
    public string Value { get; }

    private FeatureSelectionPreference(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static FeatureSelectionPreference FeatureSelectionPreferenceUnspecified {
      get;
    } = new("FEATURE_SELECTION_PREFERENCE_UNSPECIFIED");

    /// <summary>
    ///
    /// </summary>
    public static FeatureSelectionPreference PrioritizeQuality { get; } = new("PRIORITIZE_QUALITY");

    /// <summary>
    ///
    /// </summary>
    public static FeatureSelectionPreference Balanced { get; } = new("BALANCED");

    /// <summary>
    ///
    /// </summary>
    public static FeatureSelectionPreference PrioritizeCost { get; } = new("PRIORITIZE_COST");

    public static IReadOnlyList<FeatureSelectionPreference> AllValues {
      get;
    } = new[] { FeatureSelectionPreferenceUnspecified, PrioritizeQuality, Balanced,
                PrioritizeCost };

    public static FeatureSelectionPreference FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new FeatureSelectionPreference("FEATURE_SELECTION_PREFERENCE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new FeatureSelectionPreference(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator FeatureSelectionPreference(string value) => FromString(value);

    public bool Equals(FeatureSelectionPreference other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class FeatureSelectionPreferenceConverter : JsonConverter<FeatureSelectionPreference> {
    public override FeatureSelectionPreference Read(ref Utf8JsonReader reader,
                                                    System.Type typeToConvert,
                                                    JsonSerializerOptions options) {
      var value = reader.GetString();
      return FeatureSelectionPreference.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, FeatureSelectionPreference value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
