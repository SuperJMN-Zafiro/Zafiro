using System;
using System.Reactive.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Zafiro.Uwp.Controls
{
    public static class ObservableFileExtensions
    {
        public static IObservable<StorageFile> Pickfile(string commitButtonText, string[] loadExtensions)
        {
            var picker = new FileOpenPicker
            {
                CommitButtonText = commitButtonText,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            foreach (var ext in loadExtensions)
            {
                picker.FileTypeFilter.Add(ext);
            }

            return Observable.FromAsync(() => picker.PickSingleFileAsync().AsTask()).Where(x => x != null);
        }
    }
}