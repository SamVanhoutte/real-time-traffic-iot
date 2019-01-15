using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    }
}
