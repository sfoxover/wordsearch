using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace WordSearch.Models
{
    public class MessageJson
    {
        public string Message { get; set; }
        public object Data { get; set; }

        public MessageJson()
        {
            Data = null;
        }

        public MessageJson(string json)
        {
            Data = null;
            JObject root = JObject.Parse(json);
            if (root.SelectToken("Message", false) != null)
            {
                Message = (string)root["Message"];
            }
            if (root.SelectToken("Data", false) != null)
            {
                Data = (object)root["Data"];
            }
        }

        public MessageJson(string message, object data = null)
        {
            Message = message;
            Data = data;
        }
        
        // convert objects to json string
        public string GetJsonString()
        {
            JObject json =
                   new JObject(
                       new JProperty("Message", Message),
                       new JProperty("Data", Data != null ? JToken.FromObject(Data) : ""));
            string text = json.ToString(Newtonsoft.Json.Formatting.None);
            return text;
        }
    }
}
