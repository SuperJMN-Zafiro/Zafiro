using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Zafiro.Mixins;

namespace Zafiro.DataModel;

public class StringData(string content, Encoding encoding) : IData
{
    public StringData(string content) : this(content, Encoding.Default)
    {
    }

    public string Content { get; } = content;

    public IObservable<byte[]> Bytes { get; } = content.ToBytes(encoding).ToObservable().Buffer(1024).Select(list => list.ToArray());
    public long Length { get; } = content.Length;

    public static implicit operator StringData(string content) => new(content, Encoding.Default);
}