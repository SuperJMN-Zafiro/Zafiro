using System;
using System.Collections.Generic;

namespace Zafiro.Core.UI.ObjectEditor
{
    public class EditorKey
    {
        public Type TargetType { get; }
        public List<string> Properties { get; }

        public EditorKey(Type targetType, List<string> properties)
        {
            TargetType = targetType;
            Properties = properties;
        }
    }
}