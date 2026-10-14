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

namespace Google.GenAI
{
  /// <summary>
  /// Counts extra HTTP attempts on the current async flow. The caller holds the
  /// scope; <see cref="ApiClient"/> writes into that same object, so the count
  /// is visible after the send returns. No scope means nobody is listening.
  /// </summary>
  public sealed class HttpRetryScope : IDisposable
  {
    private static readonly AsyncLocal<HttpRetryScope?> Current = new AsyncLocal<HttpRetryScope?>();

    private readonly HttpRetryScope? _previous;

    /// <summary>Extra sends after the first. Zero when the first send succeeded.</summary>
    public int Retries { get; private set; }

    private HttpRetryScope(HttpRetryScope? previous)
    {
      _previous = previous;
    }

    public static HttpRetryScope Begin()
    {
      var scope = new HttpRetryScope(Current.Value);
      Current.Value = scope;
      return scope;
    }

    internal static void Record(int retries)
    {
      if (retries > 0 && Current.Value is HttpRetryScope scope)
      {
        scope.Retries = retries;
      }
    }

    public void Dispose()
    {
      if (Current.Value == this)
      {
        Current.Value = _previous;
      }
    }
  }
}
