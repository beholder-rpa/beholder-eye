namespace beholder_eye
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class MatrixEvent
    {
        [JsonPropertyName("t")]
        public string Topic
        {
            get;
            set;
        }

        [JsonPropertyName("et")]
        public DateTime? EventTime
        {
            get;
            set;
        }

        [JsonPropertyName("d")]
        public object Data
        {
            get;
            set;
        }

        [JsonPropertyName("p")]
        public string Priority
        {
            get;
            set;
        }

        [JsonExtensionData]
        public IDictionary<string, JsonElement> AdditionalData
        {
            get;
            set;
        }
    }
}
