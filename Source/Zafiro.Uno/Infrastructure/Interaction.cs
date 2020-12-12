using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Optional;
using Zafiro.UI;

namespace Zafiro.Uno.Infrastructure
{
    public class Interaction : IInteraction
    {
        public Task Message(string title, string markdown, Option<string> dismissText, Option<string> assetPathRoot)
        {
            return new MessageDialog(markdown) { Title = title }.ShowAsync().AsTask();
        }
    }
}