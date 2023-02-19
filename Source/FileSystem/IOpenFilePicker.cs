using CSharpFunctionalExtensions;

namespace Zafiro.FileSystem;

public interface IOpenFilePicker
{
    IObservable<IEnumerable<Result<IZafiroFile>>> PickMultiple(params (string name, string[] extensions)[] filters);
    IObservable<Result<IZafiroFile>> PickSingle(params (string name, string[] extensions)[] filters);
}