using Microsoft.Extensions.Configuration;
using System;

namespace BoxApi.Infrastructure.Settings
{
    public class SettingsReader : ISettingsReader
    {

        public string ReadSetting(string settingName)
        {
            string val = RetrieveFromLocalSettings(settingName);

            if (string.IsNullOrEmpty(val))
                val = Environment.GetEnvironmentVariable(settingName, EnvironmentVariableTarget.Process);

            return val;
        }

        private static string RetrieveFromLocalSettings(string settingName)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("system.settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var val = config[$"Values:{settingName}"];
            return val;
        }
    }
}