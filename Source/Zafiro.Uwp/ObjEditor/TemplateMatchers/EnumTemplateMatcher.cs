using System;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace Zafiro.Uwp.ObjEditor.TemplateMatchers
{
    public class EnumTemplateMatcher : TemplateMatcher
    {
        protected override DataTemplate SelectOverride(EditorCollection editors, PropertyInfo property)
        {
            if (property.PropertyType.IsEnum)
            {
                return editors.FirstOrDefault(x => x.Key.TargetType == typeof(Enum))?.Template;
            }

            return null;
        }
    }
}