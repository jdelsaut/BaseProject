using BaseProject.Common.SerializerSettings;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace BaseProject.Common.CustomSerializer
{
    public static class CustomSerializer<T>
    {
        public static T Deserialize(string jsonToDeserialize)
        {
            DeserializeObjectSettings deserializeObjectSettings = new DeserializeObjectSettings();

            return Deserializing(jsonToDeserialize, deserializeObjectSettings);
        }

        public static T Deserialize(string jsonToDeserialize, IContractResolver contractToUse)
        {
            DeserializeObjectSettings deserializeObjectSettings = new DeserializeObjectSettings();
            deserializeObjectSettings.DeserializeJsonSettings.ContractResolver = contractToUse;

            return Deserializing(jsonToDeserialize, deserializeObjectSettings);
        }

        public static string Serialize(T objectToSerialize)
        {
            return Serializing(objectToSerialize, new SerializeObjectSettings());
        }

        public static string Serialize(T objectToSerialize, IContractResolver contractToUse)
        {
            SerializeObjectSettings serializeObjectSettings = new SerializeObjectSettings();
            serializeObjectSettings.SerializeJsonSettings.ContractResolver = contractToUse;

            return Serializing(objectToSerialize, serializeObjectSettings);
        }

        private static string Serializing(T objectToSerialize, SerializeObjectSettings serializeObjectSettings)
        {
            string serializedObject = JsonConvert.SerializeObject(objectToSerialize, serializeObjectSettings.SerializeJsonSettings);
            CustomSerializer<T>.HandleErrors(serializeObjectSettings.SerializeErrors);

            return serializedObject;
        }

        private static T Deserializing(string jsonToDeserialize, DeserializeObjectSettings deserializeObjectSettings)
        {
            T deserializedObject = JsonConvert.DeserializeObject<T>(jsonToDeserialize, deserializeObjectSettings.DeserializeJsonSettings);
            CustomSerializer<T>.HandleErrors(deserializeObjectSettings.DeserializeErrors);

            return deserializedObject;
        }

        private static void HandleErrors(IDictionary<string, string> errors)
        {
            if (errors.Count > 0)
            {
                throw new JsonSerializationException(
                    $@"{errors.Select(x => $"'{x.Key}' {x.Value}").Aggregate((x, y) =>
                       $"{x}, {y}"
                    )}."
                );
            }
        }
    }
}
