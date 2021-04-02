using System.IO;
using Newtonsoft.Json;

namespace SertifiTestApi.Tests.Utils
{
    public class JsonUtils
    {
        public static T DeserializeByPath<T>(string fileName)
        {
            var path = Path.Combine("TestData", fileName);
            using var r = new StreamReader(path);
            var json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
