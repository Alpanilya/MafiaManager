using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MafiaManager.Core.Converters
{
    public class GameSettingsConverter : JsonConverter<IGameSettings>
    {
        public override IGameSettings Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<GameSettings>(ref reader, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            IGameSettings value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (GameSettings)value, options);
        }
    }
}
