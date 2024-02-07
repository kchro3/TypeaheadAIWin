using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;


namespace TypeaheadAIWin.Source
{
    /**
     * This class is used to serialize ChatMessage objects to JSON.
     * 
     * It more or less conforms to the JSON conversion in Swift. Swift supports the serialization of enums with associated values,
     * which makes the serialization of the messageType complex.
     */
    public class ChatMessageJsonConverter : JsonConverter<ChatMessage>
    {
        public override ChatMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Deserialization is not supported.");
        }

        public override void Write(Utf8JsonWriter writer, ChatMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("isCurrentUser", value.Role == ChatMessageRole.User);
            writer.WriteString("text", value.Text);

            // Determine if the message is a text or image message
            if (value.Image != null)
            {
                writer.WriteStartObject("messageType");
                writer.WriteStartObject("image");
                writer.WriteStartObject("data");
                writer.WriteStartObject("b64_json");

                writer.WriteString("_0", ConvertToBase64(value.Image));

                writer.WriteEndObject();
                writer.WriteEndObject();
                writer.WriteEndObject();
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteStartObject("messageType");
                writer.WriteStartObject("string");
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        private static string ConvertToBase64(ImageSource imageSource, int qualityLevel = 75)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                BitmapEncoder encoder = new JpegBitmapEncoder { QualityLevel = qualityLevel };
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using var stream = new MemoryStream();
                encoder.Save(stream);
                byte[] bitmapData = stream.ToArray();
                return Convert.ToBase64String(bitmapData);
            }
            else
            {
                throw new ArgumentException("ImageSource must be a BitmapSource.");
            }
        }
    }
}
