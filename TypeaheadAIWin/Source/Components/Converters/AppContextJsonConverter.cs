using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TypeaheadAIWin.Source.Model;

namespace TypeaheadAIWin.Source.Components.Converters
{
    public class AppContextJsonConverter : JsonConverter<ApplicationContext>
    {
        public override ApplicationContext? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ApplicationContext value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("appName", value.AppName);
            writer.WriteString("processName", value.ProcessName);
            writer.WriteString("serializedUIElement", value.SerializedUIElement);
            writer.WriteEndObject();
        }
    }
}
