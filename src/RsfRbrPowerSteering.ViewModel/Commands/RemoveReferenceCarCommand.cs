using RsfRbrPowerSteering.ViewModel.Interfaces;

namespace RsfRbrPowerSteering.ViewModel.Commands;

internal class RemoveReferenceCarCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;

    public RemoveReferenceCarCommand(
        ICommandManager commandManager,
        MainViewModel mainViewModel)
        : base(commandManager)
    {
        _mainViewModel = mainViewModel;
    }

    public override void Execute(object? parameter)
    {
        if (parameter is CarTemplateViewModel template)
        {
            _mainViewModel.RemoveReferenceCar(template);
        }
    }
}
