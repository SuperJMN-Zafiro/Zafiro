using System.Collections.Generic;
using Zafiro.Core.ObjectEditor.TemplateMatchers;

namespace Zafiro.Core.ObjectEditor
{
    public interface IObjectEditor<TEditor, TTemplate>
    {
        object SelectedItems { get; set; }
        TTemplate DefaultEditorTemplate { get; set; }
        EditorCollection<TTemplate> EditorsCore { get; }
    }
}