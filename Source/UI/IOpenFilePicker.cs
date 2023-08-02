using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.UI;

public interface IOpenFilePicker
{
    IObservable<IEnumerable<IZafiroFile>> PickMultiple(params FileTypeFilter[] filters);
    IObservable<Maybe<IZafiroFile>> PickSingle(params FileTypeFilter[] filters);
}