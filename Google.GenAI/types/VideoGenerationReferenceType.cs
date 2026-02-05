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
  /// Enum for the reference type of a video generation reference image.
  /// </summary>

  [JsonConverter(typeof(VideoGenerationReferenceTypeConverter))]
  public readonly record struct VideoGenerationReferenceType
      : IEquatable<VideoGenerationReferenceType> {
    public string Value { get; }

    private VideoGenerationReferenceType(string value) {
      Value = value;
    }

    /// <summary>
    /// A reference image that provides assets to the generated video, such as the scene, an object,
    /// a character, etc.
    /// </summary>
    public static VideoGenerationReferenceType Asset { get; } = new("ASSET");

    /// <summary>
    /// A reference image that provides aesthetics including colors, lighting, texture, etc., to be
    /// used as the style of the generated video, such as 'anime', 'photography', 'origami', etc.
    /// </summary>
    public static VideoGenerationReferenceType Style { get; } = new("STYLE");

    public static IReadOnlyList<VideoGenerationReferenceType> AllValues {
      get;
    } = new[] { Asset, Style };

    public static VideoGenerationReferenceType FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new VideoGenerationReferenceType("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new VideoGenerationReferenceType(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator VideoGenerationReferenceType(string value) => FromString(value);

    public bool Equals(VideoGenerationReferenceType other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class VideoGenerationReferenceTypeConverter : JsonConverter<VideoGenerationReferenceType> {
    public override VideoGenerationReferenceType Read(ref Utf8JsonReader reader,
                                                      System.Type typeToConvert,
                                                      JsonSerializerOptions options) {
      var value = reader.GetString();
      return VideoGenerationReferenceType.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, VideoGenerationReferenceType value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
