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
  /// Source of the File.
  /// </summary>

  [JsonConverter(typeof(FileSourceConverter))]
  public readonly record struct FileSource : IEquatable<FileSource> {
    public string Value { get; }

    private FileSource(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static FileSource SourceUnspecified { get; } = new("SOURCE_UNSPECIFIED");

    /// <summary>
    ///
    /// </summary>
    public static FileSource Uploaded { get; } = new("UPLOADED");

    /// <summary>
    ///
    /// </summary>
    public static FileSource Generated { get; } = new("GENERATED");

    /// <summary>
    ///
    /// </summary>
    public static FileSource Registered { get; } = new("REGISTERED");

    public static IReadOnlyList<FileSource> AllValues {
      get;
    } = new[] { SourceUnspecified, Uploaded, Generated, Registered };

    public static FileSource FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new FileSource("SOURCE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new FileSource(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator FileSource(string value) => FromString(value);

    public bool Equals(FileSource other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class FileSourceConverter : JsonConverter<FileSource> {
    public override FileSource Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                    JsonSerializerOptions options) {
      var value = reader.GetString();
      return FileSource.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, FileSource value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
