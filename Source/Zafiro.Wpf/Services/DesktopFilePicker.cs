using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Win32;
using Zafiro.Core.Files;
using Zafiro.Core.UI;
using IFilePicker = Zafiro.Core.UI.IFilePicker;

namespace Zafiro.Wpf.Services
{
    public class DesktopFilePicker : IFilePicker
    {
        private readonly Func<Uri, ZafiroFile> getFile;

        public DesktopFilePicker(Func<Uri, ZafiroFile> getFile)
        {
            this.getFile = getFile;
        }

        public IObservable<ZafiroFile> Open(string title, IEnumerable<FileTypeFilter> filters)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = GetFilter(filters);
            dialog.FileName = "";

            //if (fileSystemOperations.DirectoryExists(InitialDirectory))
            //{
            //    dialog.InitialDirectory = InitialDirectory;
            //}
            
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return Observable.Return(getFile(new Uri(dialog.FileName)));
            }

            return Observable.Return<ZafiroFile>(null);
        }

        private static string GetFilter(IEnumerable<FileTypeFilter> filters)
        {
            var lines = filters.Select(x =>
            {
                var exts = string.Join(";", x.Extensions);
                return $"{x.Description}|{exts}";
            });

            var filter = string.Join("|", lines);

            return filter;
        }

        public IObservable<ZafiroFile> Save(string title, IEnumerable<FileTypeFilter> filters)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = GetFilter(filters);
            dialog.FileName = "";

            //if (fileSystemOperations.DirectoryExists(InitialDirectory))
            //{
            //    dialog.InitialDirectory = InitialDirectory;
            //}
            
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return Observable.Return(getFile(new Uri(dialog.FileName)));
            }

            return Observable.Return<ZafiroFile>(null);
        }
    }
}