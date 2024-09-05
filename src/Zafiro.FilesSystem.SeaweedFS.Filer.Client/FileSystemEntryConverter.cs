using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zafiro.FileSystem.SeaweedFS.Filer.Client;

public class FileSystemEntryConverter : JsonConverter<BaseEntry>
{
    public override BaseEntry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            if (IsFile(root))
            {
                return JsonSerializer.Deserialize<FileMetadata>(root.GetRawText(), options);
            }

            return JsonSerializer.Deserialize<Directory>(root.GetRawText(), options);
        }
    }

    private bool IsFile(JsonElement root)
    {
        var hasSize = root.GetProperty("FileSize").GetUInt64() > 0;
        var hasMd5 = root.GetProperty("Md5").GetString() != null;
        var hasInode = root.GetProperty("Inode").GetUInt64() > 0;
        return hasSize || hasMd5 || hasInode;
    }

    public override void Write(Utf8JsonWriter writer, BaseEntry value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Serialization is not supported in this custom converter.");
    }
}