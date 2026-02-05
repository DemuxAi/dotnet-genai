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
  /// The API spec that the external API implements. This enum is not supported in Gemini API.
  /// </summary>

  [JsonConverter(typeof(ApiSpecConverter))]
  public readonly record struct ApiSpec : IEquatable<ApiSpec> {
    public string Value { get; }

    private ApiSpec(string value) {
      Value = value;
    }

    /// <summary>
    /// Unspecified API spec. This value should not be used.
    /// </summary>
    public static ApiSpec ApiSpecUnspecified { get; } = new("API_SPEC_UNSPECIFIED");

    /// <summary>
    /// Simple search API spec.
    /// </summary>
    public static ApiSpec SimpleSearch { get; } = new("SIMPLE_SEARCH");

    /// <summary>
    /// Elastic search API spec.
    /// </summary>
    public static ApiSpec ElasticSearch { get; } = new("ELASTIC_SEARCH");

    public static IReadOnlyList<ApiSpec> AllValues {
      get;
    } = new[] { ApiSpecUnspecified, SimpleSearch, ElasticSearch };

    public static ApiSpec FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new ApiSpec("API_SPEC_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new ApiSpec(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator ApiSpec(string value) => FromString(value);

    public bool Equals(ApiSpec other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class ApiSpecConverter : JsonConverter<ApiSpec> {
    public override ApiSpec Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                 JsonSerializerOptions options) {
      var value = reader.GetString();
      return ApiSpec.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, ApiSpec value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
