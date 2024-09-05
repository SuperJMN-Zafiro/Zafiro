using System.IO.IsolatedStorage;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;

namespace Zafiro.FileSystem;

[PublicAPI]
public class ApplicationStorage
{
    public static Result<Stream> OpenRead(string path)
    {
        return Result.Try(GetStore)
            .Bind(store =>
            {
                return Result.Try(() =>
                {
                    var isolatedStorageFileStream = new IsolatedStorageFileStream(path, FileMode.Open, store);
                    return (Stream)isolatedStorageFileStream;
                });
            });
    }

    public static Result<Stream> OpenWrite(string path)
    {
        return Result.Try(GetStore)
            .Map(store => (Stream)new IsolatedStorageFileStream(path, FileMode.Create, store));
    }

    private static IsolatedStorageFile GetStore()
    {
        var isolatedStorageFile = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
        return isolatedStorageFile;
    }
}