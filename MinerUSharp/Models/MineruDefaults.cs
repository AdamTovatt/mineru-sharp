namespace MinerUSharp.Models
{
    /// <summary>
    /// Centralized defaults and constant values used across MinerU models and helpers.
    /// </summary>
    public static class MineruDefaults
    {
        public const string DefaultOutputDirectory = "./output";

        public static readonly IReadOnlyList<string> DefaultLanguageList = new[] { "ch" };

        public const string DefaultBackend = "pipeline";

        public const string DefaultParseMethod = "auto";

        public const bool DefaultFormulaEnable = true;

        public const bool DefaultTableEnable = true;

        public const bool DefaultReturnMarkdown = true;

        public const bool DefaultReturnMiddleJson = false;

        public const bool DefaultReturnModelOutput = false;

        public const bool DefaultReturnContentList = false;

        public const bool DefaultReturnImages = false;

        public const bool DefaultResponseFormatZip = false;

        public const int DefaultStartPageId = 0;

        public const int DefaultEndPageId = 99999;

        public const string ZipMediaTypeSubstring = "zip";
    }
}

