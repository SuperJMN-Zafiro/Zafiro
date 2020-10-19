using System.Collections.ObjectModel;

namespace Zafiro.Core.UI.Interaction
{
    public class PopupConfiguration<T>
    {
        public PopupConfiguration(IPopup popup, T model)
        {
            Popup = popup;
            Model = model;
        }

        public T Model { get; }

        public void AddOption(Option option)
        {
            Options.Add(option);
        }

        public Collection<Option> Options { get; } = new Collection<Option>(); 
        public IPopup Popup { get; }
    }
}