namespace beholder_eye
{
    using System.Text.Json.Serialization;

    public class PointerShape
    {
        [JsonPropertyName("type")]
        public int? Type
        {
            get;
            set;
        }

        [JsonPropertyName("height")]
        public int? Height
        {
            get;
            set;
        }

        [JsonPropertyName("width")]
        public int? Width
        {
            get;
            set;
        }

        [JsonPropertyName("pitch")]
        public int? Pitch
        {
            get;
            internal set;
        }

        [JsonPropertyName("hotspotX")]
        public int? HotSpotX
        {
            get;
            set;
        }

        [JsonPropertyName("hotspotY")]
        public int? HotSpotY
        {
            get;
            set;
        }
    }
}
