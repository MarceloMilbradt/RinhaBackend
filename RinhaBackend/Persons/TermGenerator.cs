using RinhaBackend.Models;
using System.Buffers;
using System.Text;

namespace RinhaBackend.Persons;

internal static class TermGenerator
{
    private const char SPACE = ' ';

    public static string BuildSearchTerm(Person person)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(person.Apelido);
        stringBuilder.Append(SPACE);
        stringBuilder.Append(person.Nome);
        foreach (var stack in person.Stack)
        {
            stringBuilder.Append(SPACE);
            stringBuilder.Append(stack);
        }
        return stringBuilder.ToString();
    }
    public static ReadOnlySpan<char> BuildSearchField(Person person)
    {
        int totalLength = person.Apelido.Length + person.Nome.Length + 2;
        foreach (var item in person.Stack)
        {
            totalLength += item.Length + 1;
        }

        char[] charArray = ArrayPool<char>.Shared.Rent(totalLength);

        int currentIndex = 0;

        currentIndex = CopyAndAdvance(charArray, currentIndex, person.Apelido);
        currentIndex = CopyAndAdvance(charArray, currentIndex, SPACE);
        currentIndex = CopyAndAdvance(charArray, currentIndex, person.Nome);
        currentIndex = CopyAndAdvance(charArray, currentIndex, SPACE);

        foreach (var item in person.Stack)
        {
            currentIndex = CopyAndAdvance(charArray, currentIndex, item);
            currentIndex = CopyAndAdvance(charArray, currentIndex, SPACE);
        }

        for (int i = 0; i < currentIndex; i++)
        {
            if (char.IsUpper(charArray[i]))
            {
                charArray[i] = char.ToLowerInvariant(charArray[i]);
            }
        }

        ReadOnlySpan<char> result = charArray.AsSpan(0, currentIndex);
        ArrayPool<char>.Shared.Return(charArray);
        return result;
    }

    private static int CopyAndAdvance(char[] targetArray, int startIndex, ReadOnlySpan<char> sourceSpan)
    {
        sourceSpan.CopyTo(targetArray.AsSpan(startIndex));
        return startIndex + sourceSpan.Length;
    }

    private static int CopyAndAdvance(char[] targetArray, int startIndex, char character)
    {
        targetArray[startIndex] = character;
        return startIndex + 1;
    }


}