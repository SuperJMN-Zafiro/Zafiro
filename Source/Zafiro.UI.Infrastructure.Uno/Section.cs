using System;

namespace Zafiro.UI.Infrastructure.Uno
{
    public class Section
    {
        public Section(string name, Type viewModelType)
        {
            Name = name;
            ViewModelType = viewModelType;
        }

        public string Name { get; }

        public Type ViewModelType { get; }

        public object Icon { get; set; }
    }
}