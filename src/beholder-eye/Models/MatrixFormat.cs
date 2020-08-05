namespace beholder_eye
{
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataMatrixFormat
    {
        Raw,
        Hex,
        Text,
        TextGrid,
        Json,
        MatrixEvents
    }
}
