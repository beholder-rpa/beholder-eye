namespace beholder_eye
{
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SnapshotFormat
    {
        Png,
        Jpeg,
    }
}
