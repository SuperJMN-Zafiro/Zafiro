using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Zafiro.UI;

namespace Zafiro.FileSystem;

public interface IOpenFilePicker
{
    IObservable<IEnumerable<Result<IZafiroFile>>> PickMultiple(params FileTypeFilter[] filters);
    IObservable<Result<IZafiroFile>> PickSingle(params FileTypeFilter[] filters);
}