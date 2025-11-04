namespace MinerUSharp.Models
{
    /// <summary>
    /// Fluent builder for creating <see cref="MineruRequest"/> instances.
    /// </summary>
    public sealed class MineruRequestBuilder
    {
        private readonly List<Stream> _files = new List<Stream>();
        private string _outputDirectory = MineruDefaults.DefaultOutputDirectory;
        private List<string> _languageList = new List<string>(MineruDefaults.DefaultLanguageList);
        private string _backend = MineruDefaults.DefaultBackend;
        private string _parseMethod = MineruDefaults.DefaultParseMethod;
        private bool _formulaEnable = MineruDefaults.DefaultFormulaEnable;
        private bool _tableEnable = MineruDefaults.DefaultTableEnable;
        private string? _serverUrl;
        private bool _returnMarkdown = MineruDefaults.DefaultReturnMarkdown;
        private bool _returnMiddleJson = MineruDefaults.DefaultReturnMiddleJson;
        private bool _returnModelOutput = MineruDefaults.DefaultReturnModelOutput;
        private bool _returnContentList = MineruDefaults.DefaultReturnContentList;
        private bool _returnImages = MineruDefaults.DefaultReturnImages;
        private bool _responseFormatZip = MineruDefaults.DefaultResponseFormatZip;
        private int _startPageId = MineruDefaults.DefaultStartPageId;
        private int _endPageId = MineruDefaults.DefaultEndPageId;

        internal MineruRequestBuilder()
        {
        }

        /// <summary>
        /// Adds a file to parse.
        /// </summary>
        /// <param name="fileStream">The file stream to add.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithFile(Stream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            _files.Add(fileStream);
            return this;
        }

        /// <summary>
        /// Adds multiple files to parse.
        /// </summary>
        /// <param name="fileStreams">The file streams to add.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithFiles(params Stream[] fileStreams)
        {
            if (fileStreams == null)
                throw new ArgumentNullException(nameof(fileStreams));

            _files.AddRange(fileStreams);
            return this;
        }

        /// <summary>
        /// Sets the output directory.
        /// </summary>
        /// <param name="outputDirectory">The output directory path.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithOutputDirectory(string outputDirectory)
        {
            _outputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));
            return this;
        }

        /// <summary>
        /// Sets the languages to use for OCR.
        /// </summary>
        /// <param name="languages">The language codes.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithLanguages(params string[] languages)
        {
            if (languages == null || languages.Length == 0)
                throw new ArgumentException("At least one language is required.", nameof(languages));

            _languageList = new List<string>(languages);
            return this;
        }

        /// <summary>
        /// Sets the backend to use for processing.
        /// </summary>
        /// <param name="backend">The backend name.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithBackend(string backend)
        {
            _backend = backend ?? throw new ArgumentNullException(nameof(backend));
            return this;
        }

        /// <summary>
        /// Sets the parse method.
        /// </summary>
        /// <param name="parseMethod">The parse method.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithParseMethod(string parseMethod)
        {
            _parseMethod = parseMethod ?? throw new ArgumentNullException(nameof(parseMethod));
            return this;
        }

        /// <summary>
        /// Enables or disables formula parsing.
        /// </summary>
        /// <param name="enable">True to enable formula parsing; otherwise, false.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithFormulaEnabled(bool enable = true)
        {
            _formulaEnable = enable;
            return this;
        }

        /// <summary>
        /// Enables or disables table parsing.
        /// </summary>
        /// <param name="enable">True to enable table parsing; otherwise, false.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithTableEnabled(bool enable = true)
        {
            _tableEnable = enable;
            return this;
        }

        /// <summary>
        /// Sets the server URL for additional processing.
        /// </summary>
        /// <param name="serverUrl">The server URL.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithServerUrl(string? serverUrl)
        {
            _serverUrl = serverUrl;
            return this;
        }

        /// <summary>
        /// Configures the request to return markdown output.
        /// </summary>
        /// <param name="enable">True to return markdown; otherwise, false.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithMarkdownResponse(bool enable = true)
        {
            _returnMarkdown = enable;
            return this;
        }

        /// <summary>
        /// Configures the request to return the middle JSON.
        /// </summary>
        /// <param name="enable">True to return middle JSON; otherwise, false.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithMiddleJson(bool enable = true)
        {
            _returnMiddleJson = enable;
            return this;
        }

        /// <summary>
        /// Configures the request to return the model output.
        /// </summary>
        /// <param name="enable">True to return model output; otherwise, false.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithModelOutput(bool enable = true)
        {
            _returnModelOutput = enable;
            return this;
        }

        /// <summary>
        /// Configures the request to return the content list.
        /// </summary>
        /// <param name="enable">True to return content list; otherwise, false.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithContentList(bool enable = true)
        {
            _returnContentList = enable;
            return this;
        }

        /// <summary>
        /// Configures the request to return extracted images.
        /// </summary>
        /// <param name="enable">True to return images; otherwise, false.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithImages(bool enable = true)
        {
            _returnImages = enable;
            return this;
        }

        /// <summary>
        /// Configures the request to return the response as a ZIP file.
        /// </summary>
        /// <param name="enable">True to return as ZIP; otherwise, false.</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithZipResponse(bool enable = true)
        {
            _responseFormatZip = enable;
            return this;
        }

        /// <summary>
        /// Sets the page range to parse.
        /// </summary>
        /// <param name="startPage">The starting page (0-indexed).</param>
        /// <param name="endPage">The ending page (inclusive).</param>
        /// <returns>This builder for chaining.</returns>
        public MineruRequestBuilder WithPageRange(int startPage, int endPage)
        {
            if (startPage < 0)
                throw new ArgumentException("Start page must be non-negative.", nameof(startPage));

            if (endPage < startPage)
                throw new ArgumentException("End page must be greater than or equal to start page.", nameof(endPage));

            _startPageId = startPage;
            _endPageId = endPage;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="MineruRequest"/> instance.
        /// </summary>
        /// <returns>A new <see cref="MineruRequest"/> instance.</returns>
        public MineruRequest Build()
        {
            return new MineruRequest
            {
                Files = _files.AsReadOnly(),
                OutputDirectory = _outputDirectory,
                LanguageList = _languageList.AsReadOnly(),
                Backend = _backend,
                ParseMethod = _parseMethod,
                FormulaEnable = _formulaEnable,
                TableEnable = _tableEnable,
                ServerUrl = _serverUrl,
                ReturnMarkdown = _returnMarkdown,
                ReturnMiddleJson = _returnMiddleJson,
                ReturnModelOutput = _returnModelOutput,
                ReturnContentList = _returnContentList,
                ReturnImages = _returnImages,
                ResponseFormatZip = _responseFormatZip,
                StartPageId = _startPageId,
                EndPageId = _endPageId,
            };
        }
    }
}
