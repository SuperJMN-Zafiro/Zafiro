namespace Zafiro.PropertySystem.Tests.Model
{
    using System.Collections.Generic;

    public class BaseObject
    {
        private ICollection<BaseObject> children = new List<BaseObject>();
        public BaseObject Parent { get; set; }
        public void AddChild(BaseObject child)
        {
            child.Parent = this;
            children.Add(child);
        }
    }
}