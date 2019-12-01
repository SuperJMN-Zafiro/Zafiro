using System.Reflection;
using Avalonia.Markup.Xaml.Templates;

namespace Zafiro.Avalonia.ObjEditor.TemplateMatchers
{
    public interface ITemplateMatcher
    {
        DataTemplate Select(EditorCollection editors, PropertyInfo property);
        ITemplateMatcher Next { get; set; }
    }
}