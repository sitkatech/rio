using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rio.API.Converters
{
    public class NullableConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => Nullable.GetUnderlyingType(typeToConvert) != null;

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options) =>
            (JsonConverter)Activator.CreateInstance(
                typeof(NullableConverter<>).MakeGenericType(
                    new Type[] { Nullable.GetUnderlyingType(type) }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

        class NullableConverter<T> : JsonConverter<T?> where T : struct
        {
            private readonly JsonConverter<T> _valueConverter;

            public NullableConverter(JsonSerializerOptions options) => _valueConverter = (JsonConverter<T>)options.GetConverter(typeof(T));

            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;
                if (reader.TokenType == JsonTokenType.String)
                {
                    var s = reader.GetString();
                    if (string.IsNullOrEmpty(s))
                        return null;
                }
                if (_valueConverter != null)
                    return _valueConverter.Read(ref reader, typeof(T), options);
                else
                    return JsonSerializer.Deserialize<T>(ref reader, options);
            }

            public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
            {
                if (value == null)// Not sure this is needed - the converter is not called for null values by default.
                    writer.WriteNullValue();
                else if (_valueConverter != null)
                    _valueConverter.Write(writer, value.Value, options);
                else
                    JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }
}