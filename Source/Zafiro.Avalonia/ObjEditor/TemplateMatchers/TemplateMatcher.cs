using System.Reflection;
using Avalonia.Markup.Xaml.Templates;

namespace Zafiro.Avalonia.ObjEditor.TemplateMatchers
{
    public abstract class TemplateMatcher : ITemplateMatcher
    {
        protected abstract DataTemplate SelectOverride(EditorCollection editors, PropertyInfo property);

        public DataTemplate Select(EditorCollection editors, PropertyInfo property)
        {
            var selected = SelectOverride(editors, property);

            if (selected != null)
            {
                return selected;
            }

            return Next?.Select(editors, property);
        }

        public ITemplateMatcher Chain(ITemplateMatcher nextlogger)
        {
            ITemplateMatcher lastLogger = this;

            while (lastLogger.Next != null)
            {
                lastLogger = lastLogger.Next;
            }

            lastLogger.Next = nextlogger;
            return this;
        }

        public ITemplateMatcher Next { get; set; }
    }
}