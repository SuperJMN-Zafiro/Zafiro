using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;

namespace Zafiro.Uno.Controls.ObjectEditor
{
    [CreateFromString(MethodName = nameof(FromString))]
    public class EditorDefinitionKey
    {
        private string keyString;

        public static EditorDefinitionKey FromString(string str)
        {
            var (type, properties) = Parse(str);

            return new EditorDefinitionKey
            {
                TargetType = type,
                Properties = properties,
            };
        }

        public static (Type, ICollection<string>) Parse(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            var split = str.Split('|');
            if (split.Length == 2)
            {
                var targetType = Type.GetType(split[0]);
                var properties = split[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                return (targetType, properties);
            };

            throw new ArgumentException($"The string '{str}' is not a valid EditorDefinitionKey");
        }

        public Type TargetType { get; set; }
        public ICollection<string> Properties { get; set; } = new List<string>();

        public string KeyString
        {
            get => keyString;
            set
            {
                keyString = value;
                if (value != null)
                {
                    var (type, properties) = Parse(value);
                    {
                        TargetType = type;
                        Properties = properties;
                    }
                }
            }
        }
    }
}