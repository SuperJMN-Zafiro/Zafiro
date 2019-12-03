using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Zafiro.Core.Files;

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

        public IObservable<ZafiroFile> Pick(string title, string[] extensions)
        {
            foreach (var ext in extensions)
            {
                openDlg.Filters.Add(new FileDialogFilter()
                {
                    Name = ext,
                    Extensions = new List<string> { ext.Substring(1, ext.Length - 1) },
                });
            }

            var window = mainWindow.Value;

            return Observable
                .FromAsync(() => openDlg.ShowAsync(window))
                .Select(storageFile => storageFile != null && storageFile.Any() ? new AvaloniaFile(storageFile.First()) : null);
        }

        public IObservable<ZafiroFile> PickSave(string title, KeyValuePair<string, IList<string>>[] extensions)
        {
            saveDlg.Title = title;

            foreach (var ext in extensions)
            {
                openDlg.Filters.Add(new FileDialogFilter()
                {
                    Name = ext.Key,
                    Extensions = ext.Value.ToList(),
                });
            }

            var window = mainWindow.Value;

            return Observable
                .FromAsync(() => saveDlg.ShowAsync(window))
                .Where(storageFile => storageFile != null)
                .Select(storageFile => new AvaloniaFile(storageFile));
        }
    }
}