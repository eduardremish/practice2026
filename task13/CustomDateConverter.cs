using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string format = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader read, Type type, JsonSerializerOptions opt)
        {
            var dateString = read.GetString();

            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            throw new JsonException($"Ошибка - неверный формат. Верный - {format}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime val, JsonSerializerOptions opt)
        {
            writer.WriteStringValue(val.ToString(format, CultureInfo.InvariantCulture));
        }
    }
}
