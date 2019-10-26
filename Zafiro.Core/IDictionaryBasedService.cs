using System.Threading.Tasks;

namespace Zafiro.Core
{
    public interface IDictionaryBasedService
    {
        Task<Payload> Request(Payload payload);
    }
}