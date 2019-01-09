using System;
using System.Text;

namespace Savanh.Extensions.Randoms
{
    public static class RandomExtensions
    {
        public static string GetString(this Random random, string format)
        {
            // Based on http://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
            // Added logic to specify the format of the random string (# will be random string, 0 will be random numeric, other characters remain)
            StringBuilder result = new StringBuilder();
            for (int formatIndex = 0; formatIndex < format.Length; formatIndex++)
            {
                switch (format.ToUpper()[formatIndex])
                {
                    case '0': result.Append(GetRandomNumeric(random)); break;
                    case '#': result.Append(GetRandomCharacter(random)); break;
                    default: result.Append(format[formatIndex]); break;
                }
            }
            return result.ToString();
        }

        private static char GetRandomCharacter(Random random)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return chars[random.Next(chars.Length)];
        }

        private static char GetRandomNumeric(System.Random random)
        {
            string nums = "0123456789";
            return nums[random.Next(nums.Length)];
        }
    }
}
