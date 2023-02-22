using System;
using System.Collections.Generic;
using System.Text;

namespace BoxApi.Infrastructure.Settings
{
    public interface ISettingsReader
    {
        string ReadSetting(string settingName);
    }
}
