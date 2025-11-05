# MinerUSharp
[![Tests](https://github.com/AdamTovatt/mineru-sharp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/AdamTovatt/mineru-sharp/actions/workflows/dotnet.yml)
[![NuGet Version](https://img.shields.io/nuget/v/MinerUSharp.svg)](https://www.nuget.org/packages/MinerUSharp/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MinerUSharp.svg)](https://www.nuget.org/packages/MinerUSharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

A C# client library for the self-hosted [MinerU](https://github.com/opendatalab/MinerU) API.

> [!NOTE]
> Requires a self-hosted [MinerU](https://github.com/opendatalab/MinerU) API instance.
> 
> See the offical [MinerU](https://github.com/opendatalab/MinerU) page for instructions on self-hosting or use the wrapper [`MinerUHost`](https://github.com/AdamTovatt/mineru-host) for automatic setup.

## Installation

The easiest way to install and use the `MinerUSharp` library is through [NuGet](https://www.nuget.org/packages/MinerUSharp).
```bash
dotnet add package MinerUSharp
```

That's it! You've now installed `MinerUSharp`.

Don't forget to also install some way of hosting the [MinerU](https://github.com/opendatalab/MinerU) API.

The [official page](https://github.com/opendatalab/MinerU) has instructions. You can also use [`MinerUHost`](https://github.com/AdamTovatt/mineru-host) which automates both setup and process management for the underlying Mineru Python service. It is available as a [NuGet package](https://www.nuget.org/packages/MinerUHost) and as a prebuilt standalone application through [GitHub Releases](https://github.com/AdamTovatt/mineru-host/releases).

## Usage

### Basic Usage

Import the following namespaces:
```csharp
using MinerUSharp;
using MinerUSharp.Models;
```

Then use it like this:
```csharp
using MineruClient client = new MineruClient("http://localhost:8080");
using FileStream fileStream = File.OpenRead("document.pdf");

MineruRequest request = new MineruRequest
{
    Files = new[] { fileStream },
    LanguageList = new[] { "en", "ch" },
    StartPageId = 1,
    EndPageId = 10,
    ReturnMarkdown = true,
};

using MineruResponse response = await client.ParseFileAsync(request);
string markdown = await response.ReadAsMarkdownAsync();
```

### Fluent API

You can also use it with fluent API.
This example does the same thing as the example above:
```csharp
using MineruClient client = new MineruClient("http://localhost:8080");
using FileStream fileStream = File.OpenRead("document.pdf");

MineruRequest request = MineruRequest.Create(fileStream)
    .WithLanguages("en", "ch")
    .WithMarkdownResponse()
    .WithPageRange(startPage: 1, endPage: 10)
    .Build();

using MineruResponse response = await client.ParseFileAsync(request);
string markdown = await response.ReadAsMarkdownAsync();
```

## "Advanced Usage"
The quotes are because it's not really that "advanced", but here are some more detailed code snippets:
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

## Additional Options

- The `MineruClient` constructor accepts an optional `HttpClient` parameter for custom HTTP client configuration.
- The `ParseFileAsync` method accepts an optional `CancellationToken` parameter for cancellation support.

> [!NOTE]
> The underlying `MinerU` Python API doesn't seem to correctly handle cancelled requests. It seems to continue processing them until they are finished, at least at the time of writing this. The cancellation can still be used to free up your own thread that's calling the Python process, but the Python process will still continue in the background until it's done.

```csharp
using HttpClient httpClient = new HttpClient();

using MineruClient client = new MineruClient("http://localhost:8080", httpClient);

// The timeout of 5000 ms shown below is very short and not recommended for real scenarios, it's just an example
// that shows that a cancellation token can be sent. It should probably come from a user controlled source.
using CancellationTokenSource cts = new CancellationTokenSource(millisecondsDelay: 5000);
using MineruResponse response = await client.ParseFileAsync(request, cts.Token);
```

## Requirements

- .NET 8.0 or later
- [MinerU](https://github.com/opendatalab/MinerU) API server that can be accessed

