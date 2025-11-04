# MinerUSharp
[![Tests](https://github.com/AdamTovatt/mineru-sharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AdamTovatt/mineru-sharp/actions/workflows/dotnet.yml)
[![NuGet Version](https://img.shields.io/nuget/v/MinerUSharp.svg)](https://www.nuget.org/packages/MinerUSharp/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MinerUSharp.svg)](https://www.nuget.org/packages/MinerUSharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

A C# client library for the [MinerU](https://github.com/opendatalab/MinerU) API.

## Installation

The easiest way to install is with [NuGet](https://www.nuget.org/packages/MinerUSharp).

```bash
dotnet add package MinerUSharp
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
MineruRequest request = MineruRequest.Create(fileStream)
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
        MineruRequest request = MineruRequest.Create(documentStream)
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

// Read as markdown (extracts md_content from the JSON response)
string markdown = await response.ReadAsMarkdownAsync();

// Read as strongly-typed response body
MineruResponseBody body = await response.ReadAsResponseBodyAsync();
string markdownFromFirstFile = body.Results["file0"].MarkdownContent;

// Read as raw JSON element
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

