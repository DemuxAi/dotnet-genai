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
  /// The aspect ratio for the image output.
  /// </summary>

  [JsonConverter(typeof(AspectRatioConverter))]
  public readonly record struct AspectRatio : IEquatable<AspectRatio> {
    public string Value { get; }

    private AspectRatio(string value) {
      Value = value;
    }

    /// <summary>
    /// Default value. This value is unused.
    /// </summary>
    public static AspectRatio AspectRatioUnspecified { get; } = new("ASPECT_RATIO_UNSPECIFIED");

    /// <summary>
    /// 1:1 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioOneByOne { get; } = new("ASPECT_RATIO_ONE_BY_ONE");

    /// <summary>
    /// 2:3 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioTwoByThree { get; } = new("ASPECT_RATIO_TWO_BY_THREE");

    /// <summary>
    /// 3:2 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioThreeByTwo { get; } = new("ASPECT_RATIO_THREE_BY_TWO");

    /// <summary>
    /// 3:4 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioThreeByFour { get; } = new("ASPECT_RATIO_THREE_BY_FOUR");

    /// <summary>
    /// 4:3 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioFourByThree { get; } = new("ASPECT_RATIO_FOUR_BY_THREE");

    /// <summary>
    /// 4:5 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioFourByFive { get; } = new("ASPECT_RATIO_FOUR_BY_FIVE");

    /// <summary>
    /// 5:4 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioFiveByFour { get; } = new("ASPECT_RATIO_FIVE_BY_FOUR");

    /// <summary>
    /// 9:16 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioNineBySixteen {
      get;
    } = new("ASPECT_RATIO_NINE_BY_SIXTEEN");

    /// <summary>
    /// 16:9 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioSixteenByNine {
      get;
    } = new("ASPECT_RATIO_SIXTEEN_BY_NINE");

    /// <summary>
    /// 21:9 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioTwentyOneByNine {
      get;
    } = new("ASPECT_RATIO_TWENTY_ONE_BY_NINE");

    /// <summary>
    /// 1:8 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioOneByEight { get; } = new("ASPECT_RATIO_ONE_BY_EIGHT");

    /// <summary>
    /// 8:1 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioEightByOne { get; } = new("ASPECT_RATIO_EIGHT_BY_ONE");

    /// <summary>
    /// 1:4 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioOneByFour { get; } = new("ASPECT_RATIO_ONE_BY_FOUR");

    /// <summary>
    /// 4:1 aspect ratio.
    /// </summary>
    public static AspectRatio AspectRatioFourByOne { get; } = new("ASPECT_RATIO_FOUR_BY_ONE");

    public static IReadOnlyList<AspectRatio> AllValues {
      get;
    } = new[] { AspectRatioUnspecified,   AspectRatioOneByOne,        AspectRatioTwoByThree,
                AspectRatioThreeByTwo,    AspectRatioThreeByFour,     AspectRatioFourByThree,
                AspectRatioFourByFive,    AspectRatioFiveByFour,      AspectRatioNineBySixteen,
                AspectRatioSixteenByNine, AspectRatioTwentyOneByNine, AspectRatioOneByEight,
                AspectRatioEightByOne,    AspectRatioOneByFour,       AspectRatioFourByOne };

    public static AspectRatio FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new AspectRatio("ASPECT_RATIO_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new AspectRatio(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator AspectRatio(string value) => FromString(value);

    public bool Equals(AspectRatio other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class AspectRatioConverter : JsonConverter<AspectRatio> {
    public override AspectRatio Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                     JsonSerializerOptions options) {
      var value = reader.GetString();
      return AspectRatio.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, AspectRatio value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
