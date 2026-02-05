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
  /// Enum representing the editing mode.
  /// </summary>

  [JsonConverter(typeof(EditModeConverter))]
  public readonly record struct EditMode : IEquatable<EditMode> {
    public string Value { get; }

    private EditMode(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static EditMode EditModeDefault { get; } = new("EDIT_MODE_DEFAULT");

    /// <summary>
    ///
    /// </summary>
    public static EditMode EditModeInpaintRemoval { get; } = new("EDIT_MODE_INPAINT_REMOVAL");

    /// <summary>
    ///
    /// </summary>
    public static EditMode EditModeInpaintInsertion { get; } = new("EDIT_MODE_INPAINT_INSERTION");

    /// <summary>
    ///
    /// </summary>
    public static EditMode EditModeOutpaint { get; } = new("EDIT_MODE_OUTPAINT");

    /// <summary>
    ///
    /// </summary>
    public static EditMode EditModeControlledEditing { get; } = new("EDIT_MODE_CONTROLLED_EDITING");

    /// <summary>
    ///
    /// </summary>
    public static EditMode EditModeStyle { get; } = new("EDIT_MODE_STYLE");

    /// <summary>
    ///
    /// </summary>
    public static EditMode EditModeBgswap { get; } = new("EDIT_MODE_BGSWAP");

    /// <summary>
    ///
    /// </summary>
    public static EditMode EditModeProductImage { get; } = new("EDIT_MODE_PRODUCT_IMAGE");

    public static IReadOnlyList<EditMode> AllValues {
      get;
    } = new[] { EditModeDefault,  EditModeInpaintRemoval,    EditModeInpaintInsertion,
                EditModeOutpaint, EditModeControlledEditing, EditModeStyle,
                EditModeBgswap,   EditModeProductImage };

    public static EditMode FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new EditMode("UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new EditMode(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator EditMode(string value) => FromString(value);

    public bool Equals(EditMode other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class EditModeConverter : JsonConverter<EditMode> {
    public override EditMode Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                  JsonSerializerOptions options) {
      var value = reader.GetString();
      return EditMode.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, EditMode value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
