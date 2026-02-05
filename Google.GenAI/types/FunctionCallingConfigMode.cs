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
  /// Function calling mode.
  /// </summary>

  [JsonConverter(typeof(FunctionCallingConfigModeConverter))]
  public readonly record struct FunctionCallingConfigMode : IEquatable<FunctionCallingConfigMode> {
    public string Value { get; }

    private FunctionCallingConfigMode(string value) {
      Value = value;
    }

    /// <summary>
    /// Unspecified function calling mode. This value should not be used.
    /// </summary>
    public static FunctionCallingConfigMode ModeUnspecified { get; } = new("MODE_UNSPECIFIED");

    /// <summary>
    /// Default model behavior, model decides to predict either function calls or natural language
    /// response.
    /// </summary>
    public static FunctionCallingConfigMode Auto { get; } = new("AUTO");

    /// <summary>
    /// Model is constrained to always predicting function calls only. If "allowed_function_names"
    /// are set, the predicted function calls will be limited to any one of
    /// "allowed_function_names", else the predicted function calls will be any one of the provided
    /// "function_declarations".
    /// </summary>
    public static FunctionCallingConfigMode Any { get; } = new("ANY");

    /// <summary>
    /// Model will not predict any function calls. Model behavior is same as when not passing any
    /// function declarations.
    /// </summary>
    public static FunctionCallingConfigMode None { get; } = new("NONE");

    /// <summary>
    /// Model is constrained to predict either function calls or natural language response. If
    /// "allowed_function_names" are set, the predicted function calls will be limited to any one of
    /// "allowed_function_names", else the predicted function calls will be any one of the provided
    /// "function_declarations".
    /// </summary>
    public static FunctionCallingConfigMode Validated { get; } = new("VALIDATED");

    public static IReadOnlyList<FunctionCallingConfigMode> AllValues {
      get;
    } = new[] { ModeUnspecified, Auto, Any, None, Validated };

    public static FunctionCallingConfigMode FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new FunctionCallingConfigMode("MODE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new FunctionCallingConfigMode(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator FunctionCallingConfigMode(string value) => FromString(value);

    public bool Equals(FunctionCallingConfigMode other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class FunctionCallingConfigModeConverter : JsonConverter<FunctionCallingConfigMode> {
    public override FunctionCallingConfigMode Read(ref Utf8JsonReader reader,
                                                   System.Type typeToConvert,
                                                   JsonSerializerOptions options) {
      var value = reader.GetString();
      return FunctionCallingConfigMode.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, FunctionCallingConfigMode value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
