using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace TrafficCameraEventGenerator.Configuration.Settings
{
    public class HardcodedConfigurationReader : IConfigurationReader
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDictionary<string, string> _settings = new Dictionary<string, string>
        {
            { "IOTHUB_CAMERA1_CONNECTION", "HostName=savanh-traffic-camera.azure-devices.net;DeviceId=dev-camera-01;SharedAccessKey=nSWtl/1yw/cXebzMAsLalHyzFoBx6nBXCIMe5AzzgcY=" },
            { "IOTHUB_CAMERA2_CONNECTION", "HostName=savanh-traffic-camera.azure-devices.net;DeviceId=dev-camera-02;SharedAccessKey=ZhMFyC8rdAJuIoVegye0OUFfPZep909qBq+hv4Gkkrg=" },
            { "SEGMENT_LANE_COUNT", "4" },
            { "SEGMENT_AVG_CARS_PER_MINUTE", "70" },
            { "SEGMENT_SPEED_LIMIT", "130" },
            { "SEGMENT_CAMERA_DISTANCE", "1800" },
            { "SEGMENT_SPEEDING_PERCENTAGE", "3" },
            { "SEGMENT_MIN_SPEED", "9" },
            { "SEGMENT_MAX_SPEED", "188" },
            { "SEGMENT_ID", "9" },
        };

        public T GetConfigValue<T>(string variableName, bool required, T defaultValue = default(T))
        {
            string variableValue = _settings.ContainsKey(variableName) ? _settings[variableName] : null;
            if (variableValue == null)
            {
                _logger.Trace($"Variable {variableName} not found, defaulting to {defaultValue}");
                return defaultValue;
            }
            else
            {
                _logger.Trace($"Variable {variableName} found, and valued to {variableValue}");
                return (T)Convert.ChangeType(variableValue, typeof(T));
            }
        }
    }
}
