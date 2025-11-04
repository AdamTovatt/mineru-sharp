# MinerUSharp

A C# client library for the [MinerU](https://github.com/opendatalab/MinerU) API.

## Installation

Add the project reference to your solution or build the library and reference the DLL.

```bash
dotnet add reference path/to/MinerUSharp/MinerUSharp.csproj
```

## Usage

### Basic Usage

```csharp
using MinerUSharp;
using MinerUSharp.Models;

using MineruClient client = new MineruClient("http://localhost:8080");
using FileStream fileStream = File.OpenRead("document.pdf");

MineruRequest request = new MineruRequest
{
    Files = new[] { fileStream },
    LanguageList = new[] { "en", "ch" },
    ReturnMarkdown = true,
};

using MineruResponse response = await client.ParseFileAsync(request);
string markdown = await response.ReadAsMarkdownAsync();
```

### Fluent API

```csharp
MineruRequest request = MineruRequest.Create()
    .WithFile(fileStream)
    .WithLanguages("en", "ch")
    .WithMarkdownResponse()
    .WithPageRange(startPage: 1, endPage: 10)
    .Build();

using MineruResponse response = await client.ParseFileAsync(request);
string markdown = await response.ReadAsMarkdownAsync();
```

### Dependency Injection

```csharp
// Program.cs or Startup.cs
services.AddMineruClient("http://localhost:8080");

// In your service
public class DocumentService
{
    private readonly IMineruClient _client;
    
    public DocumentService(IMineruClient client)
    {
        _client = client;
    }
    
    public async Task<string> ParseDocumentAsync(Stream documentStream)
    {
        MineruRequest request = MineruRequest.Create()
            .WithFile(documentStream)
            .WithMarkdownResponse()
            .Build();
            
        using MineruResponse response = await _client.ParseFileAsync(request);
        return await response.ReadAsMarkdownAsync();
    }
}
```

### Response Options

```csharp
using MineruResponse response = await client.ParseFileAsync(request);

// Read as markdown
string markdown = await response.ReadAsMarkdownAsync();

// Read as JSON
JsonElement json = await response.ReadAsJsonAsync();

// Read as bytes
byte[] bytes = await response.ReadAsBytesAsync();

// Save to file
await response.SaveToFileAsync("output.md");

// Get raw stream for custom processing
Stream stream = response.GetContentStream();
```

## Features

- Fluent request builder API
- Stream-based response handling
- Support for multiple file formats
- Dependency injection integration
- Comprehensive error handling with validation details
- Full async/await support
- Proper resource disposal with IDisposable/IAsyncDisposable

## Requirements

- .NET 8.0 or later
- MinerU API server running and accessible

