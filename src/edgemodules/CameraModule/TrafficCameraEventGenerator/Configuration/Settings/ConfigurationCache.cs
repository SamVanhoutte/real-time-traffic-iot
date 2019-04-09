using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficCameraEventGenerator.Configuration.Settings
{
    public static class ConfigurationCache
    {
        private static readonly Dictionary<string, string> _values = new Dictionary<string, string>();
        public static void CacheValue(string variableName, string value)
        {
            if (_values.ContainsKey(variableName))
            {
                _values[variableName] = value;
            }
            else
            {
                _values.Add(variableName, value);
            }
        }

        public static string GetValue(string variableName)
        {
            return _values[variableName] ?? "";
        }
    }
}
