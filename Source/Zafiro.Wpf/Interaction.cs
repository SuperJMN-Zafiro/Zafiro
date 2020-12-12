using System.Threading.Tasks;
using MvvmLib.Commands;
using Optional;
using Optional.Unsafe;

namespace Zafiro.UI.Wpf
{
    public class Interaction : IInteraction
    {
        private readonly IPopup shell;

        public Interaction(IPopup shell)
        {
            this.shell = shell;
        }

        public Task Message(string title, string markdown, Option<string> dismissText, Option<string> assetPathRoot)
        {
            var markdownContent = new MarkdownContent()
            {
                AssetPathRoot = assetPathRoot.ValueOrDefault(),
                Markdown = markdown,
            };

            return shell.ShowAsModal(new HaveDataContextAdapter(markdownContent), (object)null,
                c =>
                {
                    c.View.Title = title;
                    dismissText.MatchSome(text =>
                    {
                        c.AddOption(new Option(text, new DelegateCommand(() => c.View.Close())));
                    });
                });
        }
    }
}