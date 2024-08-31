using System.Threading.Tasks;

namespace Zafiro;

public interface IDictionaryBasedService
{
    Task<Payload> Request(Payload payload);
}