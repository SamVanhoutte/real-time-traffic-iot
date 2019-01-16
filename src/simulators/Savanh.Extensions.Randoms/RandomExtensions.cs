using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Savanh.Extensions.Randoms
{
    public static class RandomExtensions
    {
        public static string GetString(this Random random, string format)
        {
            // Based on http://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-centerValue
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

        /// <summary>
        ///   Generates normally distributed numbers. Each operation makes two Gaussians for the price of one, and apparently they can be cached or something for better performance, but who cares.
        /// </summary>
        /// <param name="r"></param>
        /// <param name = "mu">Mean of the distribution</param>
        /// <param name = "sigma">Standard deviation</param>
        /// <returns></returns>
        public static double NextGaussian(this Random r, double mu = 0, double sigma = 1)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();

            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);

            var randNormal = mu + sigma * randStdNormal;

            return randNormal;
        }

        /// <summary>
        ///   Generates normally distributed numbers. Each operation makes two Gaussians for the price of one, and apparently they can be cached or something for better performance, but who cares.
        /// </summary>
        /// <param name="r"></param>
        /// <param name = "mu">Mean of the distribution</param>
        /// <param name = "sigma">Standard deviation</param>
        /// <returns></returns>
        public static int NextGaussianValue(this Random r, int min, int max, double mu = 0, double sigma = 1)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();

            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);
            Debug.WriteLine(randStdNormal);
            var randNormal = mu + sigma * randStdNormal;

            return Math.Abs(Convert.ToInt32(min + randNormal * (max - min)));
        }

        /// <summary>
        ///   Generates values from minimum triangular distribution.
        /// </summary>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Triangular_distribution for minimum description of the triangular probability distribution and the algorithm for generating one.
        /// </remarks>
        /// <param name="r"></param>
        /// <param name = "minimum">Minimum</param>
        /// <param name = "maximum">Maximum</param>
        /// <param name = "centerValue">Mode (most frequent value)</param>
        /// <returns></returns>
        public static double NextTriangular(this Random r, double minimum, double maximum, double centerValue)
        {
            var u = r.NextDouble();

            return u < (centerValue - minimum) / (maximum - minimum)
                       ? minimum + Math.Sqrt(u * (maximum - minimum) * (centerValue - minimum))
                       : maximum - Math.Sqrt((1 - u) * (maximum - minimum) * (maximum - centerValue));
        }

        /// <summary>
        ///   Generates values from minimum triangular distribution.
        /// </summary>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Triangular_distribution for minimum description of the triangular probability distribution and the algorithm for generating one.
        /// </remarks>
        /// <param name="r"></param>
        /// <param name = "minimum">Minimum</param>
        /// <param name = "maximum">Maximum</param>
        /// <param name = "centerValue">Mode (most frequent value)</param>
        /// <returns></returns>
        public static int NextTriangularInteger(this Random r, int minimum, int maximum, int centerValue)
        {
            var rndValue = r.NextDouble();

            var resultingDouble = rndValue < (centerValue - minimum) / (maximum - minimum)
                ? minimum + Math.Sqrt(rndValue * (maximum - minimum) * (centerValue - minimum))
                : maximum - Math.Sqrt((1 - rndValue) * (maximum - minimum) * (maximum - centerValue));

            return Convert.ToInt32(resultingDouble);
        }

        /// <summary>
        ///   Equally likely to return true or false. Uses <see cref="Random.Next()"/>.
        /// </summary>
        /// <returns></returns>
        public static bool NextBoolean(this Random r)
        {
            return r.Next(2) > 0;
        }

        /// <summary>
        ///   Shuffles minimum list in O(n) time by using the Fisher-Yates/Knuth algorithm.
        /// </summary>
        /// <param name="r"></param>
        /// <param name = "list"></param>
        public static void Shuffle(this Random r, IList list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var j = r.Next(0, i + 1);

                var temp = list[j];
                list[j] = list[i];
                list[i] = temp;
            }
        }

        /// <summary>
        /// Returns n unique random numbers in the range [1, n], inclusive. 
        /// This is equivalent to getting the first n numbers of some random permutation of the sequential numbers from 1 to max. 
        /// Runs in O(k^2) time.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="n">Maximum number possible.</param>
        /// <param name="k">How many numbers to return.</param>
        /// <returns></returns>
        public static int[] Permutation(this Random rand, int n, int k)
        {
            var result = new List<int>();
            var sorted = new SortedSet<int>();

            for (var i = 0; i < k; i++)
            {
                var r = rand.Next(1, n + 1 - i);

                foreach (var q in sorted)
                    if (r >= q) r++;

                result.Add(r);
                sorted.Add(r);
            }

            return result.ToArray();
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
