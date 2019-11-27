using System.Reflection;
using Windows.UI.Xaml;

namespace Zafiro.Uwp.Controls.ObjEditor.TemplateMatchers
{
    public interface ITemplateMatcher
    {
        DataTemplate Select(EditorCollection editors, PropertyInfo property);
        ITemplateMatcher Next { get; set; }
    }
}