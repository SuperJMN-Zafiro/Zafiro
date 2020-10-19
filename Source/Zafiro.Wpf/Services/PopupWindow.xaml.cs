using System.Threading.Tasks;
using Zafiro.Core;

namespace Zafiro.Wpf.Services
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class PopupWindow : IPopup
    {
        public PopupWindow()
        {
            InitializeComponent();
        }

        public new Task Show()
        {
            return this.ShowDialogAsync();
        }

        public void SetContext(object o)
        {
            DataContext = o;
        }

        public object Object => this.Content;
    }
}