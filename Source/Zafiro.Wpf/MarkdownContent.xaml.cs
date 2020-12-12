using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Zafiro.UI.Wpf
{
    /// <summary>
    /// Interaction logic for MarkdownCotent.xaml
    /// </summary>
    public partial class MarkdownContent : UserControl
    {
        public MarkdownContent()
        {
            InitializeComponent();
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start((string)e.Parameter);
        }

        public static readonly DependencyProperty AssetPathRootProperty = DependencyProperty.Register(
            "AssetPathRoot", typeof(string), typeof(MarkdownContent), new PropertyMetadata(default(string)));

        public string AssetPathRoot
        {
            get { return (string) GetValue(AssetPathRootProperty); }
            set { SetValue(AssetPathRootProperty, value); }
        }

        public static readonly DependencyProperty MarkdownProperty = DependencyProperty.Register(
            "Markdown", typeof(string), typeof(MarkdownContent), new PropertyMetadata(default(string)));

        public string Markdown
        {
            get { return (string) GetValue(MarkdownProperty); }
            set { SetValue(MarkdownProperty, value); }
        }
    }
}
