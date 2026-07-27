using System.Text;

namespace NutriGuard.Application.Common.Helpers;

public static class ArabicNormalizer
{
    public static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        text = text.Trim();

        text = text
            .Replace('أ', 'ا')
            .Replace('إ', 'ا')
            .Replace('آ', 'ا')
            .Replace('ى', 'ي')
            .Replace('ة', 'ه')
            .Replace('ؤ', 'و')
            .Replace('ئ', 'ي');

        var sb = new StringBuilder();

        foreach (var c in text)
        {
            if (c >= '\u064B' && c <= '\u065F')
                continue;

            sb.Append(c);
        }

        return sb.ToString();
    }
}