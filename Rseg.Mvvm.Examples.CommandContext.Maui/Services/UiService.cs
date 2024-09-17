using Rseg.Mvvm.Examples.CommandContext.ViewModels.Services;

namespace Rseg.Mvvm.Examples.CommandContext.Maui.Services;

public class UiService : IUiService
{
    public async Task DisplayAlert(string title, string message, string buttonText)
    {
        await App.Current.MainPage.DisplayAlert(title, message, buttonText);
    }
}