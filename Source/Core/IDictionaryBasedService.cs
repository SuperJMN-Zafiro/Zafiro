using System.Threading.Tasks;

namespace Core
{
    public interface IDictionaryBasedService
    {
        Task<Payload> Request(Payload payload);
    }
}