using Zafiro.UI;

namespace Zafiro.Wpf.Tests
{
    public class TestHaveDataContext : IHaveDataContext
    {
        public void SetDataContext(object dataContext)
        {
        }

        public object Object { get; }
    }
}
