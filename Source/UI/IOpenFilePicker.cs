using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.UI;

public interface IOpenFilePicker
{
    IObservable<IEnumerable<Result<IZafiroFile>>> PickMultiple(params FileTypeFilter[] filters);
    IObservable<Result<IZafiroFile>> PickSingle(params FileTypeFilter[] filters);
}