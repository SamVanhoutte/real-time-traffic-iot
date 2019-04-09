using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace TrafficCameraEventGenerator.Configuration.Settings
{
    public interface IConfigurationReader
    {
        T GetConfigValue<T>(string variableName, bool required, T defaultValue = default(T));
    }
}
