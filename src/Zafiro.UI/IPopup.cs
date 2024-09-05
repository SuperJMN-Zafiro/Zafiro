namespace Zafiro.UI;

public interface IPopup
{
    Task ShowAsModal<T>(IHaveDataContext content, T viewModel, Action<ViewConfiguration<T>> configure);
}