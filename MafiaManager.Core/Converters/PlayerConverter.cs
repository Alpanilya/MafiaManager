using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MafiaManager.Core.Converters
{
    public class PlayerConverter : JsonConverter<IPlayer>
    {
        public override IPlayer Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<Player>(ref reader, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            IPlayer value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (Player)value, options);
        }
    }
}
