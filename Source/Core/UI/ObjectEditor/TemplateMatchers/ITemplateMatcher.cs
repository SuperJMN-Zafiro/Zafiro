using System.Reflection;

namespace Zafiro.Core.UI.ObjectEditor.TemplateMatchers
{
    public interface ITemplateMatcher<T>
    {
        T Select(EditorCollection<T> editors, PropertyInfo property);
        ITemplateMatcher<T> Next { get; set; }
    }
}