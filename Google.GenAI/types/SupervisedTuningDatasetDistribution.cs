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

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.GenAI.Serialization;

namespace Google.GenAI.Types {
  /// <summary>
  /// Dataset distribution for Supervised Tuning. This data type is not supported in Gemini API.
  /// </summary>

  public record SupervisedTuningDatasetDistribution {
    /// <summary>
    /// Output only. Sum of a given population of values that are billable.
    /// </summary>
    [JsonPropertyName("billableSum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(StringToNullableLongConverter))]
    public long ? BillableSum { get; set; }

    /// <summary>
    /// Output only. Defines the histogram bucket.
    /// </summary>
    [JsonPropertyName("buckets")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SupervisedTuningDatasetDistributionDatasetBucket>
        ? Buckets {
            get; set;
          }

    /// <summary>
    /// Output only. The maximum of the population values.
    /// </summary>
    [JsonPropertyName("max")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? Max {
            get; set;
          }

    /// <summary>
    /// Output only. The arithmetic mean of the values in the population.
    /// </summary>
    [JsonPropertyName("mean")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? Mean {
            get; set;
          }

    /// <summary>
    /// Output only. The median of the values in the population.
    /// </summary>
    [JsonPropertyName("median")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? Median {
            get; set;
          }

    /// <summary>
    /// Output only. The minimum of the population values.
    /// </summary>
    [JsonPropertyName("min")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? Min {
            get; set;
          }

    /// <summary>
    /// Output only. The 5th percentile of the values in the population.
    /// </summary>
    [JsonPropertyName("p5")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? P5 {
            get; set;
          }

    /// <summary>
    /// Output only. The 95th percentile of the values in the population.
    /// </summary>
    [JsonPropertyName("p95")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double
        ? P95 {
            get; set;
          }

    /// <summary>
    /// Output only. Sum of a given population of values.
    /// </summary>
    [JsonPropertyName("sum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(StringToNullableLongConverter))]
    public long
        ? Sum {
            get; set;
          }

    /// <summary>
    /// Deserializes a JSON string to a SupervisedTuningDatasetDistribution object.
    /// </summary>
    /// <param name="jsonString">The JSON string to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions.</param>
    /// <returns>The deserialized SupervisedTuningDatasetDistribution object, or null if
    /// deserialization fails.</returns>
    public static SupervisedTuningDatasetDistribution
        ? FromJson(string jsonString, JsonSerializerOptions? options = null) {
      try {
        return JsonSerializer.Deserialize(
            jsonString, JsonConfig.TypeInfo<SupervisedTuningDatasetDistribution>(options));
      } catch (JsonException e) {
        Console.Error.WriteLine($"Error deserializing JSON: {e.ToString()}");
        return null;
      }
    }
  }
}
