using System.Reflection;

namespace Zafiro.Zafiro.UI.ObjectEditor.TemplateMatchers
{
    public interface ITemplateMatcher<T>
    {
        T Select(EditorCollection<T> editors, PropertyInfo property);
        ITemplateMatcher<T> Next { get; set; }
    }
}