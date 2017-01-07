using System.Collections.Generic;
using Newtonsoft.Json;

namespace Application.Infrastructure.Helpers
{
    public class JsonHelpers
    {
        public static IEnumerable<T> DeserializeIEnumerable<T>(string value)
        {
            return string.IsNullOrEmpty(value) ? new List<T>() : JsonConvert.DeserializeObject<IEnumerable<T>>(value);
        }
    }
}
