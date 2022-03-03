using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Rio.API.GeoSpatial
{
    public class DoubleFormatConverter : JsonConverter
    {
        private readonly int _numberOfSignificantDigits;

        public DoubleFormatConverter(int numberOfSignificantDigits)
        {
            _numberOfSignificantDigits = numberOfSignificantDigits;
        }
        public DoubleFormatConverter()
        {
            _numberOfSignificantDigits = 2;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double) || objectType == typeof(double?);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(string.Format(new NumberFormatInfo { NumberDecimalDigits = _numberOfSignificantDigits }, "{0:F}", value));
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}