namespace MinerUSharp.Tests.TestHelpers
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Normalizes line endings in a string to Unix-style line endings (\n) for consistent comparison in tests.
        /// </summary>
        /// <param name="value">The string to normalize.</param>
        /// <returns>The string with normalized line endings.</returns>
        public static string NormalizeLineEndings(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Replace("\r\n", "\n").Replace("\r", "\n");
        }
    }
}

