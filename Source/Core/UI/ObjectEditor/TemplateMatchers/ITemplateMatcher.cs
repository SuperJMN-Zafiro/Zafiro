using System.Reflection;

namespace Core.UI.ObjectEditor.TemplateMatchers
{
    public interface ITemplateMatcher<T>
    {
        T Select(EditorCollection<T> editors, PropertyInfo property);
        ITemplateMatcher<T> Next { get; set; }
    }
}