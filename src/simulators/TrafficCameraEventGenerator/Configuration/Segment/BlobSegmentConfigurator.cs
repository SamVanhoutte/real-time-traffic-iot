using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using TrafficCameraEventGenerator.Configuration.Settings;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace TrafficCameraEventGenerator.Configuration.Segment
{
    public class BlobSegmentConfigurator : ITrafficSegmentConfigurator
    {
        private readonly IConfigurationReader _configurationReader;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public BlobSegmentConfigurator(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        public async Task<TrafficSegmentConfiguration> GetConfiguration()
        {
            var storageAccountConnection = _configurationReader.GetConfigValue<string>("STORAGE_CONNECTION_STRING", true);
            var segmentId = _configurationReader.GetConfigValue<string>("SEGMENT_ID", true);

            var storageAccount = CloudStorageAccount.Parse(storageAccountConnection);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var configBlob = blobClient.GetContainerReference("traffic-config").GetBlockBlobReference("segment-configs.json");
            var configText = await configBlob.DownloadTextAsync();
            var segmentConfigs = JsonConvert.DeserializeObject<List<TrafficSegmentConfiguration>>(configText);

            return segmentConfigs.FirstOrDefault(c =>
                c.SegmentId.Equals(segmentId, StringComparison.InvariantCultureIgnoreCase));
        }

    }
}
