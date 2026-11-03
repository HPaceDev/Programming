using System.Windows.Input;

namespace ViewModel;

public sealed class SaveCommand : ICommand
{
    private readonly Action action;
    public SaveCommand(Action action) => this.action = action;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => action();

    // У этой команды доступность постоянна.
    public event EventHandler? CanExecuteChanged { add { } remove { } }
}
