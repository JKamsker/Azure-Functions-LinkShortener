using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkShortener.StringUtils
{
    public class RandomEx
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            // ReSharper disable once StringLiteralTypo
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //abcdefghijklmnopqrstuvwxyz
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
