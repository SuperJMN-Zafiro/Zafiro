using System.Reflection;
using Avalonia.Markup.Xaml.Templates;

namespace Zafiro.Avalonia.ObjEditor.TemplateMatchers
{
    internal class DefaultTemplateMatcher : TemplateMatcher
    {
        private readonly PropertyItem parent;

        public DefaultTemplateMatcher(PropertyItem parent)
        {
            this.parent = parent;
        }

        protected override DataTemplate SelectOverride(EditorCollection editors, PropertyInfo property)
        {
            var objEditor = parent.FindAscendant<ObjectEditor>();
            return objEditor.DefaultEditorTemplate;
        }
    }
}