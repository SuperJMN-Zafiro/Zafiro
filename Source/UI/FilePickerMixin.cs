using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Optional;
using Zafiro.Core.Files;

namespace Zafiro.UI
{
    public static class FilePickerMixin
    {
        public static async Task<Option<IZafiroFile>> Pick(this IOpenFilePicker openFileService,
            IEnumerable<FileTypeFilter> extensions, Func<string> getCurrentFolder, Action<string> setCurrentFolder)
        {
            openFileService.FileTypeFilter.Clear();
            openFileService.FileTypeFilter.AddRange(extensions);

            openFileService.InitialDirectory = getCurrentFolder();

            var selected = await openFileService.Pick();

            selected.MatchSome(file =>
            {
                var directoryName = System.IO.Path.GetDirectoryName(file.Source.OriginalString);
                setCurrentFolder(directoryName);
            });

            return selected;
        }

        public static IObservable<IZafiroFile> Picks(this IOpenFilePicker openFileService,
            IEnumerable<FileTypeFilter> extensions, Func<string> getCurrentFolder, Action<string> setCurrentFolder)
        {
            return
                from pick in Observable.FromAsync(() => openFileService.Pick(extensions, getCurrentFolder, setCurrentFolder))
                from file in pick.ToEnumerable()
                select file;
        }
    }
}