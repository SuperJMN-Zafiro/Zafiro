using Zafiro.FileSystem.Core;

namespace Zafiro.FileSystem.Tests;

public class SizeCompareStrategyTests
{
    [Fact]
    public async Task Same_size_are_equal()
    {
        var sut = new SizeCompareStrategy();
        IZafiroFile a = new MyTestFile(123);
        IZafiroFile b = new MyTestFile(123);
        var result = await sut.Compare(a, b);
        result.Should().SucceedWith(true);
    }

    [Fact]
    public async Task Different_size_are_distinct()
    {
        var sut = new SizeCompareStrategy();
        IZafiroFile a = new MyTestFile(33);
        IZafiroFile b = new MyTestFile(123);
        var result = await sut.Compare(a, b);
        result.Should().SucceedWith(false);
    }

    [Fact]
    public async Task Failure()
    {
        var sut = new SizeCompareStrategy();
        IZafiroFile a = new MyTestFile(Result.Failure<long>("Failed badly"));
        IZafiroFile b = new MyTestFile(123);
        var result = await sut.Compare(a, b);
        result.Should().Fail();
    }

    public class MyTestFile : IZafiroFile
    {
        public MyTestFile(Result<long> size)
        {
            Properties = Task.FromResult(size.Map(l => new FileProperties(false, DateTimeOffset.MinValue, l)));
        }

        public IObservable<byte> Contents { get; }
        public Task<Result<bool>> Exists { get; }
        public ZafiroPath Path { get; }
        public Task<Result<FileProperties>> Properties { get; }
        public Task<Result<IDictionary<HashMethod, byte[]>>> Hashes { get; }
        public IFileSystemRoot FileSystem { get; }
        public Task<Result> Delete() => throw new NotImplementedException();

        public Task<Result> SetContents(IObservable<byte> contents, CancellationToken cancellationToken = default) => throw new NotImplementedException();
        public Task<Result<Stream>> GetData() => throw new NotImplementedException();
        public Task<Result> SetData(Stream stream, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public Task<Result> SetData(Stream data) => throw new NotImplementedException();
    }
}