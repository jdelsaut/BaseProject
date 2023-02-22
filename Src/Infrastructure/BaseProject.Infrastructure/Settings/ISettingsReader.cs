using System;
using System.Collections.Generic;
using System.Text;

namespace BaseProject.Infrastructure.Settings
{
    public interface ISettingsReader
    {
        string ReadSetting(string settingName);
    }
}
