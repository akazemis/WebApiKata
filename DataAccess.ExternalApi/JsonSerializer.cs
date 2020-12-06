using DataAccess.Interfaces;
using Newtonsoft.Json;

namespace DataAccess.ExternalApi
{
    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string serializedObject)
        {
            return JsonConvert.DeserializeObject<T>(serializedObject);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
