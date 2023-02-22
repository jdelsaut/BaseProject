using BoxApi.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoxApi.Infrastructure.Logging
{
    public class MicroServiceEventLog : Dictionary<string, string>
    {
        public MicroServiceEventLog(string eventName, string functionName, string BoxApiId, string level, string message, ISettingsReader settings)
            : base()
        {
            this["name"] = eventName;
            this["microservice"] = "BoxApi";
            this["functionName"] = functionName;
            this["version"] = settings.ReadSetting(AppSettingsKeys.ApiVersion);
            this["BoxApiId"] = BoxApiId;
            this["level"] = level;
            this["message"] = message;
        }

        public string Name => this["name"];
        public string Microservice => this["microservice"];
        public string FunctionName => this["functionName"];
        public string Version => this["version"];
        public string BoxApiId => this["BoxApiId"];
        public string Level => this["level"];
        public string Message => this["message"];
    }
}
