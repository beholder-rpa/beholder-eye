namespace beholder_eye
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class ImageSettings
    {
        [JsonPropertyName("x")]
        public int X
        {
            get;
            set;
        }

        [JsonPropertyName("y")]
        public int Y
        {
            get;
            set;
        }

        [JsonPropertyName("width")]
        public int Width
        {
            get;
            set;
        }

        [JsonPropertyName("height")]
        public int Height
        {
            get;
            set;
        }

        [JsonPropertyName("maxFPS")]
        public int MaxFPS
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
