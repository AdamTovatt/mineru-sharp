using System.Text.Json;

namespace MinerUSharp.Models
{
    /// <summary>
    /// Represents a response from the MinerU API.
    /// </summary>
    public sealed class MineruResponse : IDisposable, IAsyncDisposable
    {
        private readonly Stream _contentStream;
        private readonly HttpResponseMessage _httpResponse;
        private bool _disposed;
        private MineruResponseBody? _parsedBody;

        /// <summary>
        /// Gets the HTTP status code of the response.
        /// </summary>
        public System.Net.HttpStatusCode StatusCode => _httpResponse.StatusCode;

        /// <summary>
        /// Gets the content type of the response.
        /// </summary>
        public string? ContentType => _httpResponse.Content.Headers.ContentType?.MediaType;

        /// <summary>
        /// Gets a value indicating whether the response is a ZIP file.
        /// </summary>
        public bool IsZipResponse => ContentType?.Contains(MineruDefaults.ZipMediaTypeSubstring, StringComparison.OrdinalIgnoreCase) ?? false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MineruResponse"/> class.
        /// </summary>
        /// <param name="httpResponse">The HTTP response message.</param>
        /// <param name="contentStream">The content stream.</param>
        internal MineruResponse(HttpResponseMessage httpResponse, Stream contentStream)
        {
            _httpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
            _contentStream = contentStream ?? throw new ArgumentNullException(nameof(contentStream));
        }

        /// <summary>
        /// Reads the response content and parses it as a <see cref="MineruResponseBody"/>.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The parsed response body.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the response has been disposed.</exception>
        /// <exception cref="JsonException">Thrown if the content is not valid JSON.</exception>
        public async Task<MineruResponseBody> ReadAsResponseBodyAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (_parsedBody != null)
                return _parsedBody;

            _contentStream.Position = 0;
            _parsedBody = await JsonSerializer.DeserializeAsync<MineruResponseBody>(_contentStream, cancellationToken: cancellationToken);

            if (_parsedBody == null)
                throw new JsonException("Failed to deserialize response body.");

            return _parsedBody;
        }

        /// <summary>
        /// Reads the response content as a markdown string.
        /// For JSON responses, this extracts the markdown content from the first file in the results.
        /// For ZIP responses, this method should not be used; use GetContentStream() or SaveToFileAsync() instead.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The markdown content.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the response has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the response does not contain any results or markdown content.</exception>
        public async Task<string> ReadAsMarkdownAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            // If it's a ZIP response, the caller should use GetContentStream() or SaveToFileAsync()
            if (IsZipResponse)
                throw new InvalidOperationException("Cannot read ZIP responses as markdown. Use GetContentStream() or SaveToFileAsync() instead.");

            MineruResponseBody body = await ReadAsResponseBodyAsync(cancellationToken);

            if (body.Results.Count == 0)
                throw new InvalidOperationException("Response does not contain any results.");

            // Get the first result (usually "file0")
            MineruFileResult firstResult = body.Results.Values.First();

            if (string.IsNullOrEmpty(firstResult.MarkdownContent))
                throw new InvalidOperationException("Response does not contain markdown content.");

            return firstResult.MarkdownContent;
        }

        /// <summary>
        /// Reads the response content as a JSON element.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The JSON content as a <see cref="JsonElement"/>.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the response has been disposed.</exception>
        /// <exception cref="JsonException">Thrown if the content is not valid JSON.</exception>
        public async Task<JsonElement> ReadAsJsonAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            JsonDocument document = await JsonDocument.ParseAsync(_contentStream, default, cancellationToken);
            return document.RootElement.Clone();
        }

        /// <summary>
        /// Reads the response content as a byte array.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The content as a byte array.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the response has been disposed.</exception>
        public async Task<byte[]> ReadAsBytesAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await _contentStream.CopyToAsync(memoryStream, cancellationToken);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Gets the content stream directly for custom processing.
        /// </summary>
        /// <returns>The content stream.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the response has been disposed.</exception>
        /// <remarks>
        /// The caller should not dispose the returned stream. It will be disposed when this response is disposed.
        /// </remarks>
        public Stream GetContentStream()
        {
            ThrowIfDisposed();
            return _contentStream;
        }

        /// <summary>
        /// Saves the response content to a file.
        /// </summary>
        /// <param name="filePath">The path to save the file to.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the response has been disposed.</exception>
        public async Task SaveToFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

            ThrowIfDisposed();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await _contentStream.CopyToAsync(fileStream, cancellationToken);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MineruResponse));
        }

        /// <summary>
        /// Disposes the response and releases associated resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _contentStream?.Dispose();
            _httpResponse?.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Asynchronously disposes the response and releases associated resources.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            if (_contentStream != null)
                await _contentStream.DisposeAsync();

            _httpResponse?.Dispose();
            _disposed = true;
        }
    }
}
