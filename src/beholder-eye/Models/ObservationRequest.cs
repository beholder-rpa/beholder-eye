namespace beholder_eye
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class ObservationRequest
    {
        [JsonPropertyName("adapterIndex")]
        public int? AdapterIndex
        {
            get;
            set;
        }

        [JsonPropertyName("deviceIndex")]
        public int? DeviceIndex
        {
            get;
            set;
        }

        [JsonPropertyName("regions")]
        public IList<ObservationRegion> Regions
        {
            get;
            set;
        }

        [JsonPropertyName("streamDesktopThumbnail")]
        public bool? StreamDesktopThumbnail
        {
            get;
            set;
        }

        [JsonPropertyName("desktopThumbnailStreamSettings")]
        public DesktopThumbnailStreamSettings DesktopThumbnailStreamSettings
        {
            get;
            set;
        }

        [JsonPropertyName("streamPointerPosition")]
        public bool? StreamPointerPosition
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
