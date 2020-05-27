using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FBS_Telemetry_Viewer
{
    /// <summary>
    /// Collect all fields of JSON object.
    /// https://riptutorial.com/csharp/example/32164/
    /// </summary>
    class JsonFieldsCollector
    {
        private readonly Dictionary<string, JValue> fields;

        public JsonFieldsCollector(JToken token)
        {
            this.fields = new Dictionary<string, JValue>();
            CollectFields(token);
        }

        private void CollectFields(JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty child in jToken.Children<JProperty>())
                        CollectFields(child);
                    break;
                case JTokenType.Array:
                    foreach (JToken child in jToken.Children())
                        CollectFields(child);
                    break;
                case JTokenType.Property:
                    CollectFields(((JProperty)jToken).Value);
                    break;
                default:
                    this.fields.Add(jToken.Path, (JValue)jToken);
                    break;
            }
        }

        public IEnumerable<KeyValuePair<string, JValue>> GetAllFields() => this.fields;
    }
}
