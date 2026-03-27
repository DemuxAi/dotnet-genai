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
  /// Pricing and performance service tier.
  /// </summary>

  [JsonConverter(typeof(ServiceTierConverter))]
  public readonly record struct ServiceTier : IEquatable<ServiceTier> {
    public string Value { get; }

    private ServiceTier(string value) {
      Value = value;
    }

    /// <summary>
    /// Default service tier, which is standard.
    /// </summary>
    public static ServiceTier ServiceTierUnspecified { get; } = new("SERVICE_TIER_UNSPECIFIED");

    /// <summary>
    /// Flex service tier.
    /// </summary>
    public static ServiceTier ServiceTierFlex { get; } = new("SERVICE_TIER_FLEX");

    /// <summary>
    /// Standard service tier.
    /// </summary>
    public static ServiceTier ServiceTierStandard { get; } = new("SERVICE_TIER_STANDARD");

    /// <summary>
    /// Priority service tier.
    /// </summary>
    public static ServiceTier ServiceTierPriority { get; } = new("SERVICE_TIER_PRIORITY");

    public static IReadOnlyList<ServiceTier> AllValues {
      get;
    } = new[] { ServiceTierUnspecified, ServiceTierFlex, ServiceTierStandard, ServiceTierPriority };

    public static ServiceTier FromString(string value) {
      if (string.IsNullOrEmpty(value)) {
        return new ServiceTier("SERVICE_TIER_UNSPECIFIED");
      }

      foreach (var known in AllValues) {
        if (known.Value == value) {
          return known;
        }
      }

      return new ServiceTier(value);
    }

    public override string ToString() => Value ?? string.Empty;

    public static implicit operator ServiceTier(string value) => FromString(value);

    public bool Equals(ServiceTier other) => Value == other.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
  }

  public class ServiceTierConverter : JsonConverter<ServiceTier> {
    public override ServiceTier Read(ref Utf8JsonReader reader, System.Type typeToConvert,
                                     JsonSerializerOptions options) {
      var value = reader.GetString();
      return ServiceTier.FromString(value);
    }

    public override void Write(Utf8JsonWriter writer, ServiceTier value,
                               JsonSerializerOptions options) {
      writer.WriteStringValue(value.Value);
    }
  }
}
