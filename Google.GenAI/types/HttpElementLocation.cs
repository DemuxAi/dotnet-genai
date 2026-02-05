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
  /// The location of the API key. This enum is not supported in Gemini API.
  /// </summary>

  [JsonConverter(typeof(HttpElementLocationConverter))]
  public readonly record struct HttpElementLocation : IEquatable<HttpElementLocation> {
    public string Value { get; }

    private HttpElementLocation(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static HttpElementLocation HttpInUnspecified { get; } = new("HTTP_IN_UNSPECIFIED");

    /// <summary>
    /// Element is in the HTTP request query.
    /// </summary>
    public static HttpElementLocation HttpInQuery { get; } = new("HTTP_IN_QUERY");

    /// <summary>
    /// Element is in the HTTP request header.
    /// </summary>
    public static HttpElementLocation HttpInHeader { get; } = new("HTTP_IN_HEADER");

    /// <summary>
    /// Element is in the HTTP request path.
    /// </summary>
    public static HttpElementLocation HttpInPath { get; } = new("HTTP_IN_PATH");

    /// <summary>
    /// Element is in the HTTP request body.
    /// </summary>
    public static HttpElementLocation HttpInBody { get; } = new("HTTP_IN_BODY");

    /// <summary>
    /// Element is in the HTTP request cookie.
    /// </summary>
    public static HttpElementLocation HttpInCookie { get; } = new("HTTP_IN_COOKIE");

    public static IReadOnlyList<HttpElementLocation> AllValues {
      get;
    } = new[] { HttpInUnspecified, HttpInQuery, HttpInHeader,
                HttpInPath,        HttpInBody,  HttpInCookie };

    public static HttpElementLocation FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new HttpElementLocation("HTTP_IN_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new HttpElementLocation(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator HttpElementLocation(string value) => FromString(value);

    public bool Equals(HttpElementLocation other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class HttpElementLocationConverter : JsonConverter<HttpElementLocation> {
    public override HttpElementLocation Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                             JsonSerializerOptions options) {
      var value = reader.GetString();
      return HttpElementLocation.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, HttpElementLocation value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
