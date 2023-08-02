using System;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.UI;

public interface ISaveFilePicker
{
    IObservable<Maybe<IZafiroFile>> Pick(string desiredName, string defaultExtension, params FileTypeFilter[] filters);
}