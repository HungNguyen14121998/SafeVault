using System.Text.RegularExpressions;

public static class ValidationHelpers
{

    public static bool IsValidInput(string input, string allowedSpecialCharacters = "")
    {
        if (string.IsNullOrEmpty(input))
            return false;

        var validCharacters = allowedSpecialCharacters.ToHashSet();

        return input.All(c => char.IsLetterOrDigit(c) || validCharacters.Contains(c));
    }

    // Very basic pattern check for common SQL injection keywords
    private static readonly Regex sqlInjectionRegex = new Regex(
        @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|UNION|--|;|EXEC|XP_|OR|AND)\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static bool IsSafeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return true; // empty input is safe

        // If regex finds suspicious keywords, return false
        return !sqlInjectionRegex.IsMatch(input);
    }

    public static bool IsValidXSSInput(string input)

    {

        if (string.IsNullOrEmpty(input))

            return true;

        //we dont want to allow input with <script or <iframe

        if ((input.ToLower().Contains("“<script”")) || (input.ToLower().Contains("“<iframe”")))
            return false;
        return true;
    }

}