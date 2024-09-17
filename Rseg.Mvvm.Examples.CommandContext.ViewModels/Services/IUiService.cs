namespace Rseg.Mvvm.Examples.CommandContext.ViewModels.Services;

public interface IUiService
{
    Task DisplayAlert(string title, string message, string buttonText);
}