



namespace NutriGuard.Application.Common.Helpers;



public static class ArabicNormalizer
{
    public static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        return text
            .Trim()
            .ToLower()

            .Replace("أ", "ا")
            .Replace("إ", "ا")
            .Replace("آ", "ا")

            .Replace("ة", "ه")

            .Replace("ى", "ي")

            .Replace("ؤ", "و")
            .Replace("ئ", "ي");
    }
}