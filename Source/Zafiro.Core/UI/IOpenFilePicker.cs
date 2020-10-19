using System.Collections.Generic;

namespace Zafiro.Core.UI
{
    public interface IOpenFilePicker
    {
        string InitialDirectory { get; set; }
        List<FileTypeFilter> FileTypeFilter { get; set; }
        string PickFile();
    }
}