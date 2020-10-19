using System;
using System.Threading.Tasks;

namespace Zafiro.Core.UI.Interaction
{
    public interface IShell
    {
        Task Popup<T>(IContextualizable content, T viewModel, Action<PopupConfiguration<T>> configure);
    }
}