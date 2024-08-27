using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Rseg.Mvvm.Examples.CommandContext;

public enum ExceptionProcessing
{
    NoException,
    ThrowException,
    ThrowCustomException
}

public class CustomException : Exception
{
}

public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(Action1Command))]
    [NotifyCanExecuteChangedFor(nameof(Action2Command))]
    [NotifyCanExecuteChangedFor(nameof(Action3Command))]
    [NotifyCanExecuteChangedFor(nameof(Action11Command))]
    [NotifyCanExecuteChangedFor(nameof(Action12Command))]
    [NotifyCanExecuteChangedFor(nameof(Action13Command))]
    [NotifyCanExecuteChangedFor(nameof(Task1Command))]
    [NotifyCanExecuteChangedFor(nameof(Task2Command))]
    [NotifyCanExecuteChangedFor(nameof(Task3Command))]
    [NotifyCanExecuteChangedFor(nameof(Task11Command))]
    [NotifyCanExecuteChangedFor(nameof(Task12Command))]
    [NotifyCanExecuteChangedFor(nameof(Task13Command))]
    private bool _canExecute = true;
    
    [ObservableProperty]
    private ExceptionProcessing _exceptionOption = ExceptionProcessing.NoException;
    
    public ObservableCollection<string> Items { get; set; } = new ()
    {
        { "Item 1" },
        { "Item 2" },
        { "Item 3" }
    };
    
    public MainViewModel()
    {
        Action2Command = SetupCommand(Action2, CanExecuteCommand);
        Action3Command = SetupCommand(Action3, CanExecuteCommand);
        Action12Command = SetupCommand<string>(Action12, CanExecuteCommand1);
        Action13Command = SetupCommand<string>(Action13, CanExecuteCommand1);
        Task2Command = SetupCommand(Task2, CanExecuteCommand);
        Task3Command = SetupCommand(Task3, CanExecuteCommand);
        Task12Command = SetupCommand<string>(Task12, CanExecuteCommand1);
        Task13Command = SetupCommand<string>(Task13, CanExecuteCommand1);
    }
    
    private bool CanExecuteCommand()
    {
        return CanExecute;
    }
    
    private bool CanExecuteCommand1(string value)
    {
        return CanExecute && !string.IsNullOrEmpty(value);
    }
    
    /// <summary>
    /// Waypoint 3
    /// Action1 demonstrates using the [RelayCommand] attribute to generate the command definition
    /// and then use PerformHandler() within the handler body to perform the logic within the
    /// common context.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteCommand))]
    private async Task Action1()
    {
        await PerformHandler(() =>
        {
            DoSomeActionLogic();
        });
    }
    
    /// <summary>
    /// Waypoint 4
    /// Action2 demonstrates explicitly defining the command and using SetupCommand() in the view models
    /// constructor to configure the command.
    /// </summary>
    public IAsyncRelayCommand Action2Command { get; protected set; }
    private void Action2()
    {
        DoSomeActionLogic();
    }
    
    /// <summary>
    /// Waypoint 5
    /// Action3 demonstrates how to add specific exception handling in command handler and allow
    /// the default context exception handling to handle everything else.
    /// </summary>
    public IAsyncRelayCommand Action3Command { get; protected set; }
    private void Action3()
    {
        try
        {
            DoSomeActionLogic();
        }
        catch (CustomException)
        {
            Console.WriteLine("CustomException handled in Action3...");
        }
    }
    
    /// <summary>
    /// Waypoint 6
    /// Action11 through Action13 build on the previous action exception they are commands
    /// that accept a command parameter
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteCommand1))]
    private async Task Action11(string item)
    {
        await PerformHandler(() =>
        {
            DoSomeActionLogic(item);
        });
    }    
    
    public IAsyncRelayCommand<string> Action12Command { get; protected set; }
    private void Action12(string item)
    {
        DoSomeActionLogic(item);
    }    
    
    public IAsyncRelayCommand<string> Action13Command { get; protected set; }
    private void Action13(string item)
    {
        try
        {
            DoSomeActionLogic(item);
        }
        catch (CustomException)
        {
            Console.WriteLine($"CustomException handled in Action13 for item {item}...");
        }
    }    
    
    /// <summary>
    /// Waypoint 7
    /// Task1 through Task13 build on what has been described in Action1 through Action13 exception
    /// use async Task handlers.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteCommand))]
    private async Task Task1()
    {
        await PerformHandler(async () =>
        {
            await DoSomeTaskLogic();
        });
    }
    
    public IAsyncRelayCommand Task2Command { get; protected set; }
    private async Task Task2()
    {
        await DoSomeTaskLogic();
    }
    
    public IAsyncRelayCommand Task3Command { get; protected set; }
    private async Task Task3()
    {
        try
        {
            await DoSomeTaskLogic();
        }
        catch (CustomException)
        {
            Console.WriteLine("CustomException handled in Task3...");
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteCommand1))]
    private async Task Task11(string item)
    {
        await PerformHandler(async () =>
        {
            await DoSomeTaskLogic(item);
        });
    }
    
    public IAsyncRelayCommand<string> Task12Command { get; protected set; }
    private async Task Task12(string item)
    {
        await DoSomeTaskLogic(item);
    }
    
    public IAsyncRelayCommand<string> Task13Command { get; protected set; }
    private async Task Task13(string item)
    {
        try
        {
            await DoSomeTaskLogic(item);
        }
        catch (CustomException)
        {
            Console.WriteLine($"CustomException handled in Task3 for item {item}...");
        }
    }    
    
    /// <summary>
    /// Waypoint 8
    /// DoSomeActionLogic() and DoSomeTaskLoic() are simply methods the represent placeholder logic; these
    /// would not be needed in an actual app.
    /// </summary>
    /// <param name="callerName"></param>
    /// <exception cref="Exception"></exception>
    /// <exception cref="CustomException"></exception>
    private void DoSomeActionLogic([CallerMemberName]string callerName="Action")
    {
        switch (ExceptionOption)
        {
            default:
            case ExceptionProcessing.NoException:
                Console.WriteLine($"{callerName} completed...");
                break;

            case ExceptionProcessing.ThrowException:
                throw new Exception($"Something bad happened in {callerName}...");

            case ExceptionProcessing.ThrowCustomException:
                throw new CustomException();
        }
    }    
    
    private void DoSomeActionLogic(string item, [CallerMemberName]string callerName="Action")
    {
        switch (ExceptionOption)
        {
            default:
            case ExceptionProcessing.NoException:
                Console.WriteLine($"{callerName} completed...");
                break;

            case ExceptionProcessing.ThrowException:
                throw new Exception($"Something bad happened in {callerName}...");

            case ExceptionProcessing.ThrowCustomException:
                throw new CustomException();
        }
    }
    
    private async Task DoSomeTaskLogic([CallerMemberName]string callerName="Task")
    {
        await Task.Delay(1000);
        switch (ExceptionOption)
        {
            default:
            case ExceptionProcessing.NoException:
                Console.WriteLine($"{callerName} completed...");
                break;

            case ExceptionProcessing.ThrowException:
                throw new Exception($"Something bad happened in {callerName}...");

            case ExceptionProcessing.ThrowCustomException:
                throw new CustomException();
        }
    }    
    
    private async Task DoSomeTaskLogic(string item, [CallerMemberName]string callerName="Task")
    {
        await Task.Delay(1000);
        switch (ExceptionOption)
        {
            default:
            case ExceptionProcessing.NoException:
                Console.WriteLine($"{callerName} completed...");
                break;

            case ExceptionProcessing.ThrowException:
                throw new Exception($"Something bad happened in {callerName}...");

            case ExceptionProcessing.ThrowCustomException:
                throw new CustomException();
        }
    }
}