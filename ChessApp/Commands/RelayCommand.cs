using System.Windows.Input;

namespace ChessApp.Commands;

public class RelayCommand : ICommand
{
    // Action that will be executed while calling command
    private readonly Action<object> _execute;
    // Function that checks if command can be executed
    private readonly Predicate<object> _canExecute;

    /// <summary>
    /// Command constructor
    /// </summary>
    /// <param name="execute">Delegate that handles command execution</param>
    /// <param name="canExecute">Function that checks if we can execute command ( by default - true )</param>
    /// <exception cref="ArgumentNullException">Check if passed value is not null</exception>
    public RelayCommand(Action<object> execute , Predicate<object> canExecute = null)
    {
        _execute = execute  ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    /// <summary>
    /// Defines if command can be executed
    /// </summary>
    /// <param name="parameter">Parameter of command(can be null)</param>
    /// <returns>Returns true by default</returns>
    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
    
    /// <summary>
    /// Executes command
    /// </summary>
    /// <param name="parameter">Parameter of command(can be null)</param>
    public void Execute(object parameter) => _execute(parameter);
    
    /// <summary>
    /// An event that occurs when the conditions for executing a command are met.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        // Add remove event handlers to automatically update the command state
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}