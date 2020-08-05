namespace beholder_eye
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class Image
    {
        [JsonPropertyName("captureTime")]
        public DateTime? CaptureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Base-64 encoded byte array of the image
        /// </summary>
        [JsonPropertyName("data")]
        public string Data
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
