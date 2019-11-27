using System.Linq;
using System.Reflection;
using Avalonia.Markup.Xaml.Templates;

namespace Zafiro.Avalonia.ObjEditor.TemplateMatchers
{
    public class NameAndTypeTemplateMatcher : TemplateMatcher
    {
        protected override DataTemplate SelectOverride(EditorCollection editors, PropertyInfo property)
        {
            return editors.FirstOrDefault(x => x.Key.TargetType == property.PropertyType && x.Key.PropertyName == property.Name)?.Template;
        }
    }
}