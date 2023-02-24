using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.UI;

public interface IOpenFilePicker
{
    IObservable<IEnumerable<IStorable>> PickMultiple(params FileTypeFilter[] filters);
    IObservable<Maybe<IStorable>> PickSingle(params FileTypeFilter[] filters);
}