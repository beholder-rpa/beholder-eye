namespace beholder_eye
{
    using System.Text.Json.Serialization;

    public class AlignRequest
    {
        /// <summary>
        /// Indicates the pixel size.
        /// </summary>
        [JsonPropertyName("pixelSize")]
        public int? PixelSIze
        {
            get;
            set;
        }
    }
}
