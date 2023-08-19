using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PentaGE.Core.Serialization
{
    internal class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("X", out JsonElement xElement) &&
                    root.TryGetProperty("Y", out JsonElement yElement) &&
                    root.TryGetProperty("Z", out JsonElement zElement))
                {
                    float x = xElement.GetSingle();
                    float y = yElement.GetSingle();
                    float z = zElement.GetSingle();
                    return new Vector3(x, y, z);
                }
            }

            throw new JsonException("Invalid Vector3 format.");
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteNumber("Z", value.Z);
            writer.WriteEndObject();
        }
    }
}