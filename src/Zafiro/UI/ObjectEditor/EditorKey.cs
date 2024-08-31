using System;
using System.Collections.Generic;

namespace Zafiro.UI.ObjectEditor;

public class EditorKey
{
    public EditorKey(Type targetType, List<string> properties)
    {
        TargetType = targetType;
        Properties = properties;
    }

    public Type TargetType { get; }
    public List<string> Properties { get; }
}