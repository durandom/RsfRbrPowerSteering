using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class AddReferenceCarCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;

    public AddReferenceCarCommand(
        ICommandManager commandManager,
        MainViewModel mainViewModel) 
        : base(commandManager)
    {
        _mainViewModel = mainViewModel;
    }

    public override void Execute(object? parameter)
    {
        _mainViewModel.AddReferenceCar();
    }
}
