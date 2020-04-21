using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using Zafiro.Core;
using Zafiro.Core.UI;
using Zafiro.Wpf.Services.MarkupWindow;

namespace Zafiro.Wpf.Services
{
    public class WpfDialogService : IDialogService
    {
        public Task Show(string title, string content)
        {
            var observable = Observable.Defer<Unit>(() =>
            {
                MessageBox.Show(content, title);
                return Observable.Return(Unit.Default);
            });

            return observable.ToTask();
        }

        public async Task<Option> Pick(string title, string markdown, IEnumerable<Option> options, string assetBasePath = "")
        {
            var markdownViewerWindow = new MarkdownViewerWindow();
            Option option;
            using (var viewModel = new MarkupMessageViewModel(title, markdown, options, markdownViewerWindow, assetBasePath))
            {
                markdownViewerWindow.DataContext = viewModel;
                var wnd = markdownViewerWindow;
                await wnd.ShowDialogAsync();
                option = viewModel.SelectedOption;
            }

            return option;
        }
    }
}