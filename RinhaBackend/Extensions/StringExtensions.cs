using System.Text;

namespace System;

public static class StringExtensions
{

    public static string RemoveNullBytes(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        ReadOnlySpan<char> span = [.. input];
        int nullByteCount = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == '\0')
            {
                nullByteCount++;
            }
        }

        if (nullByteCount == 0)
            return input;

        char[] newArray = new char[span.Length - nullByteCount];
        int idx = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] != '\0')
            {
                newArray[idx++] = span[i];
            }
        }

        return new string(newArray);
    }
}
