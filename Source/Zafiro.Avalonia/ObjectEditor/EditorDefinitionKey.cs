using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;

namespace Zafiro.Avalonia.ObjectEditor
{
    public class EditorDefinitionKey
    {
        public static EditorDefinitionKey FromString(string str)
        {

            var split = str.Split('|');
            if (split.Length == 2)
            {
                var targetType = Type.GetType(split[0]);
                var properties = split[1].Split(new []{ ','}, StringSplitOptions.RemoveEmptyEntries);

                return new EditorDefinitionKey
                {
                    TargetType = targetType,
                    Properties = new AvaloniaList<string>(properties),
                };
            }


            return new EditorDefinitionKey();
        }

        public Type TargetType { get; set; }
        public AvaloniaList<string> Properties { get; set; } = new AvaloniaList<string>();
    }
}