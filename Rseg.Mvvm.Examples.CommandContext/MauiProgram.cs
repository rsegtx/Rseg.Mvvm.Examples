using Microsoft.Extensions.Logging;

namespace Rseg.Mvvm.Examples.CommandContext;

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
        
        return builder.Build();
    }
}