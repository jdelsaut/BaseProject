using BaseProject.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Linq;

namespace BaseProject.Common.JsonConverters
{
    public class EnumIntoStringAndValidatorConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var namesInEnum = objectType.GetEnumNames();
            if (!namesInEnum.Contains(reader.Value.ToString(), StringComparer.InvariantCultureIgnoreCase))
            {
                throw new NotInEnumException($@"'{reader.Value.ToString()}' is not allowed as a value (values allowed: '{
                    string.Join("', '", namesInEnum)
                }')");
            }

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}
