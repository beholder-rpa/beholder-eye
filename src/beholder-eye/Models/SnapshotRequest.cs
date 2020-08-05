namespace beholder_eye
{
    using System.Text.Json.Serialization;

    public class SnapshotRequest
    {
        /// <summary>
        /// Indicates the scale factor of the thumbnail. Defaults to 1 (100% of original screen size)
        /// </summary>
        [JsonPropertyName("scaleFactor")]
        public double? ScaleFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the format of the snapshot. Defaults to Png
        /// </summary>
        [JsonPropertyName("format")]
        public SnapshotFormat? Format
        {
            get;
            set;
        }

        /// <summary>
        /// Metadata to store with the snapshot.
        /// </summary>
        [JsonPropertyName("metadata")]
        public object Metadata
        {
            get;
            set;
        }
    }
}
