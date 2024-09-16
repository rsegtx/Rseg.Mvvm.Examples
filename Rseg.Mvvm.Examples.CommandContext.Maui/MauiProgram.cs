using Microsoft.Extensions.Logging;
using Rseg.Mvvm.Examples.CommandContext.ViewModels;

namespace Rseg.Mvvm.Examples.CommandContext.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // register top level pages/viewmodels so that Shell can create them using DI
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<IUiService>(new UiService());
        
        return builder.Build();
    }
}