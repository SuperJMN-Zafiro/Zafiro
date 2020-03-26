using System.Threading.Tasks;

namespace Zafiro.Uno.Infrastructure
{
    public interface IDialogService
    {
        Task Show(string title, string content);
    }
}