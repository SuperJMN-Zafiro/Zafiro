namespace Zafiro.UI.Wizards.Slim;

/// <summary>
/// Indicates the behavior of a wizard step, especially the last one.
/// </summary>
public enum StepKind
{
    /// <summary>Standard step.</summary>
    Normal,
    /// <summary>Final step that still allows going back.</summary>
    Commit,
    /// <summary>Final step that prevents going back.</summary>
    Completion,
}
