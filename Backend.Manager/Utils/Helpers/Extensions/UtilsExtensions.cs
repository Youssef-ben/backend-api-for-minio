namespace Backend.Minio.Manager.Helpers.Extension
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    public static class UtilsExtensions
    {
        /// <summary>
        /// Format the Given string with the specified values.
        /// </summary>
        /// <param name="self">THe string to be formatted.</param>
        /// <param name="param">The string values as an array.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatText([NotNull]this string self, [NotNull]params object[] param)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return string.Empty;
            }

            return string.Format(self, param);
        }

        /// <summary>
        /// Sanitize the string based on the Minio requirement for naming
        /// the buckets.
        /// </summary>
        /// <param name="self">The bucket name.</param>
        /// <returns>The sanitized bucket name.</returns>
        public static string SanitizeString([NotNull]this string self)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return string.Empty;
            }

            return Regex.Replace(self, @"[^0-9a-zA-Z-_]+", string.Empty)
                .Replace("_", "-")
                .Trim('-');
        }

        /// <summary>
        /// Sanitize and format the string to lower case value based on the Minio
        /// requirement for naming the buckets.
        /// </summary>
        /// <param name="self">The bucket name.</param>
        /// <returns>the normalized bucket name.</returns>
        public static string NormalizeString([NotNull]this string self)
        {
            return self
                .SanitizeString()
                .ToLower();
        }
    }
}
