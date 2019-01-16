using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrafficCameraEventGenerator.Configuration.Segment;
using TrafficCameraEventGenerator.Configuration.Settings;
using Xunit;

namespace TrafficCamera.Tests
{
    public class BlobSegmentConfiguratorTests
    {
        [Fact]
        public async Task BlobSegmentConfigurator_ShouldSucceed()
        {
            ITrafficSegmentConfigurator configurator = new BlobSegmentConfigurator(new HardcodedConfigurationReader());
            var configuration = await configurator.GetConfiguration();
            Assert.NotNull(configuration);
            Assert.Equal(90, configuration.SpeedLimit);
        }
    }
}
