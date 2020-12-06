using WebApiKata.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApiKata.Services
{
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public JsonSerializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public T Deserialize<T>(string serializedObject)
        {
            return JsonConvert.DeserializeObject<T>(serializedObject, _jsonSerializerSettings);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        }
    }
}
