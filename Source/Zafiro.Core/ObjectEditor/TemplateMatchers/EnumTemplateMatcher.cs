using System;
using System.Linq;
using System.Reflection;

namespace Zafiro.Core.ObjectEditor.TemplateMatchers
{
    public class EnumTemplateMatcher<T> : TemplateMatcher<T> where T : class
    {
        protected override T SelectOverride(EditorCollection<T> editors, PropertyInfo property)
        {
            if (property.PropertyType.IsEnum)
            {
                return editors.FirstOrDefault(x => x.Key.TargetType == typeof(Enum))?.Template;
            }

            return default;
        }
    }
}