using System.Collections.Generic;

namespace UI
{
    public class PopupModel
    {
        public string Title { get; }

        public PopupModel(object content, string title, IEnumerable<Option> options)
        {
            Title = title;
            Content = content;
            Options = options;
        }

        public IEnumerable<Option> Options { get; set; }

        public object Content { get; }
    }
}