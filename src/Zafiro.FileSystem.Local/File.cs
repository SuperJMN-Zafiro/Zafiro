using System.Reactive.Concurrency;
using Zafiro.DataModel;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.FileSystem.Local;

public class File : IMutableFile
{
    public File(IFileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    public IFileInfo FileInfo { get; }

    public IObservable<byte[]> Bytes { get; }
    public long Length { get; }

    public string Name => FileInfo.Name;

    public Task<Result> SetContents(IData data, IScheduler? scheduler, CancellationToken cancellationToken = default)
    {
        var result = Result.Try(() => FileInfo.Create());

        return result.Using(stream => data.DumpTo(stream, scheduler, cancellationToken));
    }

    public async Task<Result<IData>> GetContents()
    {
        return Data.FromFileInfo(FileInfo);
    }

    public bool IsHidden => (FileInfo.Attributes & FileAttributes.Hidden) != 0;

    public async Task<Result> Delete()
    {
        return Result.Try(() => FileInfo.Delete());
    }

    public Task<Result<bool>> Exists()
    {
        throw new NotImplementedException();
    }

    public async Task<Result> Create()
    {
        return Result.Try(() =>
        {
            using (FileInfo.Create())
            {
            }
        });
    }
}