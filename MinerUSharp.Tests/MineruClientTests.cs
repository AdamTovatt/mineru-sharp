using EasyReasy;
using MinerUSharp.Exceptions;
using MinerUSharp.Models;
using MinerUSharp.Tests.TestHelpers;
using System.Net;
using System.Text;

namespace MinerUSharp.Tests
{
    public sealed class MineruClientTests : IClassFixture<ResourceManagerFixture>, IDisposable
    {
        private readonly MockHttpMessageHandler _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly ResourceManager _resourceManager;

        public MineruClientTests(ResourceManagerFixture fixture)
        {
            _mockHandler = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_mockHandler);
            _resourceManager = fixture.ResourceManager;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        [Fact]
        public void Constructor_WithNullOrEmptyBaseUrl_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MineruClient(string.Empty));
            Assert.Throws<ArgumentException>(() => new MineruClient("   "));
        }

        [Fact]
        public void Constructor_WithValidBaseUrl_ShouldCreateClient()
        {
            // Act
            using MineruClient client = new MineruClient("http://localhost:8080");

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public void Constructor_WithHttpClient_ShouldUseProvidedClient()
        {
            // Arrange
            using HttpClient httpClient = new HttpClient();

            // Act
            using MineruClient client = new MineruClient("http://localhost:8080", httpClient);

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public async Task ParseFileAsync_WithNullRequest_ShouldThrowArgumentNullException()
        {
            // Arrange
            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => client.ParseFileAsync(null!));
        }

        [Fact]
        public async Task ParseFileAsync_WithInvalidRequest_ShouldThrowArgumentException()
        {
            // Arrange
            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            MineruRequest request = new MineruRequest
            {
                Files = Array.Empty<Stream>(), // Invalid: no files
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => client.ParseFileAsync(request));
        }

        [Fact]
        public async Task ParseFileAsync_WithSuccessResponse_ShouldReturnResponse()
        {
            // Arrange
            string responseContent = await _resourceManager.ReadAsStringAsync(TestFile.ClientTestResponse);
            _mockHandler.SetupResponse(HttpStatusCode.OK, responseContent, "application/json");

            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            using MemoryStream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test file content"));

            MineruRequest request = MineruRequest.Create()
                .WithFile(fileStream)
                .WithMarkdownResponse()
                .Build();

            // Act
            using MineruResponse response = await client.ParseFileAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string markdown = await response.ReadAsMarkdownAsync();
            Assert.Equal("# Parsed Document\n\nContent here.", markdown);
        }

        [Fact]
        public async Task ParseFileAsync_WithValidationError_ShouldThrowMineruApiException()
        {
            // Arrange
            string errorResponse = @"{
            ""detail"": [
                {
                    ""loc"": [""body"", ""files""],
                    ""msg"": ""field required"",
                    ""type"": ""value_error.missing""
                }
            ]
        }";
            _mockHandler.SetupResponse(HttpStatusCode.UnprocessableEntity, errorResponse, "application/json");

            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            using MemoryStream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test file content"));

            MineruRequest request = MineruRequest.Create()
                .WithFile(fileStream)
                .Build();

            // Act & Assert
            MineruApiException exception = await Assert.ThrowsAsync<MineruApiException>(
                () => client.ParseFileAsync(request));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
            Assert.NotNull(exception.ValidationErrors);
            Assert.Single(exception.ValidationErrors);
            Assert.Equal("field required", exception.ValidationErrors[0].Message);
            Assert.Equal("value_error.missing", exception.ValidationErrors[0].ErrorType);
        }

        [Fact]
        public async Task ParseFileAsync_WithBadRequest_ShouldThrowMineruApiException()
        {
            // Arrange
            _mockHandler.SetupResponse(HttpStatusCode.BadRequest, "Bad request", "text/plain");

            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            using MemoryStream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test file content"));

            MineruRequest request = MineruRequest.Create()
                .WithFile(fileStream)
                .Build();

            // Act & Assert
            MineruApiException exception = await Assert.ThrowsAsync<MineruApiException>(
                () => client.ParseFileAsync(request));

            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Contains("malformed", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ParseFileAsync_WithNotFound_ShouldThrowMineruApiException()
        {
            // Arrange
            _mockHandler.SetupResponse(HttpStatusCode.NotFound, "Not found", "text/plain");

            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            using MemoryStream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test file content"));

            MineruRequest request = MineruRequest.Create()
                .WithFile(fileStream)
                .Build();

            // Act & Assert
            MineruApiException exception = await Assert.ThrowsAsync<MineruApiException>(
                () => client.ParseFileAsync(request));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
            Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ParseFileAsync_WithInternalServerError_ShouldThrowMineruApiException()
        {
            // Arrange
            _mockHandler.SetupResponse(HttpStatusCode.InternalServerError, "Server error", "text/plain");

            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            using MemoryStream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test file content"));

            MineruRequest request = MineruRequest.Create()
                .WithFile(fileStream)
                .Build();

            // Act & Assert
            MineruApiException exception = await Assert.ThrowsAsync<MineruApiException>(
                () => client.ParseFileAsync(request));

            Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
            Assert.Contains("server encountered an error", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ParseFileAsync_WithMultipleFiles_ShouldIncludeAllFiles()
        {
            // Arrange
            _mockHandler.SetupResponse(HttpStatusCode.OK, "# Success", "text/markdown");

            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            using MemoryStream fileStream1 = new MemoryStream(Encoding.UTF8.GetBytes("file 1"));
            using MemoryStream fileStream2 = new MemoryStream(Encoding.UTF8.GetBytes("file 2"));

            MineruRequest request = MineruRequest.Create()
                .WithFiles(fileStream1, fileStream2)
                .WithLanguages("en", "ch")
                .Build();

            // Act
            using MineruResponse response = await client.ParseFileAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ParseFileAsync_WithCancellation_ShouldThrowOperationCanceledException()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockHandler.Setup((request, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            using MemoryStream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test"));

            MineruRequest request = MineruRequest.Create()
                .WithFile(fileStream)
                .Build();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => client.ParseFileAsync(request, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task ParseFileAsync_WithAllOptions_ShouldSucceed()
        {
            // Arrange
            _mockHandler.SetupResponse(HttpStatusCode.OK, "{\"result\":\"success\"}", "application/json");

            using MineruClient client = new MineruClient("http://localhost:8080", _httpClient);
            using MemoryStream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test"));

            MineruRequest request = MineruRequest.Create()
                .WithFile(fileStream)
                .WithOutputDirectory("/custom/output")
                .WithLanguages("en", "fr")
                .WithBackend("custom-backend")
                .WithParseMethod("manual")
                .WithFormulaEnabled(false)
                .WithTableEnabled(false)
                .WithServerUrl("https://example.com")
                .WithMarkdownResponse(true)
                .WithMiddleJson(true)
                .WithModelOutput(true)
                .WithContentList(true)
                .WithImages(true)
                .WithZipResponse(true)
                .WithPageRange(0, 100)
                .Build();

            // Act
            using MineruResponse response = await client.ParseFileAsync(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Dispose_AfterDispose_ShouldThrowObjectDisposedException()
        {
            // Arrange
            MineruClient client = new MineruClient("http://localhost:8080");
            using MemoryStream fileStream = new MemoryStream(Encoding.UTF8.GetBytes("test"));

            MineruRequest request = MineruRequest.Create()
                .WithFile(fileStream)
                .Build();

            // Act
            client.Dispose();

            // Assert
            Assert.ThrowsAsync<ObjectDisposedException>(() => client.ParseFileAsync(request));
        }
    }
}
