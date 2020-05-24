namespace Backend.Minio.Manager.Helpers.Extension
{
    using System.Text.RegularExpressions;

    public static class UtilsExtensions
    {
        public static string FormatText(this string self, params object[] param)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return string.Empty;
            }

            return string.Format(self, param);
        }

        public static string SanitizeString(this string self)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                return string.Empty;
            }

            return Regex.Replace(self, @"[^0-9a-zA-Z-_]+", string.Empty)
                .Replace("_", "-")
                .Trim('-');
        }

        public static string NormalizeString(this string self)
        {
            return self.SanitizeString().ToLower();
        }
    }
}
