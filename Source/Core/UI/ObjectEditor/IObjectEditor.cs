using Zafiro.Core.UI.ObjectEditor.TemplateMatchers;

namespace Zafiro.Core.UI.ObjectEditor
{
    public interface IObjectEditor<TEditor, TTemplate>
    {
        object SelectedItems { get; set; }
        TTemplate DefaultEditorTemplate { get; set; }
        EditorCollection<TTemplate> EditorsCore { get; }
    }
}