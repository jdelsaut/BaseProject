using Newtonsoft.Json;
using System.Collections.Generic;

namespace BoxApi.Common.SerializerSettings
{
    public class SerializeObjectSettings
    {
        public Dictionary<string, string> SerializeErrors
        {
            get;
        }

        private JsonSerializerSettings _serializeSettings;

        public JsonSerializerSettings SerializeJsonSettings
        {
            get
            {
                if (_serializeSettings == null)
                {
                    _serializeSettings = new JsonSerializerSettings();
                    _serializeSettings.Error += (
                        (x, e) =>
                        {
                            if (!SerializeErrors.ContainsKey(e.ErrorContext.Path))
                            {
                                SerializeErrors.Add(e.ErrorContext.Path, "could not be serialized.");
                            }

                            e.ErrorContext.Handled = true;
                        }
                    );
                }

                return _serializeSettings;
            }
        }

        public SerializeObjectSettings()
        {
            SerializeErrors = new Dictionary<string, string>();
        }
    }
}
