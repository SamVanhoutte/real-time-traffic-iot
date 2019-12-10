using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace TrafficCameraEventGenerator.Configuration.Settings
{
    public class EnvironmentConfigurationReader : IConfigurationReader
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public T GetConfigValue<T>(string variableName, bool required, T defaultValue = default(T))
        {
            string variableValue = Environment.GetEnvironmentVariable(variableName);
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

        public void CacheValue<T>(string variableName, T value)
        {
            throw new NotImplementedException();
        }
    }
}
