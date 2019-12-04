using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Zafiro.Uwp.Design
{
    public class ObjectCommands : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<IEnumerable<DesignerItem>> selectedContainers;

        public ObjectCommands(DesignerSurface designerSurface)
        {
            selectedContainers = designerSurface.SelectedContainersChanged.ToProperty(this, x => x.SelectedContainers);
            
            var canExecute = designerSurface.SelectedContainersChanged.Select(x => x.Count() > 1);

            AlignToTopCommand = ReactiveCommand.Create(() => AlignToTop(SelectedContainers.ToList()), canExecute);
            AlignToLeftCommand = ReactiveCommand.Create(() => AlignToLeft(SelectedContainers.ToList()), canExecute);
            AlignToRightCommand = ReactiveCommand.Create(() => AlignToRight(SelectedContainers.ToList()), canExecute);
            AlignToBottomCommand = ReactiveCommand.Create(() => AlignToBottom(SelectedContainers.ToList()), canExecute);
        }

        public IEnumerable<DesignerItem> SelectedContainers => selectedContainers.Value;

        public ReactiveCommand<Unit, Unit> AlignToTopCommand { get; }

        public ReactiveCommand<Unit, Unit> AlignToRightCommand { get; }

        public ReactiveCommand<Unit, Unit> AlignToLeftCommand { get; }

        public ReactiveCommand<Unit, Unit> AlignToBottomCommand { get; }

        private void AlignToTop(IList<DesignerItem> graphics)
        {
            var x = graphics.Min(g => g.Top);
            foreach (var g in graphics)
            {
                g.Top = x;
            }
        }

        private void AlignToLeft(IList<DesignerItem> graphics)
        {
            var x = graphics.Min(g => g.Left);
            foreach (var g in graphics)
            {
                g.Left = x;
            }
        }

        private void AlignToRight(IList<DesignerItem> graphics)
        {
            var x = graphics.Max(g => g.Left + g.Width);
            foreach (var g in graphics)
            {
                g.Left = x - g.Width;
            }
        }

        private void AlignToBottom(IList<DesignerItem> graphics)
        {
            var x = graphics.Max(g => g.Top + g.Height);
            foreach (var g in graphics)
            {
                g.Top = x - g.Height;
            }
        }
    }

}