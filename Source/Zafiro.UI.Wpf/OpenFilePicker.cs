using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Optional;
using Zafiro.Core.Files;
using Zafiro.Core.FileSystem;

namespace Zafiro.UI.Wpf
{
    public class OpenFilePicker : IOpenFilePicker
    {
        private readonly Func<string, IZafiroFile> fileFactory;
        private readonly IFileSystemOperations fileSystemOperations;

        public OpenFilePicker(Func<string, IZafiroFile> fileFactory, IFileSystemOperations fileSystemOperations)
        {
            this.fileFactory = fileFactory;
            this.fileSystemOperations = fileSystemOperations;
        }

        public string InitialDirectory { get; set; }
        public List<FileTypeFilter> FileTypeFilter { get; set; } = new List<FileTypeFilter>();
        public Task<Option<IZafiroFile>> Pick()
        {
            var dialog = new OpenFileDialog();
            var lines = FileTypeFilter.Select(x =>
            {
                var exts = string.Join(";", x.Extensions);
                return $"{x.Description}|{exts}";
            });

            var filter = string.Join("|", lines);

            dialog.Filter = filter;
            dialog.FileName = "";

            if (fileSystemOperations.DirectoryExists(InitialDirectory))
            {
                dialog.InitialDirectory = InitialDirectory;
            }
            
            if (dialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                return Task.FromResult(fileFactory(dialog.FileName).Some());
            }

            return Task.FromResult(Optional.Option.None<IZafiroFile>());
        }       
    }
}