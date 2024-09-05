using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;
using Zafiro.FileSystem.Readonly;

namespace Zafiro.UI;

public interface IFileSystemPicker
{
    Task<Result<IEnumerable<IFile>>> PickForOpenMultiple(params FileTypeFilter[] filters);
    Task<Result<Maybe<IFile>>> PickForOpen(params FileTypeFilter[] filters);
    Task<Maybe<IMutableFile>> PickForSave(string desiredName, Maybe<string> defaultExtension, params FileTypeFilter[] filters);
    Task<Maybe<IMutableDirectory>> PickFolder();
}