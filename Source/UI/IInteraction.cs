using System.Threading.Tasks;
using Optional;

namespace UI
{
    public interface IInteraction
    {
        public Task Message(string title, string markdown, Option<string> dismissText, Option<string> assetPathRoot);
    }
}