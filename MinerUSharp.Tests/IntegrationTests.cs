using EasyReasy;
using MinerUSharp.Models;
using MinerUSharp.Tests.TestHelpers;

namespace MinerUSharp.Tests
{
    public sealed class IntegrationTests : IClassFixture<ResourceManagerFixture>
    {
        private readonly ResourceManager _resourceManager;

        public IntegrationTests(ResourceManagerFixture fixture)
        {
            _resourceManager = fixture.ResourceManager;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ReadTestImage_ShouldReturnImageBytes()
        {
            // Act
            byte[] imageBytes = await _resourceManager.ReadAsBytesAsync(TestFile.Image01);

            // Assert
            Assert.NotNull(imageBytes);
            Assert.NotEmpty(imageBytes);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ParseTestImage_ShouldReturnCorrectText()
        {
            using (HttpClient http = new HttpClient())
            {
                using (MineruClient client = new MineruClient("http://localhost:8000/", http))
                {
                    using (Stream imageStream = await _resourceManager.GetResourceStreamAsync(TestFile.Image01))
                    {
                        MineruRequest request = MineruRequest.Create(imageStream).WithLanguages("en").WithTableEnabled(false).Build();
                        MineruResponse response = await client.ParseFileAsync(request);

                        string responseMarkdown = await response.ReadAsMarkdownAsync();

                        string expectedText = await _resourceManager.ReadAsStringAsync(TestFile.Text01);

                        Assert.Equal(expectedText.NormalizeLineEndings(), responseMarkdown.NormalizeLineEndings());
                    }
                }
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ParseFile_WithCancellation_ShouldThrowOperationCanceledException()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(5000))
            {
                using (HttpClient http = new HttpClient())
                {
                    using (MineruClient client = new MineruClient("http://localhost:8000/", http))
                    {
                        using (Stream imageStream = await _resourceManager.GetResourceStreamAsync(TestFile.Image01))
                        {
                            MineruRequest request = MineruRequest.Create(imageStream).WithLanguages("en").Build();

                            await Assert.ThrowsAsync<TaskCanceledException>(async () =>
                                await client.ParseFileAsync(request, cancellationTokenSource.Token));
                        }
                    }
                }
            }
        }
    }
}

