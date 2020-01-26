using System;

namespace SampleApp.Infrastructure
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