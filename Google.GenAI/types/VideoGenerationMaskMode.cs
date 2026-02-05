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
  /// Enum for the mask mode of a video generation mask.
  /// </summary>

  [JsonConverter(typeof(VideoGenerationMaskModeConverter))]
  public readonly record struct VideoGenerationMaskMode : IEquatable<VideoGenerationMaskMode> {
    public string Value { get; }

    private VideoGenerationMaskMode(string value) {
      Value = value;
    }

    /// <summary>
    /// The image mask contains a masked rectangular region which is applied on the first frame of
    /// the input video. The object described in the prompt is inserted into this region and will
    /// appear in subsequent frames.
    /// </summary>
    public static VideoGenerationMaskMode Insert { get; } = new("INSERT");

    /// <summary>
    /// The image mask is used to determine an object in the first video frame to track. This object
    /// is removed from the video.
    /// </summary>
    public static VideoGenerationMaskMode Remove { get; } = new("REMOVE");

    /// <summary>
    /// The image mask is used to determine a region in the video. Objects in this region will be
    /// removed.
    /// </summary>
    public static VideoGenerationMaskMode RemoveStatic { get; } = new("REMOVE_STATIC");

    /// <summary>
    /// The image mask contains a masked rectangular region where the input video will go. The
    /// remaining area will be generated. Video masks are not supported.
    /// </summary>
    public static VideoGenerationMaskMode Outpaint { get; } = new("OUTPAINT");

    public static IReadOnlyList<VideoGenerationMaskMode> AllValues {
      get;
    } = new[] { Insert, Remove, RemoveStatic, Outpaint };

    public static VideoGenerationMaskMode FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new VideoGenerationMaskMode("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new VideoGenerationMaskMode(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator VideoGenerationMaskMode(string value) => FromString(value);

    public bool Equals(VideoGenerationMaskMode other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class VideoGenerationMaskModeConverter : JsonConverter<VideoGenerationMaskMode> {
    public override VideoGenerationMaskMode Read(ref Utf8JsonReader reader,
                                                 System.Type typeToConvert,
                                                 JsonSerializerOptions options) {
      var value = reader.GetString();
      return VideoGenerationMaskMode.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, VideoGenerationMaskMode value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
