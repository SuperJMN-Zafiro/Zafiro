using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Zafiro.Core;
using Zafiro.Core.UI;
using Zafiro.Wpf.Services.MarkupWindow;

namespace Zafiro.Wpf.Services
{
    public class ViewService : IViewService
    {
        private readonly IDictionary<string, Type> viewDic = new Dictionary<string, Type>();

        public void Register(string token, Type viewType)
        {
            viewDic.Add(token, viewType);
        }

        public void Show(string key, object viewModel)
        {
            var view = (Window)Locate(key);

            if (viewDic.TryGetValue(key, out var viewType))
            {
                view.DataContext = viewModel;
                view.Owner = Application.Current.MainWindow;
                view.Show();
            }
        }

        private FrameworkElement Locate(string key)
        {
            if (!viewDic.TryGetValue(key, out var viewType))
            {
                return null;
            }

            return (FrameworkElement) Activator.CreateInstance(viewType);
        }

        public async Task<DialogResult> Show(string key, string title, object vm)
        {
            var dialogWindow = new DialogWindow();
            var options = new List<Option>
            {
                new Option("OK", OptionValue.OK),
                new Option("Cancel", OptionValue.Cancel)
            };

            var content = Locate(key);
            var dialogViewModel = new DialogViewModel(title, content, options, dialogWindow);
            dialogWindow.DataContext = dialogViewModel;

            content.DataContext = vm;
            
            var confirmed = (await dialogWindow.ShowDialogAsync()).HasValue && dialogViewModel.SelectedOption?.OptionValue == OptionValue.OK;
            dialogWindow.Close();

            return confirmed ? DialogResult.Yes : DialogResult.No;
        }
    }
}