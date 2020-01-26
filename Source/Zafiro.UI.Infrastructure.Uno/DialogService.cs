using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SampleApp.Infrastructure
{
    public class DialogService : IDialogService
    {
        public Task Show(string title, string content)
        {
            return new MessageDialog(content) { Title = title }.ShowAsync().AsTask();
        }
    }
}