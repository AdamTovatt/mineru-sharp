using EasyReasy;
using MinerUSharp.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MinerUSharp.Tests
{
    public sealed class MineruResponseTests : IClassFixture<ResourceManagerFixture>
    {
        private readonly ResourceManager _resourceManager;

        public MineruResponseTests(ResourceManagerFixture fixture)
        {
            _resourceManager = fixture.ResourceManager;
        }

        [Fact]
        public async Task ReadAsMarkdownAsync_WithJsonResponse_ShouldExtractMarkdownContent()
        {
            // Arrange
            byte[] responseBytes = await _resourceManager.ReadAsBytesAsync(TestFile.SimpleMarkdownResponse);
            MemoryStream contentStream = new MemoryStream(responseBytes);
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(responseBytes),
            };
            httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            string markdown = await response.ReadAsMarkdownAsync();

            // Assert
            Assert.Equal("# Test Document\n\nThis is a test.", markdown);

            // Cleanup
            await response.DisposeAsync();
        }

        [Fact]
        public async Task ReadAsMarkdownAsync_WithRealMarkdownResponse_ShouldExtractMarkdownContent()
        {
            // Arrange
            byte[] responseBytes = await _resourceManager.ReadAsBytesAsync(TestFile.MarkdownResponse01);
            MemoryStream contentStream = new MemoryStream(responseBytes);
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(responseBytes),
            };
            httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            string markdown = await response.ReadAsMarkdownAsync();

            // Assert
            Assert.NotNull(markdown);
            Assert.Contains("simple sting I", markdown);
            Assert.Contains("parsed?", markdown);
            Assert.Contains("Also, here is some text further down.", markdown);

            // Cleanup
            await response.DisposeAsync();
        }

        [Fact]
        public async Task ReadAsResponseBodyAsync_WithJsonResponse_ShouldDeserializeCorrectly()
        {
            // Arrange
            byte[] responseBytes = await _resourceManager.ReadAsBytesAsync(TestFile.MultiResultResponse);
            MemoryStream contentStream = new MemoryStream(responseBytes);
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(responseBytes),
            };
            httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            MineruResponseBody body = await response.ReadAsResponseBodyAsync();

            // Assert
            Assert.NotNull(body);
            Assert.Equal("pipeline", body.Backend);
            Assert.Equal("2.6.3", body.Version);
            Assert.Single(body.Results);
            Assert.True(body.Results.ContainsKey("file0"));
            Assert.Equal("Test content", body.Results["file0"].MarkdownContent);

            // Cleanup
            await response.DisposeAsync();
        }

        [Fact]
        public async Task ReadAsJsonAsync_WithJsonContent_ShouldReturnJsonElement()
        {
            // Arrange
            string jsonContent = "{\"result\":\"success\",\"count\":42}";
            MemoryStream contentStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent),
            };
            httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            JsonElement json = await response.ReadAsJsonAsync();

            // Assert
            Assert.Equal(JsonValueKind.Object, json.ValueKind);
            Assert.Equal("success", json.GetProperty("result").GetString());
            Assert.Equal(42, json.GetProperty("count").GetInt32());

            // Cleanup
            await response.DisposeAsync();
        }

        [Fact]
        public async Task ReadAsBytesAsync_ShouldReturnByteArray()
        {
            // Arrange
            byte[] expectedBytes = new byte[] { 0x50, 0x44, 0x46, 0x2D }; // PDF magic bytes
            MemoryStream contentStream = new MemoryStream(expectedBytes);
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(contentStream),
            };

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            byte[] actualBytes = await response.ReadAsBytesAsync();

            // Assert
            Assert.Equal(expectedBytes, actualBytes);

            // Cleanup
            await response.DisposeAsync();
        }

        [Fact]
        public void GetContentStream_ShouldReturnStream()
        {
            // Arrange
            MemoryStream contentStream = new MemoryStream();
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            Stream stream = response.GetContentStream();

            // Assert
            Assert.NotNull(stream);
            Assert.Same(contentStream, stream);

            // Cleanup
            response.Dispose();
        }

        [Fact]
        public async Task SaveToFileAsync_ShouldWriteToFile()
        {
            // Arrange
            string content = "Test content for file";
            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
            MemoryStream contentStream = new MemoryStream(contentBytes);
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            string tempFilePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.txt");

            try
            {
                // Act
                await response.SaveToFileAsync(tempFilePath);

                // Assert
                Assert.True(File.Exists(tempFilePath));
                string fileContent = await File.ReadAllTextAsync(tempFilePath);
                Assert.Equal(content, fileContent);
            }
            finally
            {
                // Cleanup
                await response.DisposeAsync();
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }

        [Fact]
        public void StatusCode_ShouldReturnHttpStatusCode()
        {
            // Arrange
            MemoryStream contentStream = new MemoryStream();
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            HttpStatusCode statusCode = response.StatusCode;

            // Assert
            Assert.Equal(HttpStatusCode.OK, statusCode);

            // Cleanup
            response.Dispose();
        }

        [Fact]
        public void ContentType_WithJsonContent_ShouldReturnJsonContentType()
        {
            // Arrange
            MemoryStream contentStream = new MemoryStream();
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("test"),
            };
            httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            string? contentType = response.ContentType;

            // Assert
            Assert.Equal("application/json", contentType);

            // Cleanup
            response.Dispose();
        }

        [Fact]
        public void IsZipResponse_WithZipContentType_ShouldReturnTrue()
        {
            // Arrange
            MemoryStream contentStream = new MemoryStream();
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("test"),
            };
            httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            bool isZip = response.IsZipResponse;

            // Assert
            Assert.True(isZip);

            // Cleanup
            response.Dispose();
        }

        [Fact]
        public void IsZipResponse_WithNonZipContentType_ShouldReturnFalse()
        {
            // Arrange
            MemoryStream contentStream = new MemoryStream();
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("test"),
            };
            httpResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            bool isZip = response.IsZipResponse;

            // Assert
            Assert.False(isZip);

            // Cleanup
            response.Dispose();
        }

        [Fact]
        public async Task DisposeAsync_ShouldDisposeResources()
        {
            // Arrange
            MemoryStream contentStream = new MemoryStream();
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            await response.DisposeAsync();

            // Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await response.ReadAsMarkdownAsync());
        }

        [Fact]
        public void Dispose_ShouldDisposeResources()
        {
            // Arrange
            MemoryStream contentStream = new MemoryStream();
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act
            response.Dispose();

            // Assert
            Assert.Throws<ObjectDisposedException>(() => response.GetContentStream());
        }

        [Fact]
        public async Task MultipleCalls_ToDisposeAsync_ShouldNotThrow()
        {
            // Arrange
            MemoryStream contentStream = new MemoryStream();
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            MineruResponse response = new MineruResponse(httpResponse, contentStream);

            // Act & Assert
            await response.DisposeAsync();
            await response.DisposeAsync(); // Should not throw
        }
    }
}
