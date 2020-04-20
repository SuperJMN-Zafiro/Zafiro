using System.Collections.Generic;
using System.Threading.Tasks;
using Deployer;

namespace Zafiro.Core.UI
{
    public interface IDialogService
    {
        Task Show(string title, string content);
        Task<Option> Pick(string title, string markdown, IEnumerable<Option> options, string assetBasePath = "");
    }
}