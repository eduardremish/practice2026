using System;
using System.Linq;

namespace task01
{

    public static class StringExtensions
    {

        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            var cleanedChars = input.Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c)).Select(char.ToLower).ToArray();
            if (cleanedChars.Length == 0)
            {
                return false;
            }
            var reversedChars = cleanedChars.Reverse().ToArray();
            return cleanedChars.SequenceEqual(reversedChars);
        }
    }
}
