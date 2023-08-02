using Zafiro.Zafiro.UI.ObjectEditor.TemplateMatchers;

namespace Zafiro.Zafiro.UI.ObjectEditor
{
    public interface IObjectEditor<TEditor, TTemplate>
    {
        object SelectedItems { get; set; }
        TTemplate DefaultEditorTemplate { get; set; }
        EditorCollection<TTemplate> EditorsCore { get; }
    }
}