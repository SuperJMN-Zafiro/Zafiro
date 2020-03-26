using Windows.UI.Xaml.Controls;

namespace Zafiro.Uno.Infrastructure.Navigation
{
    public class NavigationFrameMessage
    {
        public Frame MainFrame { get; }
        public Frame InnerFrame { get; }

        public NavigationFrameMessage(Frame mainFrame, Frame innerFrame)
        {
            MainFrame = mainFrame;
            InnerFrame = innerFrame;
        }
    }
}