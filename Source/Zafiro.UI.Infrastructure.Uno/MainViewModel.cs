using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using SampleApp.Infrastructure.Navigation;

namespace SampleApp.Infrastructure
{
    public class MainViewModel : ReactiveObject
    {
        private readonly INavigation navigation;
        private readonly ObservableAsPropertyHelper<bool> canGoBack;


        public MainViewModel(INavigation navigation, IEnumerable<Section> sections)
        {
            this.navigation = navigation;
            GoToDefault = ReactiveCommand.CreateFromTask(() => navigation.Go(Sections.First().ViewModelType));
            ItemInvoked = ReactiveCommand.CreateFromTask<Section>(NavigateTo);
            
            canGoBack = navigation.CanGoBack.ToProperty(this, x => x.CanGoBack);
            GoBack = ReactiveCommand.Create(navigation.GoBack);

            Sections = sections;
            SelectedItem = sections.First();
        }

        private Task NavigateTo(Section s)
        {
            return navigation.Go(s.ViewModelType);
        }

        public ReactiveCommand<Section, Unit> ItemInvoked { get; }

        public ReactiveCommand<Unit, Unit> GoToDefault { get; }

        public IEnumerable<Section> Sections { get; }

        public ReactiveCommand<Unit, Task> GoBack { get; }

        public bool CanGoBack => canGoBack.Value;

        public object SelectedItem { get; set; }
    }
}