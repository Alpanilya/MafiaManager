using MafiaManager.Core.Entities;
using MafiaManager.Core.Interfaces;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MafiaManager.Core.Converters
{
    public class RoleConverter : JsonConverter<IRole>
    {
        public override IRole Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<Role>(ref reader, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            IRole value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (Role)value, options);
        }
    }
}
