using BaseProject.Common.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BaseProject.Common.SerializerSettings
{
    public class DeserializeObjectSettings
    {
        public Dictionary<string, string> DeserializeErrors
        {
            get;
        }

        private JsonSerializerSettings _deserializeSettings;

        public JsonSerializerSettings DeserializeJsonSettings
        {
            get
            {
                if (_deserializeSettings == null)
                {
                    _deserializeSettings = new JsonSerializerSettings();
                    _deserializeSettings.Error += (
                        (x, e) =>
                        {
                            if (e.ErrorContext.Error is NotInEnumException)
                            {
                                DeserializeErrors.Add(e.ErrorContext.Path, e.ErrorContext.Error.Message);
                            }
                            else
                            {
                                DeserializeErrors.Add(e.ErrorContext.Path, "expected format is not correct");
                            }

                            e.ErrorContext.Handled = true;
                        }
                    );
                }

                return _deserializeSettings;
            }
        }

        public DeserializeObjectSettings()
        {
            DeserializeErrors = new Dictionary<string, string>();
        }
    }
}
