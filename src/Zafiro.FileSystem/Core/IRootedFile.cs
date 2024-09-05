using Zafiro.FileSystem.Readonly;

namespace Zafiro.FileSystem.Core;

public interface IRootedFile : IFile, IRooted<IFile>;