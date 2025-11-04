using MinerUSharp.Exceptions;
using MinerUSharp.Internal;
using MinerUSharp.Models;
using System.Net;
using System.Text.Json;

namespace MinerUSharp
{
    /// <summary>
    /// Client for interacting with the MinerU API.
    /// </summary>
    public sealed class MineruClient : IMineruClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly bool _ownsHttpClient;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MineruClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL of the MinerU API (e.g., "http://localhost:8080").</param>
        /// <param name="httpClient">Optional HttpClient instance. If not provided, a new instance will be created.</param>
        public MineruClient(string baseUrl, HttpClient? httpClient = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL cannot be null or whitespace.", nameof(baseUrl));

            _baseUrl = baseUrl.TrimEnd('/');

            if (httpClient == null)
            {
                _httpClient = new HttpClient();
                _ownsHttpClient = true;
            }
            else
            {
                _httpClient = httpClient;
                _ownsHttpClient = false;
            }
        }

        /// <summary>
        /// Parses files using the MinerU API.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="MineruResponse"/> containing the parsed content.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the request is invalid.</exception>
        /// <exception cref="MineruApiException">Thrown if the API returns an error.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the client has been disposed.</exception>
        public async Task<MineruResponse> ParseFileAsync(MineruRequest request, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MineruClient));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Validate();

            string requestUri = $"{_baseUrl}/file_parse";

            using (MultipartFormDataContent content = MultipartFormDataHelper.CreateContent(request))
            {
                HttpResponseMessage httpResponse;

                try
                {
                    httpResponse = await _httpClient.PostAsync(requestUri, content, cancellationToken);
                }
                catch (HttpRequestException ex)
                {
                    throw new MineruApiException(
                        message: "Failed to send request to the MinerU API.",
                        statusCode: HttpStatusCode.ServiceUnavailable,
                        innerException: ex);
                }
                catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new MineruApiException(
                        message: "Request to the MinerU API timed out.",
                        statusCode: HttpStatusCode.RequestTimeout,
                        innerException: ex);
                }

                if (!httpResponse.IsSuccessStatusCode)
                {
                    await HandleErrorResponseAsync(httpResponse, cancellationToken);
                }

                Stream contentStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
                return new MineruResponse(httpResponse, contentStream);
            }
        }

        private static async Task HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            string? responseContent = null;
            IReadOnlyList<ValidationError>? validationErrors = null;

            try
            {
                responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
                {
                    JsonDocument document = JsonDocument.Parse(responseContent);

                    if (document.RootElement.TryGetProperty("detail", out JsonElement detailElement) &&
                        detailElement.ValueKind == JsonValueKind.Array)
                    {
                        List<ValidationError> errors = new List<ValidationError>();

                        foreach (JsonElement errorElement in detailElement.EnumerateArray())
                        {
                            ValidationError error = new ValidationError();

                            if (errorElement.TryGetProperty("loc", out JsonElement locElement))
                            {
                                List<object> locations = new List<object>();
                                foreach (JsonElement locItem in locElement.EnumerateArray())
                                {
                                    if (locItem.ValueKind == JsonValueKind.String)
                                        locations.Add(locItem.GetString() ?? string.Empty);
                                    else if (locItem.ValueKind == JsonValueKind.Number)
                                        locations.Add(locItem.GetInt32());
                                }
                                error.Location = locations.AsReadOnly();
                            }

                            if (errorElement.TryGetProperty("msg", out JsonElement msgElement))
                            {
                                error.Message = msgElement.GetString() ?? string.Empty;
                            }

                            if (errorElement.TryGetProperty("type", out JsonElement typeElement))
                            {
                                error.ErrorType = typeElement.GetString() ?? string.Empty;
                            }

                            errors.Add(error);
                        }

                        validationErrors = errors.AsReadOnly();
                    }
                }
            }
            catch
            {
                // If we can't parse the error response, we'll just use the raw content
            }

            string errorMessage = response.StatusCode switch
            {
                HttpStatusCode.UnprocessableEntity => "The request contains validation errors.",
                HttpStatusCode.BadRequest => "The request was malformed or invalid.",
                HttpStatusCode.NotFound => "The API endpoint was not found.",
                HttpStatusCode.InternalServerError => "The API server encountered an error.",
                _ => $"The API returned an error: {response.StatusCode}",
            };

            throw new MineruApiException(
                message: errorMessage,
                statusCode: response.StatusCode,
                responseContent: responseContent,
                validationErrors: validationErrors);
        }

        /// <summary>
        /// Disposes the client and releases associated resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (_ownsHttpClient)
            {
                _httpClient?.Dispose();
            }

            _disposed = true;
        }
    }
}
