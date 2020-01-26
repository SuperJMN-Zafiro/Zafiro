using System.Threading.Tasks;

namespace SampleApp.Infrastructure
{
    public interface IDialogService
    {
        Task Show(string title, string content);
    }
}