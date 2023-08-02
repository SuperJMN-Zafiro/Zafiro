using System.Threading.Tasks;

namespace Zafiro.Zafiro
{
    public interface IDictionaryBasedService
    {
        Task<Payload> Request(Payload payload);
    }
}