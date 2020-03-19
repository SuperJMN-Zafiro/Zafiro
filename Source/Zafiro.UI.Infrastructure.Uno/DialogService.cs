using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Zafiro.UI.Infrastructure.Uno
{
    public class DialogService : IDialogService
    {
        public Task Show(string title, string content)
        {
            return new MessageDialog(content) { Title = title }.ShowAsync().AsTask();
        }
    }
}