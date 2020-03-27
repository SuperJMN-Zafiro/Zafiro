using System.Threading.Tasks;

namespace Zafiro.Core.UI
{
    public interface IDialogService
    {
        Task Show(string title, string content);
    }
}