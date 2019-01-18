using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;
using Savanh.Extensions.Randoms;
namespace TrafficCamera.Tests
{
    public class RandomExtensionTests
    {
        [Fact]
        public void TestGaussianDistribution()
        {
            Random rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                var val = rnd.NextTriangular(10, 30, 12);
                Debug.WriteLine($"New value generated: {val}");
                Assert.True(val >= 10);
                Assert.True(val <= 30);
            }

        }

        [Fact]
        public void TestProbabilityBoolean()
        {
            Random rnd = new Random();
            List<bool> values = new List<bool>();
            int probability = 15;
            for (int i = 0; i < 100; i++)
            {
                values.Add(rnd.GetBooleanWithProbability(probability));
            }

            int trueCount = 0;
            foreach (var b in values)
            {
                if (b) trueCount += 1;
            }
            Assert.True(trueCount < probability * 1.5);
        }
    }
}
