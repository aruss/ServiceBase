using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace ServiceBase.Xunit
{
    public class JsonDataAttribute : DataAttribute
    {
        private string path;

        public JsonDataAttribute(string path)
            : base()
        {
            this.path = path;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<object[][]>(json);
            var types = testMethod.GetParameters();
            var result = new List<object[]>();

            foreach (var dataRow in data)
            {
                var resultRow = new object[types.Length];
                result.Add(resultRow);

                for (int i = 0; i < dataRow.Length; i++)
                {
                    if (types.Length > i)
                    {
                        var dataCell = dataRow[i];

                        if (dataCell is JObject)
                        {
                            resultRow[i] = (dataCell as JObject).ToObject(types[i].ParameterType);
                        }
                        else
                        {
                            resultRow[i] = dataCell;
                        }
                    }
                }
            }

            return result;
        }
    }
}
