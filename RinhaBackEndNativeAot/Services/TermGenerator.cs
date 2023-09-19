using RinhaBackEndNativeAot.Models;
using System.Buffers;
using System.Text;

namespace RinhaBackEndNativeAot.Services;

internal static class TermGenerator
{
    private const char SPACE = ' ';
    private const char NULL = '\0';
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