using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zafiro.Core.UI
{
    public interface IDialogService
    {
        Task Notice(string title, string content);
        Task<Option> Interaction(string title, string markdown, IEnumerable<Option> options, string assetBasePath = "");
    }
}