using System;
using System.Threading.Tasks;

namespace Zafiro.Core.UI.Interaction
{
    public class Shell : IShell
    {
        private readonly Func<IPopup> popupFactory;

        public Shell(Func<IPopup> popupFactory)
        {
            this.popupFactory = popupFactory;
        }

        public Task Popup<T>(IContextualizable content, T viewModel, Action<PopupConfiguration<T>> configure)
        {
            var popup = popupFactory();
            var config = new PopupConfiguration<T>(popup, viewModel);
            configure(config);
            var popupModel = new PopupModel(content.Object, config.Popup.Title, config.Options);

            popup.SetContext(popupModel);
            content.SetContext(viewModel);
            return popup.Show();
        }
    }
}