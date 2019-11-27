using System.Reflection;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Extensions;

namespace Zafiro.Uwp.Controls.ObjEditor.TemplateMatchers
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