namespace Zafiro.UI.Wizards.Slim;

/// <summary>
/// Represents a UI page within the Slim wizard flow.
/// </summary>
public interface IPage
{
    /// <summary>Gets the view model or content associated with the page.</summary>
    object Content { get; }

    /// <summary>Gets the page title.</summary>
    string Title { get; }

    /// <summary>Gets the zero-based page index within the wizard.</summary>
    public int Index { get; }
}
