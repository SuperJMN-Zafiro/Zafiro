using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zafiro.Core.ObjectEditor.TemplateMatchers
{
    public class EditorCollection<T> : Collection<Editor<T>>
    {
        public EditorCollection()
        {
        }

        public EditorCollection(List<Editor<T>> toList) : base(toList)
        {
        }
    }
}