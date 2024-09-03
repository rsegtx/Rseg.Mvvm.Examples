using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Rseg.Mvvm.Examples.CommandContext;

public class BaseViewModel : ObservableObject
{
    private bool _isBusy = false;
    public bool IsBusy
    {
        get => _isBusy;
        protected set => SetProperty(ref _isBusy, value);
    }

    /// <summary>
    /// Waypoint 9
    /// HandleException() provides a common way of handling exceptions. Its defined as virtual so that
    /// specific ViewModels or projects might customize how exception handling is implemented.
    /// </summary>
    protected virtual async Task HandleException(Exception ex, bool showModalMessage=true)
    {
        //MJ_ I would also probably abstract this behind a IExceptioHandler
        //    typically users will want the flexibility to implement their own thing like error logging etc - but also have that
        //    available outside of just viewmodels
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
        // MJ_ using this is opinionated, and probably not good for unit testing.
        //     perhaps consider a IUserNotifier interface dependency. Can be mocked, and users can provide their own implementation
        await App.Current.MainPage.DisplayAlert(title, message, "OK");
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
        //MJ_ the 2 methods could be refactored into a common handler. that way when you edit, it's done in a single place
        await PerformHandler(null, syncAction: action, showAsBusy, showErrorAsModal);
    }    
    
    protected virtual async Task PerformHandler(Func<Task> action, bool showAsBusy = true, bool showErrorAsModal = true)
    {
        await PerformHandler(asyncAction: action, null, showAsBusy, showErrorAsModal);
    }


    // MJ_ something to consider: Composition vs Inheritance. The only hard VM dependency here is the IsBusy property
    //     so something that might be more extensible, esp for existing messy code bases that have their own base class implementations..
    //     is just a CommandHandler.Execute(ICommandOwner owner, Action action) etc... where ICommandOwner just defines IsBusy property
    private async Task PerformHandler(Func<Task>? asyncAction = null, Action? syncAction = null, bool showAsBusy = true, bool showErrorAsModal = true)
    {
        if (asyncAction == null && syncAction == null)
            throw new InvalidOperationException("You must send an action to execute!");

        // MJ_ what if IsBusy already true? because happy tapping causing race conditions?

        // MJ_ consider BusyMessage to compliment IsBusy? It's fairly common when UI is in busy state there's progress indicator + potetially a message showing

        try
        {
            if (showAsBusy)
                IsBusy = true;

            if (asyncAction != null)
            {
                await asyncAction.Invoke();
            }
            else
            {
                syncAction?.Invoke();
            }
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