using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MafiaManager.Core.Converters
{
    public class GameConverter : JsonConverter<IGame>
    {
        public override IGame Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<Game>(ref reader, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            IGame value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (Game)value, options);
        }
    }
}
