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
  /// The size of the image output.
  /// </summary>

  [JsonConverter(typeof(ImageSizeConverter))]
  public readonly record struct ImageSize : IEquatable<ImageSize> {
    public string Value { get; }

    private ImageSize(string value) {
      Value = value;
    }

    /// <summary>
    /// Default value. This value is unused.
    /// </summary>
    public static ImageSize ImageSizeUnspecified { get; } = new("IMAGE_SIZE_UNSPECIFIED");

    /// <summary>
    /// 512px image size.
    /// </summary>
    public static ImageSize ImageSizeFiveTwelve { get; } = new("IMAGE_SIZE_FIVE_TWELVE");

    /// <summary>
    /// 1K image size.
    /// </summary>
    public static ImageSize ImageSizeOneK { get; } = new("IMAGE_SIZE_ONE_K");

    /// <summary>
    /// 2K image size.
    /// </summary>
    public static ImageSize ImageSizeTwoK { get; } = new("IMAGE_SIZE_TWO_K");

    /// <summary>
    /// 4K image size.
    /// </summary>
    public static ImageSize ImageSizeFourK { get; } = new("IMAGE_SIZE_FOUR_K");

    public static IReadOnlyList<ImageSize> AllValues {
      get;
    } = new[] { ImageSizeUnspecified, ImageSizeFiveTwelve, ImageSizeOneK, ImageSizeTwoK,
                ImageSizeFourK };

    public static ImageSize FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new ImageSize("IMAGE_SIZE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new ImageSize(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator ImageSize(string value) => FromString(value);

    public bool Equals(ImageSize other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class ImageSizeConverter : JsonConverter<ImageSize> {
    public override ImageSize Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                   JsonSerializerOptions options) {
      var value = reader.GetString();
      return ImageSize.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, ImageSize value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
