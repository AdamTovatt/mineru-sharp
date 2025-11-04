using MinerUSharp.Models;

namespace MinerUSharp.Internal
{
    /// <summary>
    /// Helper class for building multipart/form-data content for API requests.
    /// </summary>
    internal static class MultipartFormDataHelper
    {
        /// <summary>
        /// Creates multipart form data content from a MinerU request.
        /// </summary>
        /// <param name="request">The request to convert.</param>
        /// <returns>A <see cref="MultipartFormDataContent"/> instance.</returns>
        public static MultipartFormDataContent CreateContent(MineruRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            MultipartFormDataContent content = new MultipartFormDataContent();

            // Add files
            int fileIndex = 0;
            foreach (Stream fileStream in request.Files)
            {
                StreamContent fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                content.Add(fileContent, "files", $"file{fileIndex++}");
            }

            // Add other parameters
            content.Add(new StringContent(request.OutputDirectory), "output_dir");

            foreach (string language in request.LanguageList)
            {
                content.Add(new StringContent(language), "lang_list");
            }

            content.Add(new StringContent(request.Backend), "backend");
            content.Add(new StringContent(request.ParseMethod), "parse_method");
            content.Add(new StringContent(request.FormulaEnable.ToString().ToLowerInvariant()), "formula_enable");
            content.Add(new StringContent(request.TableEnable.ToString().ToLowerInvariant()), "table_enable");

            if (request.ServerUrl != null)
            {
                content.Add(new StringContent(request.ServerUrl), "server_url");
            }

            content.Add(new StringContent(request.ReturnMarkdown.ToString().ToLowerInvariant()), "return_md");
            content.Add(new StringContent(request.ReturnMiddleJson.ToString().ToLowerInvariant()), "return_middle_json");
            content.Add(new StringContent(request.ReturnModelOutput.ToString().ToLowerInvariant()), "return_model_output");
            content.Add(new StringContent(request.ReturnContentList.ToString().ToLowerInvariant()), "return_content_list");
            content.Add(new StringContent(request.ReturnImages.ToString().ToLowerInvariant()), "return_images");
            content.Add(new StringContent(request.ResponseFormatZip.ToString().ToLowerInvariant()), "response_format_zip");
            content.Add(new StringContent(request.StartPageId.ToString()), "start_page_id");
            content.Add(new StringContent(request.EndPageId.ToString()), "end_page_id");

            return content;
        }
    }
}
