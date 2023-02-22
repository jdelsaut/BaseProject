using BoxApi.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxApi.Infrastructure.SmartID
{
    public class SmartIDGenerator : ISmartIDGenerator
    {
        private const string apiVersionParameterName = AppSettingsKeys.ApiVersion;
        private readonly ISettingsReader settingsReader;

        public SmartIDGenerator(ISettingsReader settingsReader)
        {
            this.settingsReader = settingsReader;
        }

        public string Generate(Guid id)
        {
            string version = settingsReader.ReadSetting(apiVersionParameterName);
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException(AppSettingsKeys.ApiVersion);

            return $"{version}-{id}";
        }

        public string Generate(string id)
        {
            string version = settingsReader.ReadSetting(apiVersionParameterName);
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException(AppSettingsKeys.ApiVersion);

            return $"{version}-{id}";
        }
    }
}
