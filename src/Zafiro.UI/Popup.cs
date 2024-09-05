namespace Zafiro.UI;

public class Popup : IPopup
{
    private readonly Func<IView> popupFactory;

    public Popup(Func<IView> popupFactory)
    {
        this.popupFactory = popupFactory;
    }

    public async Task ShowAsModal<T>(IHaveDataContext content, T viewModel, Action<ViewConfiguration<T>> configure)
    {
        content.SetDataContext(viewModel);
        var popup = popupFactory();
        var config = new ViewConfiguration<T>(popup, viewModel);
        configure(config);

        popup.SetContext(new PopupModel(content.Object, config.View.Title, config.Options));
        await popup.ShowAsModal();
    }
}