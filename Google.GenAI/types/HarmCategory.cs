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
  /// Harm category.
  /// </summary>

  [JsonConverter(typeof(HarmCategoryConverter))]
  public readonly record struct HarmCategory : IEquatable<HarmCategory> {
    public string Value { get; }

    private HarmCategory(string value) {
      Value = value;
    }

    /// <summary>
    /// The harm category is unspecified.
    /// </summary>
    public static HarmCategory HarmCategoryUnspecified { get; } = new("HARM_CATEGORY_UNSPECIFIED");

    /// <summary>
    /// The harm category is harassment.
    /// </summary>
    public static HarmCategory HarmCategoryHarassment { get; } = new("HARM_CATEGORY_HARASSMENT");

    /// <summary>
    /// The harm category is hate speech.
    /// </summary>
    public static HarmCategory HarmCategoryHateSpeech { get; } = new("HARM_CATEGORY_HATE_SPEECH");

    /// <summary>
    /// The harm category is sexually explicit content.
    /// </summary>
    public static HarmCategory HarmCategorySexuallyExplicit {
      get;
    } = new("HARM_CATEGORY_SEXUALLY_EXPLICIT");

    /// <summary>
    /// The harm category is dangerous content.
    /// </summary>
    public static HarmCategory HarmCategoryDangerousContent {
      get;
    } = new("HARM_CATEGORY_DANGEROUS_CONTENT");

    /// <summary>
    /// Deprecated: Election filter is not longer supported. The harm category is civic integrity.
    /// </summary>
    public static HarmCategory HarmCategoryCivicIntegrity {
      get;
    } = new("HARM_CATEGORY_CIVIC_INTEGRITY");

    /// <summary>
    /// The harm category is image hate. This enum value is not supported in Gemini API.
    /// </summary>
    public static HarmCategory HarmCategoryImageHate { get; } = new("HARM_CATEGORY_IMAGE_HATE");

    /// <summary>
    /// The harm category is image dangerous content. This enum value is not supported in Gemini
    /// API.
    /// </summary>
    public static HarmCategory HarmCategoryImageDangerousContent {
      get;
    } = new("HARM_CATEGORY_IMAGE_DANGEROUS_CONTENT");

    /// <summary>
    /// The harm category is image harassment. This enum value is not supported in Gemini API.
    /// </summary>
    public static HarmCategory HarmCategoryImageHarassment {
      get;
    } = new("HARM_CATEGORY_IMAGE_HARASSMENT");

    /// <summary>
    /// The harm category is image sexually explicit content. This enum value is not supported in
    /// Gemini API.
    /// </summary>
    public static HarmCategory HarmCategoryImageSexuallyExplicit {
      get;
    } = new("HARM_CATEGORY_IMAGE_SEXUALLY_EXPLICIT");

    /// <summary>
    /// The harm category is for jailbreak prompts. This enum value is not supported in Gemini API.
    /// </summary>
    public static HarmCategory HarmCategoryJailbreak { get; } = new("HARM_CATEGORY_JAILBREAK");

    public static IReadOnlyList<HarmCategory> AllValues {
      get;
    } = new[] { HarmCategoryUnspecified,      HarmCategoryHarassment,
                HarmCategoryHateSpeech,       HarmCategorySexuallyExplicit,
                HarmCategoryDangerousContent, HarmCategoryCivicIntegrity,
                HarmCategoryImageHate,        HarmCategoryImageDangerousContent,
                HarmCategoryImageHarassment,  HarmCategoryImageSexuallyExplicit,
                HarmCategoryJailbreak };

    public static HarmCategory FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new HarmCategory("HARM_CATEGORY_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new HarmCategory(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator HarmCategory(string value) => FromString(value);

    public bool Equals(HarmCategory other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class HarmCategoryConverter : JsonConverter<HarmCategory> {
    public override HarmCategory Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                      JsonSerializerOptions options) {
      var value = reader.GetString();
      return HarmCategory.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, HarmCategory value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
