using System;
using CSharpFunctionalExtensions;
using Zafiro.UI;

namespace Zafiro.FileSystem;

public interface ISaveFilePicker
{
    IObservable<Result<IZafiroFile>> Pick(params FileTypeFilter[] filters);
}