using Rseg.Mvvm.Examples.CommandContext.ViewModels;

namespace Rseg.Mvvm.Examples.CommandContext.Maui;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}