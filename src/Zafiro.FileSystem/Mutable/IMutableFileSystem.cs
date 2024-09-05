using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Mutable;

public interface IMutableFileSystem
{
    ZafiroPath InitialPath { get; }
    Task<Result<IMutableDirectory>> GetDirectory(ZafiroPath path);
}