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

using System.Buffers;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Google.GenAI.StressTests.Configuration;

namespace Google.GenAI.StressTests.MockServer;

/// <summary>
/// High-performance ASP.NET Core mock server for stress testing.
/// Replays recorded API responses without making real API calls.
/// Supports HTTP endpoints (GenerateContent) and WebSocket (Live API).
/// </summary>
public class StressMockServer : IDisposable
{
    private WebApplication? _app;
    private readonly MockResponseProvider _responseProvider;
    private readonly MockServerConfig _config;
    private bool _disposed;

    // Pre-encoded WebSocket responses to avoid repeated encoding
    private static readonly byte[] SetupCompleteBytes = Encoding.UTF8.GetBytes("""{"setupComplete":{}}""");
    private static readonly byte[] ContentResponseBytes = Encoding.UTF8.GetBytes("""{"serverContent":{"modelTurn":{"parts":[{"text":"Mock response from stress test server."}]}}}""");
    private static readonly byte[] TurnCompleteBytes = Encoding.UTF8.GetBytes("""{"serverContent":{"turnComplete":true}}""");

    /// <summary>
    /// The base URL of the running mock server (e.g., "http://127.0.0.1:12345")
    /// </summary>
    public string? BaseUrl { get; private set; }

    /// <summary>
    /// The port the mock server is listening on.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Create a new mock server with the specified configuration.
    /// </summary>
    public StressMockServer(MockServerConfig config)
    {
        _config = config;
        _responseProvider = new MockResponseProvider();
    }

