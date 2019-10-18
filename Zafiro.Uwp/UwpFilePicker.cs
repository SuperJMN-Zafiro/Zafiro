using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Zafiro.Core;

namespace Zafiro.Uwp.Controls.Reactive
{
    public class UwpFilePicker : IFilePicker
    {
        public IObservable<ZafiroFile> Pick(string title, string[] extensions)
        {
            var picker = new FileOpenPicker
            {
                CommitButtonText = title,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            foreach (var ext in extensions)
            {
                picker.FileTypeFilter.Add(ext);
            }

            return Observable
                .FromAsync(() => picker.PickSingleFileAsync().AsTask())
                .Where(x => x != null)
                .Select(x =>
                {
                    Func<Task<Stream>> f = x.OpenStreamForReadAsync;

                    return new UwpFile(f, x.Name);
                });
        }
    }

    public class UwpFile : ZafiroFile
    {
        public override string Name { get; }
        private readonly Func<Task<Stream>> open;

        public UwpFile(Func<Task<Stream>> open, string name)
        {
            Name = name;
            this.open = open;
        }


        public override Func<Task<Stream>> Open()
        {
            return open;
        }
    }
}