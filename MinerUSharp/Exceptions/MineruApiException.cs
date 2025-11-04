using MinerUSharp.Models;
using System.Net;

namespace MinerUSharp.Exceptions
{
    /// <summary>
    /// Exception thrown when the MinerU API returns an error.
    /// </summary>
    public sealed class MineruApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code of the error response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the validation errors returned by the API, if any.
        /// </summary>
        public IReadOnlyList<ValidationError>? ValidationErrors { get; }

        /// <summary>
        /// Gets the raw response content.
        /// </summary>
        public string? ResponseContent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MineruApiException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="responseContent">The raw response content.</param>
        /// <param name="validationErrors">The validation errors, if any.</param>
        /// <param name="innerException">The inner exception, if any.</param>
        public MineruApiException(
            string message,
            HttpStatusCode statusCode,
            string? responseContent = null,
            IReadOnlyList<ValidationError>? validationErrors = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
            ValidationErrors = validationErrors;
        }
    }
}

