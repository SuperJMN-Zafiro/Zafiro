using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Zafiro.Core.Mixins;

namespace Zafiro.Core.Files
{
    public class FileOperator
    {
        private readonly IFilePicker picker;

        public FileOperator(IFilePicker picker)
        {
            this.picker = picker;
        }

        public IObservable<T> OpenFile<T>(Func<Stream, Task<T>> processFileStream, string[] extensions, string title = "")
        {
            var pick = picker.Pick(title, extensions)
                .Where(file => file != null)
                .SelectMany(file => ObservableMixin
                    .Using(file.OpenForRead,
                        stream => Observable.FromAsync(() => InvokeFunc(stream, processFileStream))));

            return pick;
        }

        public IObservable<Unit> SaveFile(Func<Stream, Task> processFileStream, KeyValuePair<string, IList<string>>[] extensions, string title = "")
        {
            var pick = picker.PickSave(title, extensions)
                .Where(file => file != null)
                .SelectMany(file => ObservableMixin
                    .Using(file.OpenForWrite,
                        stream => Observable.FromAsync(() => InvokeFunc(stream, processFileStream))));

            return pick;
        }

        private static async Task<Unit> InvokeFunc(Stream arg, Func<Stream, Task> func)
        {
            await func(arg);
            return Unit.Default;
        }

        private static Task<T> InvokeFunc<T>(Stream arg, Func<Stream, Task<T>> func)
        {
            return func(arg);
        }
    }
}