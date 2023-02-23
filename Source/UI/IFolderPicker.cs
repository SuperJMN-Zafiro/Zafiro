using System;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.UI;

public interface IFolderPicker
{
    IObservable<Result<IZafiroDirectory>> Pick();
}