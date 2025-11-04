using System.Text.Json.Serialization;

namespace MinerUSharp.Models
{
    /// <summary>
    /// Represents the JSON body of a response from the MinerU API.
    /// </summary>
    public sealed class MineruResponseBody
    {
        /// <summary>
        /// Gets or sets the backend used for processing.
        /// </summary>
        [JsonPropertyName("backend")]
        public string Backend { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version of the MinerU API.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the results dictionary containing parsed file results.
        /// The key is the file identifier (e.g., "file0", "file1") and the value contains the parsed content.
        /// </summary>
        [JsonPropertyName("results")]
        public Dictionary<string, MineruFileResult> Results { get; set; } = new Dictionary<string, MineruFileResult>();
    }

    /// <summary>
    /// Represents the result for a single parsed file.
    /// </summary>
    public sealed class MineruFileResult
    {
        /// <summary>
        /// Gets or sets the markdown content extracted from the file.
        /// </summary>
        [JsonPropertyName("md_content")]
        public string? MarkdownContent { get; set; }

        /// <summary>
        /// Gets or sets the middle JSON content if requested.
        /// </summary>
        [JsonPropertyName("middle_json")]
        public object? MiddleJson { get; set; }

        /// <summary>
        /// Gets or sets the model output if requested.
        /// </summary>
        [JsonPropertyName("model_output")]
        public object? ModelOutput { get; set; }

        /// <summary>
        /// Gets or sets the content list if requested.
        /// </summary>
        [JsonPropertyName("content_list")]
        public object? ContentList { get; set; }
    }
}

