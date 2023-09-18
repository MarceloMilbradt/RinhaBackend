using System.Text;

namespace System;

public static class StringExtensions
{
    public static string RemoveNullBytes(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (c != '\0')
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
