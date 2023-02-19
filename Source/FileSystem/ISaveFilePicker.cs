using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface ISaveFilePicker
{
    IObservable<Result<IZafiroFile>> Pick(params (string, string[])[] filters);
}