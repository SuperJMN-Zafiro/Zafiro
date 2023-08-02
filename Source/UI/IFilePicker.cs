using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.UI;

public interface IFilePicker
{
    IObservable<IEnumerable<IZafiroFile>> PickForOpenMultiple(params FileTypeFilter[] filters);
    IObservable<Maybe<IZafiroFile>> PickForOpen(params FileTypeFilter[] filters);
    IObservable<Maybe<IZafiroFile>> PickForSave(string desiredName, Maybe<string> defaultExtension, params FileTypeFilter[] filters);
}