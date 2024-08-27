namespace Rseg.Mvvm.Examples.CommandContext;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}