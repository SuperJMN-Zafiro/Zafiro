using CSharpFunctionalExtensions;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.UI.Navigation;

public interface ISectionSessionFactory
{
    Task<Result<SectionScope>> Create(IContentSection section);
}