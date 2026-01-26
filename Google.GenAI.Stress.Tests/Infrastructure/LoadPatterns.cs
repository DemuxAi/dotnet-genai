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

using NBomber.Contracts;
using NBomber.CSharp;
using Google.GenAI.StressTests.Configuration;

namespace Google.GenAI.StressTests.Infrastructure;

/// <summary>
/// Defines load patterns for stress testing: Light, Medium, Heavy
/// </summary>
public static class LoadPatterns
{
    /// <summary>
    /// Light load: 10-50 concurrent users over 7 minutes (2 min ramp-up + 5 min sustained)
    /// Use case: Quick validation, initial leak detection
    /// Estimated requests: ~21,000 (~$0.10 quota cost)
    /// </summary>
    public static LoadSimulation Light
    {
        get
        {
            var config = StressTestConfig.Instance.LoadPatterns.Light;
            return CreateRampingConstantSimulation(
                config.MaxConcurrent,
                config.RampUpMinutes,
                config.SustainMinutes);
        }
    }

    /// <summary>
    /// Medium load: 100-500 concurrent users over 15 minutes (5 min ramp-up + 10 min sustained)
    /// Use case: Moderate sustained load testing
    /// Estimated requests: ~450,000 (~$2.25 quota cost)
    /// </summary>
    public static LoadSimulation Medium
    {
        get
        {
            var config = StressTestConfig.Instance.LoadPatterns.Medium;
            return CreateRampingConstantSimulation(
                config.MaxConcurrent,
                config.RampUpMinutes,
                config.SustainMinutes);
        }
    }

    /// <summary>
    /// Heavy load: 1000-1500 concurrent users over 25 minutes (10 min ramp-up + 15 min sustained)
    /// Use case: Extreme stress, reveals hidden leaks
    /// Estimated requests: ~2,250,000 (~$11.25 quota cost)
    /// </summary>
    public static LoadSimulation Heavy
    {
        get
        {
            var config = StressTestConfig.Instance.LoadPatterns.Heavy;
            return CreateRampingConstantSimulation(
                config.MaxConcurrent,
                config.RampUpMinutes,
                config.SustainMinutes);
        }
    }

    /// <summary>
    /// Creates a ramping constant simulation: ramp up to max, then sustain
    /// </summary>
    private static LoadSimulation CreateRampingConstantSimulation(
        int maxConcurrent,
        double rampUpMinutes,
        double sustainMinutes)
    {
        return Simulation.RampingConstant(
            copies: maxConcurrent,
            during: TimeSpan.FromMinutes(rampUpMinutes + sustainMinutes));
    }

    /// <summary>
    /// Get load pattern by name
    /// </summary>
    public static LoadSimulation GetByName(string name)
    {
        return name.ToLower() switch
        {
            "light" => Light,
            "medium" => Medium,
            "heavy" => Heavy,
            _ => throw new ArgumentException($"Unknown load pattern: {name}")
        };
    }

    /// <summary>
    /// Creates a custom load pattern
    /// </summary>
    public static LoadSimulation CreateCustom(int maxConcurrent, double durationMinutes)
    {
        return Simulation.RampingConstant(
            copies: maxConcurrent,
            during: TimeSpan.FromMinutes(durationMinutes));
    }

    /// <summary>
    /// Creates a keep-constant simulation (no ramp-up, immediate max load)
    /// Useful for testing specific scenarios
    /// </summary>
    public static LoadSimulation CreateConstant(int concurrent, double durationMinutes)
    {
        return Simulation.KeepConstant(
            copies: concurrent,
            during: TimeSpan.FromMinutes(durationMinutes));
    }

    /// <summary>
    /// Creates an inject-per-second simulation (rate-based instead of user-based)
    /// </summary>
    public static LoadSimulation CreateInjectPerSecond(int rate, double durationMinutes)
    {
        return Simulation.Inject(
            rate: rate,
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromMinutes(durationMinutes));
    }
}
