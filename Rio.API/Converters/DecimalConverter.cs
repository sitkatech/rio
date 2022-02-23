using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rio.API.Converters
{
    public class DecimalConverter : JsonConverter<decimal>
    {
        public DecimalConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType == JsonTokenType.String ? decimal.Parse(reader.GetString()) : reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
    }
}