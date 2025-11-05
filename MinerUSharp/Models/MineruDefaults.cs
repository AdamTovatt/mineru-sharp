namespace MinerUSharp.Models
{
    /// <summary>
    /// Centralized defaults and constant values used across MinerU models and helpers.
    /// </summary>
    public static class MineruDefaults
    {
        /// <summary>
        /// Default relative directory path where output artifacts are written.
        /// </summary>
        public const string DefaultOutputDirectory = "./output";

        /// <summary>
        /// Default list of language codes used when none are explicitly provided.
        /// </summary>
        public static readonly IReadOnlyList<string> DefaultLanguageList = new[] { "ch" };

        /// <summary>
        /// Default backend engine identifier used for processing.
        /// </summary>
        public const string DefaultBackend = "pipeline";

        /// <summary>
        /// Default parsing strategy used when no method is specified.
        /// </summary>
        public const string DefaultParseMethod = "auto";

        /// <summary>
        /// Indicates whether formula (equation) extraction is enabled by default.
        /// </summary>
        public const bool DefaultFormulaEnable = true;

        /// <summary>
        /// Indicates whether table extraction is enabled by default.
        /// </summary>
        public const bool DefaultTableEnable = true;

        /// <summary>
        /// Indicates whether Markdown output is returned by default.
        /// </summary>
        public const bool DefaultReturnMarkdown = true;

        /// <summary>
        /// Indicates whether intermediate JSON is returned by default.
        /// </summary>
        public const bool DefaultReturnMiddleJson = false;

        /// <summary>
        /// Indicates whether raw model output is returned by default.
        /// </summary>
        public const bool DefaultReturnModelOutput = false;

        /// <summary>
        /// Indicates whether a list of content segments is returned by default.
        /// </summary>
        public const bool DefaultReturnContentList = false;

        /// <summary>
        /// Indicates whether images are returned by default.
        /// </summary>
        public const bool DefaultReturnImages = false;

        /// <summary>
        /// Indicates whether responses are packaged as a ZIP by default.
        /// </summary>
        public const bool DefaultResponseFormatZip = false;

        /// <summary>
        /// Default starting page index (inclusive) when paginating documents.
        /// </summary>
        public const int DefaultStartPageId = 0;

        /// <summary>
        /// Default ending page index (inclusive) when paginating documents.
        /// </summary>
        public const int DefaultEndPageId = 99999;

        /// <summary>
        /// Substring used to detect ZIP media types in content-type values.
        /// </summary>
        public const string ZipMediaTypeSubstring = "zip";
    }
}



