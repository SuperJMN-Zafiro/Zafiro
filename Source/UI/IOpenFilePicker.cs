using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Files;
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