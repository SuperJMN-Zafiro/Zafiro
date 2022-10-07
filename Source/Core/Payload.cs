using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Core
{
    public class Payload : Dictionary<string, object>
    {
        public Payload(IEnumerable<KeyValuePair<string, object>> items) : base(items.ToDictionary(pair => pair.Key, pair => pair.Value))
        {
        }
    }
}