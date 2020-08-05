namespace beholder_eye
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class MatrixSettings
    {
        /// <summary>
        /// An ordered list of points whose values will indicate the data values.
        /// </summary>
        [JsonPropertyName("map")]
        public IList<int> Map
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the index of the pixel that will be used as the frame id. (little endian)  (Default: 0)
        /// </summary>
        [JsonPropertyName("frameIdIndex")]
        public int? FrameIdIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the index of the pixel that will be used for metadata - R - Width, G - Height, B - Pixel Size (Default: 1)
        /// </summary>
        [JsonPropertyName("useFrameMetadata")]
        public int? FrameMetadataIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the format that the datamatrix will decode into and ultimately contain (default: Json)
        /// </summary>
        [JsonPropertyName("dataFormat")]
        public DataMatrixFormat? DataFormat
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
