using System.Threading.Tasks;

namespace Zafiro.UI.Infrastructure.Uno
{
    public interface IDialogService
    {
        Task Show(string title, string content);
    }
}