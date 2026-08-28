using System;

namespace BaldiPlusRandomZone.Extensions
{
    public static class LevelNameGenerator
    {
        private static readonly string[] consonants = { "b","c","d","f","g","h","j","k","l","m","n","p","r","s","t","v","x","z" };

        private static readonly string[] vowels = { "a","e","i","o","u" };

        private static readonly string[] suffixes = { "um","at","or","us","ix","ar","en","is","on","ex","ax","be","ui","ol","op","ga","wa","im","ow","ew","oy","ay","ey","ew","ci","oz","aw","eg","oi","oa" };

        private static readonly string[] bannedWords =
        {
            "nazi",
            "niga",
        };

        public static string GenerateLevelName(Random random)
        {
            bool isSafe = false;

            while (!isSafe)
            {
                int syllables = random.Next(2, 6);
                string word = "";

                for (int i = 0; i < syllables; i++)
                {
                    string c = consonants[random.Next(consonants.Length)];
                    string v = vowels[random.Next(vowels.Length)];

                    word += c + v;
                }

                if ((float)random.NextDouble() > 0.5f)
                    word += suffixes[random.Next(suffixes.Length)];

                isSafe = IsSafe(char.ToUpper(word[0]) + word.Substring(1));
                return char.ToUpper(word[0]) + word.Substring(1);
            }

            return "";
        }

        private static bool IsSafe(string word)
        {
            string lower = word.ToLowerInvariant();

            foreach (string banned in bannedWords)
            {
                if (lower.Contains(banned))
                    return false;
            }

            return true;
        }
    }
}
