using System;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.UI;

public interface ISaveFilePicker
{
    IObservable<Maybe<IStorable>> Pick(string desiredName, string defaultExtension, params FileTypeFilter[] filters);
}