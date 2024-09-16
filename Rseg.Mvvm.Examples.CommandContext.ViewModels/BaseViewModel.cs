using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Rseg.Mvvm.Examples.CommandContext.ViewModels;

public class BaseViewModel : ObservableObject
{
    public IUiService UiService { get; private set; }
    
    private bool _isBusy = false;
    public bool IsBusy
    {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    public BaseViewModel(IUiService uiService)
    {
        UiService = uiService;
    }
    
    /// <summary>
    /// Waypoint 9
    /// HandleException() provides a common way of handling exceptions. Its defined as virtual so that
    /// specific ViewModels or projects might customize how exception handling is implemented.
    /// </summary>
    protected virtual async Task HandleException(Exception ex, bool showModalMessage=true)
    {
        System.Diagnostics.Debug.WriteLine($"--- {ex.GetType().Name} handled in BaseViewModel.HandleException()");
        if (showModalMessage)
            await DisplayAlert("Error", "An error occured.");
    }    
    
    /// <summary>
    /// Waypoint 10
    /// This is a hack which uses MAUI ability to display an alert. Normally, I would not want
    /// to reference any UI framework features in a ViewModel but I used this hack for convenience.
    /// I hope to develop some followup examples with better ways of dealing with UI and navigation
    /// from view models.
    /// </summary>
    public async Task DisplayAlert(string title, string message)
    {
        await UiService.DisplayAlert(title, message, "OK");
    }

    /// <summary>
    /// Waypoint 2
    /// A SetupCommand() methods are provided to configure commands.
    /// </summary>
    #region SetupCommand methods
    protected IAsyncRelayCommand SetupCommand(Action action = null, Func<bool> canExecute = null,
        bool showAsBusy = true, bool showErrorsAsModal = true)
    {
        async Task Execute()
        {
            await PerformHandler(action, showAsBusy, showErrorsAsModal);
        }

        return canExecute != null
            ? new AsyncRelayCommand(Execute, canExecute)
            : new AsyncRelayCommand(Execute);
    }
    
    protected IAsyncRelayCommand<T> SetupCommand<T>(Action<T> action, Predicate<T> canExecute = null,
        bool showAsBusy = true, bool showErrorsAsModal = true)
    {
        async Task Execute(T argument)
        {
            await PerformHandler(() =>
            {
                action.Invoke(argument);
            }, showAsBusy, showErrorsAsModal);
        }

        return canExecute != null
            ? new AsyncRelayCommand<T>(Execute, canExecute)
            : new AsyncRelayCommand<T>(Execute);
    }    
    
    protected IAsyncRelayCommand SetupCommand(Func<Task> action, Func<bool> canExecute = null,
        bool showAsBusy = true, bool showErrorsAsModal = true)
    {
        async Task Execute()
        {
            await PerformHandler(action, showAsBusy, showErrorsAsModal);
        }

        return canExecute != null
            ? new AsyncRelayCommand(Execute, canExecute)
            : new AsyncRelayCommand(Execute);
    }
    
    protected IAsyncRelayCommand<T> SetupCommand<T>(Func<T, Task> action, Predicate<T> canExecute = null,
        bool showAsBusy = true, bool showErrorsAsModal = true)
    {
        async Task Execute(T argument)
        {
            await PerformHandler(async () =>
            {
                await action.Invoke(argument);
            }, showAsBusy, showErrorsAsModal);
        }
        
        return canExecute != null
            ? new AsyncRelayCommand<T>(Execute, canExecute)
            : new AsyncRelayCommand<T>(Execute);
    }
    #endregion

    /// <summary>
    /// Waypoint 1
    /// Two overloads for PerformHandler are defined, one to handle simple actions and the other to handle
    /// async tasks.
    /// </summary>
    #region PerformActionInContext methods
    protected virtual async Task PerformHandler(Action action, bool showAsBusy = true, bool showErrorAsModal = true)
    {
        try
        {
            if (showAsBusy)
                IsBusy = true;

            action.Invoke();
        }
        catch (Exception ex)
        {
            await HandleException(ex, showErrorAsModal);
        }
        finally
        {
            if (showAsBusy)
                IsBusy = false;
        }
    }    
    
    protected virtual async Task PerformHandler(Func<Task> action, bool showAsBusy = true, bool showErrorAsModal = true)
    {
        try
        {
            if (showAsBusy)
                IsBusy = true;

            await action.Invoke();
        }
        catch (Exception ex)
        {
            await HandleException(ex, showErrorAsModal);
        }
        finally
        {
            if (showAsBusy)
                IsBusy = false;
        }
    }
    #endregion
}