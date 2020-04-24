using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Zafiro.Core;
using Zafiro.Core.UI;
using Zafiro.Wpf.Services.MarkupWindow;

namespace Zafiro.Wpf.Services
{
    public class SimpleInteraction : ISimpleInteraction
    {
        private readonly IDictionary<string, Type> viewDic = new Dictionary<string, Type>();

        public void Register(string token, Type viewType)
        {
            viewDic.Add(token, viewType);
        }

        public async Task Interact(string key, string title, object vm,
            ICollection<DialogButton> buttons)
        {
            var dialogWindow = new DialogWindow();
            var options = buttons;

            var content = Locate(key);
            var dialogViewModel = new DialogViewModel(title, content, options.ToList(), dialogWindow);
            dialogWindow.DataContext = dialogViewModel;

            content.DataContext = vm;

            await dialogWindow.ShowDialogAsync();
            
            dialogWindow.Close();
        }

        private FrameworkElement Locate(string key)
        {
            if (!viewDic.TryGetValue(key, out var viewType))
            {
                return null;
            }

            return (FrameworkElement) Activator.CreateInstance(viewType);
        }
    }
}