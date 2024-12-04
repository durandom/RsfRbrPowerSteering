using RsfRbrPowerSteering.ViewModel.Interfaces;
using System.Windows.Input;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal abstract class CommandBase : ICommand
{
    private readonly ICommandManager _commandManager;

    protected CommandBase(ICommandManager commandManager)
    {
        _commandManager = commandManager;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => _commandManager.RequerySuggested += value;
        remove => _commandManager.RequerySuggested -= value;
    }

    public virtual bool CanExecute(object? parameter) => true;

    public abstract void Execute(object? parameter);
}
