using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Zafiro.Core.Files;
using Zafiro.Core.UI;

namespace Zafiro.Avalonia
{
    public class AvaloniaFilePicker : IFilePicker
    {
        private readonly Lazy<Window> mainWindow;
        private readonly OpenFileDialog openDlg = new OpenFileDialog();
        private readonly SaveFileDialog saveDlg = new SaveFileDialog();

        public AvaloniaFilePicker(Lazy<Window> mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public IObservable<ZafiroFile> Open(string title, IEnumerable<FileTypeFilter> filters)
        {
            openDlg.Filters = GetFilters(filters).ToList();

            var window = mainWindow.Value;

            return Observable
                .FromAsync(() => openDlg.ShowAsync(window))
                .Select(storageFile => storageFile != null && storageFile.Any() ? new AvaloniaFile(storageFile.First()) : null);
        }

        private IEnumerable<FileDialogFilter> GetFilters(IEnumerable<FileTypeFilter> filters)
        {
            return filters.Select(x => new FileDialogFilter()
            {
                Name = x.Description,
                Extensions = x.Extensions.Select(e => e.Substring(1, e.Length - 1)).ToList(),
            });
        }

        public IObservable<ZafiroFile> Save(string title, IEnumerable<FileTypeFilter> filters)
        {
            saveDlg.Title = title;

            openDlg.Filters = GetFilters(filters).ToList();

            var window = mainWindow.Value;

            return Observable
                .FromAsync(() => saveDlg.ShowAsync(window))
                .Where(storageFile => storageFile != null)
                .Select(storageFile => new AvaloniaFile(storageFile));
        }
    }
}