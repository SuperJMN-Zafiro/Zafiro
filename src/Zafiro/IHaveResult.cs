using System.Threading.Tasks;

namespace Zafiro;

public interface IHaveResult<T>
{
    Task<T> Result { get; }
    void SetResult(T result);
}