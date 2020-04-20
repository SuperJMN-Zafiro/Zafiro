using System.Windows.Input;
using Deployer;

namespace Zafiro.Wpf.Services.MarkupWindow
{
    public class OptionViewModel
    {
        public Option Option { get; }

        public OptionViewModel(Option option)
        {
            Option = option;
        }

        public ICommand Command { get; set; }
    }
}