using System.Linq;
using System.Reflection;

namespace Zafiro.Core.ObjectEditor.TemplateMatchers
{
    public class NameAndTypeTemplateMatcher<T> : TemplateMatcher<T> where T : class
    {
        protected override T SelectOverride(EditorCollection<T> editors, PropertyInfo property)
        {
            return editors.FirstOrDefault(x => x.Key.TargetType == property.PropertyType && x.Key.PropertyName == property.Name)?.Template;
        }
    }
}