using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Rio.API.GeoSpatial
{
    public static class PocoToDictionary
    {
        public static Dictionary<string, TValue> ToDictionary<TValue>(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary;
        }

        public static Dictionary<string, TValue> ToDictionary<TValue>(object obj, string attributesPrefix)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary.ToDictionary(x => attributesPrefix + x.Key, x => x.Value);
        }
    }
}