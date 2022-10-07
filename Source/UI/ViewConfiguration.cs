using System.Collections.ObjectModel;

namespace Zafiro.UI
{
    public class ViewConfiguration<T>
    {
        public ViewConfiguration(IView view, T model)
        {
            View = view;
            Model = model;
        }

        public T Model { get; }

        public void AddOption(Option option)
        {
            Options.Add(option);
        }

        public Collection<Option> Options { get; } = new Collection<Option>(); 
        public IView View { get; }
    }
}