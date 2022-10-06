using System.Collections.Generic;
using System.Threading.Tasks;
using FileSystem;
using Optional;

namespace UI
{
    public interface IOpenFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; set; }
        Task<Option<IZafiroFile>> Pick();
    }
}