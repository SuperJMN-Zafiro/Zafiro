using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.SourceGenerators;
using Zafiro.Reactive;
using Zafiro.UI.Navigation;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Shell;

[PublicAPI]
public partial class Shell : ReactiveObject, IShell
{
    private readonly CompositeDisposable disposables = new();
    private readonly IServiceProvider provider;
    private readonly Dictionary<IContentSection, SectionScope> sessions = new();

    private INavigator navigator;
    [Reactive] private IContentSection? selectedSection;

    public Shell(ShellProperties shellProperties, IEnumerable<ISection> sections, IServiceProvider provider)
    {
        this.provider = provider;
        Sections = sections;

        // Header follows the current navigator
        ContentHeader = this.WhenAnyValue(x => x.Navigator)
            .WhereNotNull()
            .Select(nav => shellProperties.GetHeader(nav))
            .Switch()
            .ReplayLastActive();

        // When section changes, switch to (or create) its own navigator session
        this.WhenAnyValue(x => x.SelectedSection)
            .WhereNotNull()
            .DistinctUntilChanged()
            .Subscribe(contentSection =>
            {
                if (!sessions.TryGetValue(contentSection, out var session))
                {
                    session = new SectionScope(this.provider, contentSection.RootType);
                    sessions[contentSection] = session;
                }

                Navigator = session.Navigator;

                // Always trigger initial navigation on section change, deferred one tick to avoid construction-time cycles
                RxApp.MainThreadScheduler.Schedule(session, static (_, s) =>
                {
                    s.LoadInitial.Execute().Subscribe();
                    return Disposable.Empty;
                });
            })
            .DisposeWith(disposables);

        // Listen to section change requests, if the action bus is registered
        var sectionActions = provider.GetService<ISectionActions>();
        if (sectionActions is not null)
        {
            sectionActions.GoToSectionRequests
                .Subscribe(GoToSection)
                .DisposeWith(disposables);
        }

        SelectedSection = Sections.OfType<IContentSection>().FirstOrDefault();
        Header = shellProperties.Header;
    }

    public INavigator Navigator
    {
        get => navigator;
        private set => this.RaiseAndSetIfChanged(ref navigator, value);
    }

    public object Header { get; set; }
    public IObservable<object?> ContentHeader { get; }
    public IEnumerable<ISection> Sections { get; }

    public void GoToSection(string sectionName)
    {
        SelectedSection = Sections.OfType<IContentSection>().First(x => x.Name == sectionName);
    }
}