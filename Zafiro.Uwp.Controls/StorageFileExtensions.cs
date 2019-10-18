using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Zafiro.Uwp.Controls
{
    public static class StorageFileExtensions
    {
        public static async Task<string> ReadToEnd(this StorageFile file)
        {
            using (var openStreamForReadAsync = await file.OpenStreamForReadAsync())
            using (var streamReader = new StreamReader(openStreamForReadAsync))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}