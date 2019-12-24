using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;

namespace Zafiro.Uwp.ObjectEditor
{
    [CreateFromString(MethodName = nameof(FromString))]
    public class EditorDefinitionKey
    {
        public static EditorDefinitionKey FromString(string str)
        {
            
            var split = str.Split('|');
            if (split.Length == 2)
            {
                var targetType = Type.GetType(split[0]);
                var properties = split[1].Split(",", StringSplitOptions.RemoveEmptyEntries);

                return new EditorDefinitionKey
                {
                    TargetType = targetType,
                    Properties = properties,
                };
            }


            return new EditorDefinitionKey();
        }

        public Type TargetType { get; set; }
        public ICollection<string> Properties { get; set; } = new List<string>();
    }
}