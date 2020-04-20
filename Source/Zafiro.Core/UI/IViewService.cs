using System;
using System.Threading.Tasks;

namespace Zafiro.Core.UI
{
    public interface IViewService
    {
        void Register(string token, Type viewType);
        void Show(string key, object viewModel);
        Task<DialogResult> Show(string key, string title, object context);
    }
}