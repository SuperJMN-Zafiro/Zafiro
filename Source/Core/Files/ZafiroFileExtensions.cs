using System.Threading.Tasks;
using Core.Mixins;

namespace Core.Files
{
    public static class ZafiroFileExtensions
    {
        public static async Task<string> ReadToEnd(this IZafiroFile file)
        {
            using (var openForRead = await file.OpenForRead())
            {
                return await openForRead.ReadToEnd();
            }
        }
    }
}