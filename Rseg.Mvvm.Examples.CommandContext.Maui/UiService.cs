using Rseg.Mvvm.Examples.CommandContext.ViewModels;

namespace Rseg.Mvvm.Examples.CommandContext.Maui;

public class UiService : IUiService
{
    public async Task DisplayAlert(string title, string message, string buttonText)
    {
        await App.Current.MainPage.DisplayAlert(title, message, buttonText);
    }
}