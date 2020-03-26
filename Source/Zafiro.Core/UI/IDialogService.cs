using System.Threading.Tasks;

namespace Zafiro.ReactiveUI
{
    public interface IDialogService
    {
        Task Show(string title, string content);
    }
}