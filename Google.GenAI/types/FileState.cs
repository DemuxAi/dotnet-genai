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
  /// State for the lifecycle of a File.
  /// </summary>

  [JsonConverter(typeof(FileStateConverter))]
  public readonly record struct FileState : IEquatable<FileState> {
    public string Value { get; }

    private FileState(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static FileState StateUnspecified { get; } = new("STATE_UNSPECIFIED");

    /// <summary>
    ///
    /// </summary>
    public static FileState Processing { get; } = new("PROCESSING");

    /// <summary>
    ///
    /// </summary>
    public static FileState Active { get; } = new("ACTIVE");

    /// <summary>
    ///
    /// </summary>
    public static FileState Failed { get; } = new("FAILED");

    public static IReadOnlyList<FileState> AllValues {
      get;
    } = new[] { StateUnspecified, Processing, Active, Failed };

    public static FileState FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new FileState("STATE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new FileState(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator FileState(string value) => FromString(value);

    public bool Equals(FileState other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class FileStateConverter : JsonConverter<FileState> {
    public override FileState Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                   JsonSerializerOptions options) {
      var value = reader.GetString();
      return FileState.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, FileState value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
