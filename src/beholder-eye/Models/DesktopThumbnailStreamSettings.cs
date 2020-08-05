namespace beholder_eye
{
    using System.Text.Json.Serialization;

    public class DesktopThumbnailStreamSettings
    {
        public DesktopThumbnailStreamSettings()
        {
            MaxFps = 0.5;
            ScaleFactor = 0.15;
        }

        /// <summary>
        /// Indicates the maximum thumbnails that will be sent per second. Defaults to 0.5
        /// </summary>
        [JsonPropertyName("maxFps")]
        public double? MaxFps
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the scale factor of the thumbnail. Defaults to 0.15 (15% of original screen size)
        /// </summary>
        [JsonPropertyName("scaleFactor")]
        public double? ScaleFactor
        {
            get;
            set;
        }
    }
}
