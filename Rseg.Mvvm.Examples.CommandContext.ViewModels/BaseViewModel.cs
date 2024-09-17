using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Rseg.Mvvm.Examples.CommandContext.ViewModels.Services;

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
            await UiService.DisplayAlert("Error", "An error occured.", "OK");
    }

    /// <summary>
    /// Waypoint 2
    /// SetupCommand() methods are provided to configure commands. These methods are generally
    /// called in a view model constructor.
    /// </summary>
    #region SetupCommand methods
    protected IAsyncRelayCommand SetupCommand(Action action = null, Func<bool> canExecute = null,
        bool showAsBusy = true, bool showErrorsAsModal = true)
    {
        async Task Execute()
        {
            await PerformInContext(action, showAsBusy, showErrorsAsModal);
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
            await PerformInContext(() =>
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
            await PerformInContext(action, showAsBusy, showErrorsAsModal);
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
            await PerformInContext(async () =>
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
    /// Two overloads for PerformInContext are defined, one to handle simple actions and the other to handle
    /// async tasks. Note that the action version simply converts the action to a task and executes in the overload
    /// that accepts a task. This mean if you want to customize the execution context you only need to override a
    /// single method.
    /// </summary>
    #region PerformActionInContext methods
    protected virtual async Task PerformInContext(Action action, bool showAsBusy = true, bool showErrorAsModal = true)
    {
        var task = Task () =>
        {
            action?.Invoke();
            return Task.CompletedTask;
        };

        await PerformInContext(task, showAsBusy, showErrorAsModal);
    }
    
    protected virtual async Task PerformInContext(Func<Task> action, bool showAsBusy = true, bool showErrorAsModal = true)
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