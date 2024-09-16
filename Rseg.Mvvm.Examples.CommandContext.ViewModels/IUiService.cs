namespace Rseg.Mvvm.Examples.CommandContext.ViewModels;

public interface IUiService
{
    Task DisplayAlert(string title, string message, string buttonText);
}