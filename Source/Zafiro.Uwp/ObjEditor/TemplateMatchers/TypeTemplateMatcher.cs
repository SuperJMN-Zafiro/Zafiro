using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace Zafiro.Uwp.ObjEditor.TemplateMatchers
{
    public class TypeTemplateMatcher : TemplateMatcher
    {
        protected override DataTemplate SelectOverride(EditorCollection editors, PropertyInfo property)
        {
            var editorKey = editors.FirstOrDefault(x => x.Key.TargetType == property.PropertyType && x.Key.PropertyName == null);

            return editorKey?.Template;
        }
    }
}