using System.Collections.Generic;
using System.Threading.Tasks;
using Optional;
using Zafiro.FileSystem;

namespace Zafiro.UI
{
    public interface IOpenFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; set; }
        Task<Option<IZafiroFile>> Pick();
    }
}