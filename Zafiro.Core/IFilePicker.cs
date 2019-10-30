using System;
using System.Collections.Generic;

namespace Zafiro.Core
{
    public interface IFilePicker
    {
        IObservable<ZafiroFile> Pick(string title, string[] extensions);
        IObservable<ZafiroFile> PickSave(string title, KeyValuePair<string, IList<string>>[] extensions);
    }
}