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
  /// Type of auth scheme. This enum is not supported in Gemini API.
  /// </summary>

  [JsonConverter(typeof(AuthTypeConverter))]
  public readonly record struct AuthType : IEquatable<AuthType> {
    public string Value { get; }

    private AuthType(string value) {
      Value = value;
    }

    /// <summary>
    ///
    /// </summary>
    public static AuthType AuthTypeUnspecified { get; } = new("AUTH_TYPE_UNSPECIFIED");

    /// <summary>
    /// No Auth.
    /// </summary>
    public static AuthType NoAuth { get; } = new("NO_AUTH");

    /// <summary>
    /// API Key Auth.
    /// </summary>
    public static AuthType ApiKeyAuth { get; } = new("API_KEY_AUTH");

    /// <summary>
    /// HTTP Basic Auth.
    /// </summary>
    public static AuthType HttpBasicAuth { get; } = new("HTTP_BASIC_AUTH");

    /// <summary>
    /// Google Service Account Auth.
    /// </summary>
    public static AuthType GoogleServiceAccountAuth { get; } = new("GOOGLE_SERVICE_ACCOUNT_AUTH");

    /// <summary>
    /// OAuth auth.
    /// </summary>
    public static AuthType Oauth { get; } = new("OAUTH");

    /// <summary>
    /// OpenID Connect (OIDC) Auth.
    /// </summary>
    public static AuthType OidcAuth { get; } = new("OIDC_AUTH");

    public static IReadOnlyList<AuthType> AllValues {
      get;
    } = new[] { AuthTypeUnspecified,      NoAuth, ApiKeyAuth, HttpBasicAuth,
                GoogleServiceAccountAuth, Oauth,  OidcAuth };

    public static AuthType FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new AuthType("AUTH_TYPE_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new AuthType(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator AuthType(string value) => FromString(value);

    public bool Equals(AuthType other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class AuthTypeConverter : JsonConverter<AuthType> {
    public override AuthType Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                  JsonSerializerOptions options) {
      var value = reader.GetString();
      return AuthType.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, AuthType value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
