using System;
using System.IO;
using System.Reactive.Linq;
using Windows.Storage.Pickers;
using Zafiro.Core;

namespace Zafiro.Uwp.Controls.Reactive
{
    public class UwpFilePicker : IFilePicker
    {
        public IObservable<Stream> Pick(string title, string[] extensions)
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
                .SelectMany(x => x.OpenStreamForReadAsync());
        }
    }
}