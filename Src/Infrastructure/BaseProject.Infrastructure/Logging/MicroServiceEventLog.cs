using BaseProject.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseProject.Infrastructure.Logging
{
    public class MicroServiceEventLog : Dictionary<string, string>
    {
        public MicroServiceEventLog(string eventName, string functionName, string BaseProjectId, string level, string message, ISettingsReader settings)
            : base()
        {
            this["name"] = eventName;
            this["microservice"] = "BaseProject";
            this["functionName"] = functionName;
            this["version"] = settings.ReadSetting(AppSettingsKeys.ApiVersion);
            this["BaseProjectId"] = BaseProjectId;
            this["level"] = level;
            this["message"] = message;
        }

        public string Name => this["name"];
        public string Microservice => this["microservice"];
        public string FunctionName => this["functionName"];
        public string Version => this["version"];
        public string BaseProjectId => this["BaseProjectId"];
        public string Level => this["level"];
        public string Message => this["message"];
    }
}
