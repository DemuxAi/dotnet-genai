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
  /// Enum that specifies the language of the text in the prompt.
  /// </summary>

  [JsonConverter(typeof(ImagePromptLanguageConverter))]
  public readonly record struct ImagePromptLanguage : IEquatable<ImagePromptLanguage> {
    public string Value { get; }

    private ImagePromptLanguage(string value) {
      Value = value;
    }

    /// <summary>
    /// Auto-detect the language.
    /// </summary>
    public static ImagePromptLanguage Auto { get; } = new("auto");

    /// <summary>
    /// English
    /// </summary>
    public static ImagePromptLanguage En { get; } = new("en");

    /// <summary>
    /// Japanese
    /// </summary>
    public static ImagePromptLanguage Ja { get; } = new("ja");

    /// <summary>
    /// Korean
    /// </summary>
    public static ImagePromptLanguage Ko { get; } = new("ko");

    /// <summary>
    /// Hindi
    /// </summary>
    public static ImagePromptLanguage Hi { get; } = new("hi");

    /// <summary>
    /// Chinese
    /// </summary>
    public static ImagePromptLanguage Zh { get; } = new("zh");

    /// <summary>
    /// Portuguese
    /// </summary>
    public static ImagePromptLanguage Pt { get; } = new("pt");

    /// <summary>
    /// Spanish
    /// </summary>
    public static ImagePromptLanguage Es { get; } = new("es");

    public static IReadOnlyList<ImagePromptLanguage> AllValues {
      get;
    } = new[] { Auto, En, Ja, Ko, Hi, Zh, Pt, Es };

    public static ImagePromptLanguage FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new ImagePromptLanguage("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new ImagePromptLanguage(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator ImagePromptLanguage(string value) => FromString(value);

    public bool Equals(ImagePromptLanguage other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class ImagePromptLanguageConverter : JsonConverter<ImagePromptLanguage> {
    public override ImagePromptLanguage Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                             JsonSerializerOptions options) {
      var value = reader.GetString();
      return ImagePromptLanguage.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, ImagePromptLanguage value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
