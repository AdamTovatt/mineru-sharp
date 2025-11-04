using MinerUSharp.Models;

namespace MinerUSharp.Tests
{
    public sealed class MineruRequestTests
{
    [Fact]
    public void Request_WithDefaultValues_ShouldHaveExpectedDefaults()
    {
        // Arrange & Act
        MineruRequest request = new MineruRequest();

        // Assert
        Assert.Empty(request.Files);
        Assert.Equal("./output", request.OutputDirectory);
        Assert.Single(request.LanguageList);
        Assert.Equal("ch", request.LanguageList[0]);
        Assert.Equal("pipeline", request.Backend);
        Assert.Equal("auto", request.ParseMethod);
        Assert.True(request.FormulaEnable);
        Assert.True(request.TableEnable);
        Assert.Null(request.ServerUrl);
        Assert.True(request.ReturnMarkdown);
        Assert.False(request.ReturnMiddleJson);
        Assert.False(request.ReturnModelOutput);
        Assert.False(request.ReturnContentList);
        Assert.False(request.ReturnImages);
        Assert.False(request.ResponseFormatZip);
        Assert.Equal(0, request.StartPageId);
        Assert.Equal(99999, request.EndPageId);
    }

    [Fact]
    public void Validate_WithNoFiles_ShouldThrowArgumentException()
    {
        // Arrange
        MineruRequest request = new MineruRequest
        {
            Files = Array.Empty<Stream>(),
        };

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => request.Validate());
        Assert.Contains("file", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithNullFile_ShouldThrowArgumentException()
    {
        // Arrange
        MineruRequest request = new MineruRequest
        {
            Files = new Stream[] { null! },
        };

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => request.Validate());
        Assert.Contains("non-null", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithNegativeStartPage_ShouldThrowArgumentException()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();
        MineruRequest request = new MineruRequest
        {
            Files = new[] { stream },
            StartPageId = -1,
        };

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => request.Validate());
        Assert.Contains("Start page", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithEndPageBeforeStartPage_ShouldThrowArgumentException()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();
        MineruRequest request = new MineruRequest
        {
            Files = new[] { stream },
            StartPageId = 10,
            EndPageId = 5,
        };

        // Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() => request.Validate());
        Assert.Contains("End page", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_WithValidRequest_ShouldNotThrow()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();
        MineruRequest request = new MineruRequest
        {
            Files = new[] { stream },
        };

        // Act & Assert
        request.Validate(); // Should not throw
    }

    [Fact]
    public void Create_ShouldReturnBuilder()
    {
        // Act
        MineruRequestBuilder builder = MineruRequest.Create();

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void Builder_WithFile_ShouldAddFile()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .Build();

        // Assert
        Assert.Single(request.Files);
        Assert.Same(stream, request.Files[0]);
    }

    [Fact]
    public void Builder_WithFiles_ShouldAddMultipleFiles()
    {
        // Arrange
        using MemoryStream stream1 = new MemoryStream();
        using MemoryStream stream2 = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFiles(stream1, stream2)
            .Build();

        // Assert
        Assert.Equal(2, request.Files.Count);
        Assert.Same(stream1, request.Files[0]);
        Assert.Same(stream2, request.Files[1]);
    }

    [Fact]
    public void Builder_WithOutputDirectory_ShouldSetOutputDirectory()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithOutputDirectory("/custom/output")
            .Build();

        // Assert
        Assert.Equal("/custom/output", request.OutputDirectory);
    }

    [Fact]
    public void Builder_WithLanguages_ShouldSetLanguages()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithLanguages("en", "fr", "de")
            .Build();

        // Assert
        Assert.Equal(3, request.LanguageList.Count);
        Assert.Equal("en", request.LanguageList[0]);
        Assert.Equal("fr", request.LanguageList[1]);
        Assert.Equal("de", request.LanguageList[2]);
    }

    [Fact]
    public void Builder_WithBackend_ShouldSetBackend()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithBackend("custom-backend")
            .Build();

        // Assert
        Assert.Equal("custom-backend", request.Backend);
    }

    [Fact]
    public void Builder_WithParseMethod_ShouldSetParseMethod()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithParseMethod("manual")
            .Build();

        // Assert
        Assert.Equal("manual", request.ParseMethod);
    }

    [Fact]
    public void Builder_WithFormulaEnabled_ShouldSetFormulaEnable()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithFormulaEnabled(false)
            .Build();

        // Assert
        Assert.False(request.FormulaEnable);
    }

    [Fact]
    public void Builder_WithTableEnabled_ShouldSetTableEnable()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithTableEnabled(false)
            .Build();

        // Assert
        Assert.False(request.TableEnable);
    }

    [Fact]
    public void Builder_WithServerUrl_ShouldSetServerUrl()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithServerUrl("https://example.com")
            .Build();

        // Assert
        Assert.Equal("https://example.com", request.ServerUrl);
    }

    [Fact]
    public void Builder_WithMarkdownResponse_ShouldSetReturnMarkdown()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithMarkdownResponse(false)
            .Build();

        // Assert
        Assert.False(request.ReturnMarkdown);
    }

    [Fact]
    public void Builder_WithMiddleJson_ShouldSetReturnMiddleJson()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithMiddleJson(true)
            .Build();

        // Assert
        Assert.True(request.ReturnMiddleJson);
    }

    [Fact]
    public void Builder_WithModelOutput_ShouldSetReturnModelOutput()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithModelOutput(true)
            .Build();

        // Assert
        Assert.True(request.ReturnModelOutput);
    }

    [Fact]
    public void Builder_WithContentList_ShouldSetReturnContentList()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithContentList(true)
            .Build();

        // Assert
        Assert.True(request.ReturnContentList);
    }

    [Fact]
    public void Builder_WithImages_ShouldSetReturnImages()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithImages(true)
            .Build();

        // Assert
        Assert.True(request.ReturnImages);
    }

    [Fact]
    public void Builder_WithZipResponse_ShouldSetResponseFormatZip()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithZipResponse(true)
            .Build();

        // Assert
        Assert.True(request.ResponseFormatZip);
    }

    [Fact]
    public void Builder_WithPageRange_ShouldSetPageRange()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithPageRange(5, 15)
            .Build();

        // Assert
        Assert.Equal(5, request.StartPageId);
        Assert.Equal(15, request.EndPageId);
    }

    [Fact]
    public void Builder_WithPageRange_InvalidRange_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            MineruRequest.Create()
                .WithPageRange(10, 5));

        Assert.Contains("End page", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Builder_ChainMultipleCalls_ShouldSetAllProperties()
    {
        // Arrange
        using MemoryStream stream = new MemoryStream();

        // Act
        MineruRequest request = MineruRequest.Create()
            .WithFile(stream)
            .WithOutputDirectory("/output")
            .WithLanguages("en", "ch")
            .WithMarkdownResponse()
            .WithPageRange(1, 10)
            .WithFormulaEnabled(false)
            .Build();

        // Assert
        Assert.Single(request.Files);
        Assert.Equal("/output", request.OutputDirectory);
        Assert.Equal(2, request.LanguageList.Count);
        Assert.True(request.ReturnMarkdown);
        Assert.Equal(1, request.StartPageId);
        Assert.Equal(10, request.EndPageId);
        Assert.False(request.FormulaEnable);
    }
}
}
