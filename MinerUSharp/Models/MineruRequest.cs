namespace MinerUSharp.Models
{
    /// <summary>
    /// Represents a request to parse files using the MinerU API.
    /// </summary>
    public sealed class MineruRequest
    {
        /// <summary>
        /// Gets or sets the files to parse. This property is required.
        /// </summary>
        public IReadOnlyList<Stream> Files { get; set; } = Array.Empty<Stream>();

        /// <summary>
        /// Gets or sets the output directory path.
        /// </summary>
        public string OutputDirectory { get; set; } = MineruDefaults.DefaultOutputDirectory;

        /// <summary>
        /// Gets or sets the list of languages to use for OCR.
        /// </summary>
        public IReadOnlyList<string> LanguageList { get; set; } = MineruDefaults.DefaultLanguageList;

        /// <summary>
        /// Gets or sets the backend to use for processing.
        /// </summary>
        public string Backend { get; set; } = MineruDefaults.DefaultBackend;

        /// <summary>
        /// Gets or sets the parse method.
        /// </summary>
        public string ParseMethod { get; set; } = MineruDefaults.DefaultParseMethod;

        /// <summary>
        /// Gets or sets a value indicating whether formula parsing is enabled.
        /// </summary>
        public bool FormulaEnable { get; set; } = MineruDefaults.DefaultFormulaEnable;

        /// <summary>
        /// Gets or sets a value indicating whether table parsing is enabled.
        /// </summary>
        public bool TableEnable { get; set; } = MineruDefaults.DefaultTableEnable;

        /// <summary>
        /// Gets or sets the server URL for additional processing.
        /// </summary>
        public string? ServerUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to return markdown output.
        /// </summary>
        public bool ReturnMarkdown { get; set; } = MineruDefaults.DefaultReturnMarkdown;

        /// <summary>
        /// Gets or sets a value indicating whether to return the middle JSON.
        /// </summary>
        public bool ReturnMiddleJson { get; set; } = MineruDefaults.DefaultReturnMiddleJson;

        /// <summary>
        /// Gets or sets a value indicating whether to return the model output.
        /// </summary>
        public bool ReturnModelOutput { get; set; } = MineruDefaults.DefaultReturnModelOutput;

        /// <summary>
        /// Gets or sets a value indicating whether to return the content list.
        /// </summary>
        public bool ReturnContentList { get; set; } = MineruDefaults.DefaultReturnContentList;

        /// <summary>
        /// Gets or sets a value indicating whether to return extracted images.
        /// </summary>
        public bool ReturnImages { get; set; } = MineruDefaults.DefaultReturnImages;

        /// <summary>
        /// Gets or sets a value indicating whether to return the response as a ZIP file.
        /// </summary>
        public bool ResponseFormatZip { get; set; } = MineruDefaults.DefaultResponseFormatZip;

        /// <summary>
        /// Gets or sets the starting page ID (0-indexed).
        /// </summary>
        public int StartPageId { get; set; } = MineruDefaults.DefaultStartPageId;

        /// <summary>
        /// Gets or sets the ending page ID (inclusive).
        /// </summary>
        public int EndPageId { get; set; } = MineruDefaults.DefaultEndPageId;

        /// <summary>
        /// Creates a new fluent request builder.
        /// </summary>
        /// <returns>A new <see cref="MineruRequestBuilder"/> instance.</returns>
        public static MineruRequestBuilder Create()
        {
            return new MineruRequestBuilder();
        }

        /// <summary>
        /// Creates a new fluent request builder with a file.
        /// </summary>
        /// <param name="file">The file stream to include in the request.</param>
        /// <returns>A new <see cref="MineruRequestBuilder"/> instance with the file added.</returns>
        public static MineruRequestBuilder Create(Stream file)
        {
            return new MineruRequestBuilder().WithFile(file);
        }

        /// <summary>
        /// Validates the request and throws an exception if invalid.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the request is invalid.</exception>
        public void Validate()
        {
            if (Files == null || Files.Count == 0)
                throw new ArgumentException("At least one file is required.", nameof(Files));

            if (Files.Any(file => file == null))
                throw new ArgumentException("All files must be non-null.", nameof(Files));

            if (StartPageId < 0)
                throw new ArgumentException("Start page ID must be non-negative.", nameof(StartPageId));

            if (EndPageId < StartPageId)
                throw new ArgumentException("End page ID must be greater than or equal to start page ID.", nameof(EndPageId));
        }
    }
}
