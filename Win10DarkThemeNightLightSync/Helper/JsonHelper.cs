using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Win10DarkThemeNightLightSync.Helper
{
    public static class JsonHelper
    {
        public static Task<T> Deserialize<T>(string UTF8Json, CancellationToken token = default)
        {
            return Task.Run(() => JsonConvert.DeserializeObject<T>(UTF8Json), token);
        }

        public static Task<T> Deserialize<T>(Stream stream, CancellationToken token = default) =>
            Task.Run(() =>
            {
                var serializer = new JsonSerializer();
                using var textReader = new StreamReader(stream);
                using var reader = new JsonTextReader(textReader);
                return serializer.Deserialize<T>(reader);
            }, token);
        public static Task<MemoryStream> SerializeToMemoryStream(object payload, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                var stream = new MemoryStream();
                var serializer = new JsonSerializer();
                using var textWriter = new StreamWriter(stream, new UTF8Encoding(false), 1024, leaveOpen:true);
                using var writer = new JsonTextWriter(textWriter);
                serializer.Serialize(writer, payload);
                stream.Seek(0,SeekOrigin.Begin);
                return stream;
            }, token);
        }

        public static Task SerializeToFileStream(object payload, FileStream stream, CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                var serializer = new JsonSerializer();
                using var textWriter = new StreamWriter(stream, new UTF8Encoding(false), 1024, leaveOpen: true);
                using var writer = new JsonTextWriter(textWriter);
                serializer.Serialize(writer, payload);
                stream.Seek(0, SeekOrigin.Begin);
            }, token);
        }
    }

    public class MSEpochConverter : DateTimeConverterBase
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTimeOffset) value).ToUnixTimeMilliseconds().ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return DateTimeOffset.FromUnixTimeMilliseconds((long) reader.Value);
        }
    }

}