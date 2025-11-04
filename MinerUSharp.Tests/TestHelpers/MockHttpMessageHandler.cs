using System.Net;

namespace MinerUSharp.Tests.TestHelpers
{
    /// <summary>
    /// Mock HTTP message handler for testing HTTP requests without making actual network calls.
    /// </summary>
    public sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? _handler;

    /// <summary>
    /// Sets up the handler to return a specific response.
    /// </summary>
    /// <param name="handler">The handler function.</param>
    public void Setup(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Sets up the handler to return a simple response with status code and content.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="content">The response content.</param>
    /// <param name="contentType">The content type.</param>
    public void SetupResponse(HttpStatusCode statusCode, string content, string contentType = "application/json")
    {
        _handler = (request, cancellationToken) =>
        {
            HttpResponseMessage response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, contentType),
            };
            return Task.FromResult(response);
        };
    }

    /// <summary>
    /// Sets up the handler to return a stream response.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="stream">The response stream.</param>
    /// <param name="contentType">The content type.</param>
    public void SetupStreamResponse(HttpStatusCode statusCode, Stream stream, string contentType = "application/json")
    {
        _handler = (request, cancellationToken) =>
        {
            HttpResponseMessage response = new HttpResponseMessage(statusCode)
            {
                Content = new StreamContent(stream),
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            return Task.FromResult(response);
        };
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_handler == null)
            throw new InvalidOperationException("Mock handler has not been set up. Call Setup() first.");

        return _handler(request, cancellationToken);
    }
}
}
