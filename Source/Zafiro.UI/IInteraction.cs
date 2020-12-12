using System.Threading.Tasks;
using Optional;

namespace Zafiro.UI
{
    public interface IInteraction
    {
        public Task Message(string title, string markdown, Option<string> dismissText, Option<string> assetPathRoot);
    }
}