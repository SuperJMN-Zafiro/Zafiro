using JetBrains.Annotations;
using Optional;

namespace Zafiro.UI;

[PublicAPI]
public interface IInteraction
{
    public Task Message(string title, string markdown, Option<string> dismissText, Option<string> assetPathRoot);
}