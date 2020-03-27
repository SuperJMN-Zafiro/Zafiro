using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Serilog;
using Zafiro.Core.UI;
using Zafiro.Uno.Infrastructure.Navigation;

namespace Zafiro.Uno.Infrastructure
{
    public abstract class CompositionBase
    {
        public CompositionBase()
        {
            var container = new DependencyInjectionContainer();

            ConfigureServices(container);
            
            var viewMaps = new Dictionary<Type, Type>();
            ConfigureViewModelToViewMaps(viewMaps);

            var sections = new List<Section>();
            ConfigureSections(sections);

            ConfigureInfrastructureServices(container, viewMaps);

            ConfigureLogger();

            container.Configure(c =>
            {
                c.Export<MainViewModel>().WithCtorParam(() => sections);
            });

            Root = container.Locate<MainViewModel>();
        }

        private void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .CreateLogger();
        }

        private void ConfigureInfrastructureServices(DependencyInjectionContainer container, Dictionary<Type, Type> sections)
        { container.Configure(c =>
            {
                c.Export<DialogService>().As<IDialogService>();
                c.Export<Navigation.Navigation>().As<INavigation>().WithCtorParam(() => sections);
            });
        }

        public MainViewModel Root { get; }

        protected virtual void ConfigureViewModelToViewMaps(IDictionary<Type, Type> map)
        {
        }

        protected virtual void ConfigureServices(DependencyInjectionContainer container)
        {
        }

        protected virtual void ConfigureSections(List<Section> mappings)
        {
        }
    }
}