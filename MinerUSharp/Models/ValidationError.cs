namespace MinerUSharp.Models
{
    /// <summary>
    /// Represents a validation error from the MinerU API.
    /// </summary>
    public sealed class ValidationError
    {
        /// <summary>
        /// Gets or sets the location of the error.
        /// </summary>
        public IReadOnlyList<object> Location { get; set; } = Array.Empty<object>();

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error type.
        /// </summary>
        public string ErrorType { get; set; } = string.Empty;
    }
}