    /// <summary>
    /// Start the mock server asynchronously.
    /// </summary>
    public async Task StartAsync()
    {
        _responseProvider.LoadRecordings(_config.ResolvedRecordingsDirectory);

        var builder = WebApplication.CreateBuilder();

        // Use dynamic port allocation
        builder.WebHost.UseUrls("http://127.0.0.1:0");

        // Minimize logging noise during stress tests
        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        _app = builder.Build();

        // Enable WebSocket support for Live API
        _app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(30)
        });

        // Map WebSocket endpoints for Live API (must be before other routes)
        // Gemini Live API path
        _app.Map("/ws/google.ai.generativelanguage.v1beta.GenerativeService.BidiGenerateContent",
            app => app.Run(HandleWebSocketConnection));
        // Vertex Live API path
        _app.Map("/ws/google.cloud.aiplatform.v1beta1.LlmBidiService/BidiGenerateContent",
            app => app.Run(HandleWebSocketConnection));

        // Note: Debug logging removed to prevent memory growth during stress tests
        // Uncomment for debugging:
        // _app.Use(async (context, next) =>
        // {
        //     Console.WriteLine($"[MockServer] Request: {context.Request.Method} {context.Request.Path}");
        //     await next();
        // });

        // Map Gemini API HTTP endpoints
        // Cast to Delegate to ensure IResult return values are properly handled (fixes ASP0016 warning)
        _app.MapPost("/v1beta/models/{model}:generateContent", (Delegate)HandleGenerateContent);
        _app.MapPost("/v1beta/models/{model}:streamGenerateContent", (Delegate)HandleStreamGenerateContent);

        // Map Vertex AI HTTP endpoints
        _app.MapPost("/v1beta1/projects/{project}/locations/{location}/publishers/google/models/{model}:generateContent",
            (Delegate)HandleGenerateContent);
        _app.MapPost("/v1beta1/projects/{project}/locations/{location}/publishers/google/models/{model}:streamGenerateContent",
            (Delegate)HandleStreamGenerateContent);

        // Health check endpoint
        _app.MapGet("/health", () => Results.Ok("healthy"));

        await _app.StartAsync();

        // Get the actual bound address
        BaseUrl = _app.Urls.FirstOrDefault();
        if (BaseUrl != null)
        {
            Port = new Uri(BaseUrl).Port;
        }
        Console.WriteLine($"[StressMockServer] Started at: {BaseUrl} (HTTP + WebSocket)");
    }

    /// <summary>
    /// Handle WebSocket connections for Live API (simple echo mock).
    /// This provides enough functionality to test WebSocket lifecycle and disposal.
    /// Optimized for stress testing: uses pooled buffers and pre-encoded responses.
    /// </summary>
    private async Task HandleWebSocketConnection(HttpContext context)
    {
        // Note: Connection logging removed to prevent memory growth during stress tests
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("WebSocket request expected");
            return;
        }

        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        try
        {
            // Send SetupComplete response using pre-encoded bytes
            await SendBytesAsync(webSocket, SetupCompleteBytes, context.RequestAborted);

            // Echo loop: receive messages and send simple responses
            while (webSocket.State == WebSocketState.Open && !context.RequestAborted.IsCancellationRequested)
            {
                var (message, isRealtimeOrClient) = await ReceiveAndCheckMessageAsync(webSocket, context.RequestAborted);
                if (message == null)
                {
                    break; // Connection closed
                }

                // Check if this is a realtime input message
                if (isRealtimeOrClient)
                {
                    if (_config.SimulateLatency)
                    {
                        await Task.Delay(_config.LatencyMs / 2, context.RequestAborted);
                    }

                    // Send a simple text response using pre-encoded bytes
                    await SendBytesAsync(webSocket, ContentResponseBytes, context.RequestAborted);

                    if (_config.SimulateLatency)
                    {
                        await Task.Delay(_config.LatencyMs / 2, context.RequestAborted);
                    }

                    // Send turn complete using pre-encoded bytes
                    await SendBytesAsync(webSocket, TurnCompleteBytes, context.RequestAborted);
                }
            }
        }
        catch (WebSocketException)
        {
            // Client disconnected - this is expected during stress testing
        }
        catch (OperationCanceledException)
        {
            // Request cancelled - expected during shutdown
        }
        finally
        {
            // Ensure proper WebSocket close with timeout to avoid hangs
            if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
            {
                try
                {
                    using var closeTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Mock server closing",
                        closeTimeout.Token);
                }
                catch
                {
                    // Ignore close errors (including timeout)
                }
            }
        }
    }

    /// <summary>
    /// Receive a message from WebSocket and check if it's a realtime_input or clientContent message.
    /// Optimized for stress testing: uses pooled buffers.
    /// Returns (message, isRealtimeOrClient) tuple.
    /// </summary>
    private static async Task<(string? message, bool isRealtimeOrClient)> ReceiveAndCheckMessageAsync(
        WebSocket webSocket, CancellationToken cancellationToken)
    {
        // Use ArrayPool to avoid allocations
        var buffer = ArrayPool<byte>.Shared.Rent(4096);
        try
        {
            WebSocketReceiveResult result;
            int totalBytes = 0;

            do
            {
                result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer, totalBytes, buffer.Length - totalBytes),
                    cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    return (null, false);
                }

                totalBytes += result.Count;

                // Grow buffer if needed (rare case for large messages)
                if (!result.EndOfMessage && totalBytes >= buffer.Length - 1024)
                {
                    var newBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length * 2);
                    Buffer.BlockCopy(buffer, 0, newBuffer, 0, totalBytes);
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = newBuffer;
                }
            }
            while (!result.EndOfMessage);

            if (result.MessageType != WebSocketMessageType.Text || totalBytes == 0)
            {
                return (null, false);
            }

            // Decode message and check for keywords
            var message = Encoding.UTF8.GetString(buffer, 0, totalBytes);
            bool isRealtimeOrClient = message.Contains("realtime_input") || message.Contains("clientContent");

            return (message, isRealtimeOrClient);
        }
        catch
        {
            return (null, false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Send pre-encoded bytes via WebSocket (zero-allocation for responses).
    /// </summary>
    private static async Task SendBytesAsync(WebSocket webSocket, byte[] bytes, CancellationToken cancellationToken)
    {
        await webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken);
    }

    /// <summary>
    /// Handle non-streaming GenerateContent requests.
    /// </summary>
    private async Task<IResult> HandleGenerateContent(HttpContext context)
    {
        if (_config.SimulateLatency)
        {
            await Task.Delay(_config.LatencyMs);
        }

        var response = _responseProvider.GetResponse("generateContent");
        if (response == null)
        {
            return Results.Problem("No mock response configured for generateContent");
        }

        return Results.Content(response, "application/json");
    }

    /// <summary>
    /// Handle streaming GenerateContent requests using Server-Sent Events (SSE).
    /// </summary>
    private async Task HandleStreamGenerateContent(HttpContext context)
    {
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Connection = "keep-alive";

        var chunks = _responseProvider.GetStreamingResponse("streamGenerateContent");
        if (chunks == null || chunks.Length == 0)
        {
            await context.Response.WriteAsync("data: {\"error\": \"No mock streaming response configured\"}\n\n");
            return;
        }

        var delayPerChunk = _config.SimulateLatency ? _config.LatencyMs / chunks.Length : 0;

        foreach (var chunk in chunks)
        {
            if (context.RequestAborted.IsCancellationRequested)
            {
                break;
            }

            await context.Response.WriteAsync($"data: {chunk}\n\n");
            await context.Response.Body.FlushAsync();

            if (delayPerChunk > 0)
            {
                await Task.Delay(delayPerChunk);
            }
        }
    }

    /// <summary>
    /// Stop the mock server and release resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_app != null)
        {
            Console.WriteLine("[StressMockServer] Stopping...");
            try
            {
                _app.StopAsync().Wait(TimeSpan.FromSeconds(5));
                (_app as IDisposable)?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StressMockServer] Error during shutdown: {ex.Message}");
            }
            _app = null;
        }

        GC.SuppressFinalize(this);
    }
}
