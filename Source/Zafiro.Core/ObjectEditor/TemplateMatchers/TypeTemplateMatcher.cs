using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zafiro.Core.ObjectEditor.TemplateMatchers
{
    public class TypeTemplateMatcher<T> : TemplateMatcher<T> where T : class
    {
        protected override T SelectOverride(EditorCollection<T> editors, PropertyInfo property)
        {
            var editorKey = editors.FirstOrDefault(x => x.Key.TargetType == property.PropertyType && x.Key.PropertyName == null);

            return editorKey?.Template;
        }
    }
}