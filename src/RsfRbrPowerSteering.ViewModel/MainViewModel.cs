﻿using RsfRbrPowerSteering.Model;
using RsfRbrPowerSteering.Model.Calculation;
using RsfRbrPowerSteering.Model.Rsf;
using RsfRbrPowerSteering.Settings;
using RsfRbrPowerSteering.ViewModel.Commands;
using RsfRbrPowerSteering.ViewModel.Interfaces;
using System.Collections.ObjectModel;
using System.Reflection;

namespace RsfRbrPowerSteering.ViewModel;

public class MainViewModel : NotifyPropertyChangedBase
{
    private readonly PersonalData _personalData = new PersonalData();
    IReadOnlyDictionary<int, CarInfo>? _carInfos;
    private int _lockToLockRotationMinimum = 0;
    private int _lockToLockRotationMaximum = 0;
    private bool _isExclusiveCommandRunning;
    private bool _isInformationVisible = true;
    private bool _isReCalcuationEnabled = false;
    private CarViewModel? _targetCar;

    public MainViewModel(
        ICommandManager commandManager,
        IMessageService messageService)
    {
        Commands = new ViewModelCommands(
            commandManager,
            messageService,
            this);
        Cars.Add(DefaultCar);
        Adjustments = new AdjustmentsViewModel(this);
        // Add initial two reference cars
        ReferenceTemplates.Add(new CarTemplateViewModel(this));
        ReferenceTemplates.Add(new CarTemplateViewModel(this));
        FfbSensRangeMessage = string.Format(ViewModelTexts.RangeMessageFormat, FfbSensMinimum, FfbSensMaximum);
        Version? version = Assembly.GetEntryAssembly()?.GetName()?.Version;
        WindowTitle = string.Format(
            ViewModelTexts.WindowTitleFormat,
            string.Format(
                ViewModelTexts.VersionTextFormat,
                version?.Major,
                version?.Minor,
                version?.Build));
    }

    public event Action? LockToLockRotationsChanged;

    public string WindowTitle { get; }
    public int FfbSensMinimum { get; } = 10;
    public int FfbSensMaximum { get; } = 5000;
    public string FfbSensRangeMessage { get; }
    public ViewModelCommands Commands { get; }

    public bool IsDescriptionVisible
    {
        get => _isInformationVisible;

        set
        {
            _isInformationVisible = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(IsDescriptionHidden));
        }
    }

    public bool IsDescriptionHidden => !_isInformationVisible;

    public CarViewModel DefaultCar { get; } = new CarViewModel();

    public int LockToLockRotationMinimumForComboBox
    {
        get => _lockToLockRotationMinimum;
        set
        {
            if (_lockToLockRotationMinimum == value)
            {
                return;
            }

            _lockToLockRotationMinimum = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(LockToLockRotationMinimumForSlider));
        }
    }

    public int LockToLockRotationMaximumForComboBox
    {
        get => _lockToLockRotationMaximum;
        set
        {
            if (_lockToLockRotationMaximum == value)
            {
                return;
            }

            _lockToLockRotationMaximum = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(LockToLockRotationMaximumForSlider));
        }
    }

    public double LockToLockRotationMinimumForSlider => _lockToLockRotationMinimum;

    public double LockToLockRotationMaximumForSlider => _lockToLockRotationMaximum;

    public bool IsExclusiveCommandRunning
    {
        get => _isExclusiveCommandRunning;

        set
        {
            if (_isExclusiveCommandRunning == value)
            {
                return;
            }

            _isExclusiveCommandRunning = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(IsNoExclusiveCommandRunning));
        }
    }

    public bool IsNoExclusiveCommandRunning => !_isExclusiveCommandRunning;

    public CarViewModel? TargetCar
    {
        get => _targetCar;
        set
        {
            if (value == DefaultCar)
            {
                value = null;
            }

            if (_targetCar == value)
            {
                return;
            }

            bool isTargetCarIsSelectedChanged =
                _targetCar == null && value != null
                || _targetCar != null && value == null;
            _targetCar = value;
            NotifyPropertyChanged();

            if (isTargetCarIsSelectedChanged)
            {
                NotifyPropertyChanged(nameof(TargetCarIsSelected));
                NotifyPropertyChanged(nameof(TargetCarIsUnselected));
            }
        }
    }

    public bool TargetCarIsSelected => _targetCar != null;

    public bool TargetCarIsUnselected
    {
        get
        {
            return _targetCar == null;
        }

        set
        {
            if (value && _targetCar != null)
            {
                TargetCar = null;
            }
        }
    }

    private readonly Dictionary<int, CarViewModel> _carsById = new Dictionary<int, CarViewModel>();
    internal IReadOnlyDictionary<int, CarViewModel> CarsById => _carsById;

    private readonly Dictionary<int, LockToLockRotationViewModel> _lockToLockRotationsByIntValue = new Dictionary<int, LockToLockRotationViewModel>();
    internal IReadOnlyDictionary<int, LockToLockRotationViewModel> LockToLockRotationsByIntValue => _lockToLockRotationsByIntValue;

    public ObservableCollection<CarViewModel> Cars { get; } = new ObservableCollection<CarViewModel>();
    public ObservableCollection<LockToLockRotationViewModel> LockToLockRotations { get; } = new ObservableCollection<LockToLockRotationViewModel>();
    public AdjustmentsViewModel Adjustments { get; }
    public ObservableCollection<CarTemplateViewModel> ReferenceTemplates { get; } = new();

    // Removed PrimaryTemplate and SecondaryTemplate properties since we now use ReferenceTemplates collection directly

    internal void ToggleDescriptionVisibility()
        => IsDescriptionVisible = !IsDescriptionVisible;

    internal async Task LoadCarsAsync(bool reCalculate = true)
    {
        _isReCalcuationEnabled = false;
        _carInfos = await CarInfo.ReadCarsAsync();
        _personalData.ReadFile();
        var carViewModels = Cars.ToDictionary(car => car.Id);
        var carIds = new HashSet<int>();
        var lockToLockRotations = new HashSet<int>();

        foreach ((int carId, PersonalCarFfbSens ffbSens, int? personalLockToLockRotation) in _personalData.GetCars())
        {
            if (!_carInfos.TryGetValue(carId, out CarInfo car))
            {
                // Unknown car.
                continue;
            }

            if (!carViewModels.TryGetValue(carId, out CarViewModel? carViewModel))
            {
                // Add new car:
                carViewModel = new CarViewModel(car);

                if (personalLockToLockRotation.HasValue)
                {
                    carViewModel.LockToLockRotation = personalLockToLockRotation.Value;
                }

                _carsById[carId] = carViewModel;
                Cars.Add(carViewModel);
            }

            lockToLockRotations.Add(car.LockToLockRotation);

            // Set sensitivities:
            carViewModel.FfbSensPersonal.ApplyFfbSens(ffbSens);

            if (ReferenceTemplates.Any(t => t.SelectedCarId == carId))
            {
                var template = ReferenceTemplates.First(t => t.SelectedCarId == carId);
                template.ApplyFfbSens(ffbSens);
            }

            carIds.Add(carId);
        }

        // Remove cars no longer available:
        for (int i = Cars.Count - 1; i >= 0; i--)
        {
            CarViewModel carViewModel = Cars[i];

            if (carViewModel.Id != DefaultCar.Id && !carIds.Contains(carViewModel.Id))
            {
                _carsById.Remove(carViewModel.Id);
                Cars.RemoveAt(i);
            }
        }

        LockToLockRotationMinimumForComboBox = lockToLockRotations.Min();
        LockToLockRotationMaximumForComboBox = lockToLockRotations.Max();
        bool wereLockToLockRotationsChanged = false;

        // Remove lock-to-lock rotations no longer available:
        for (int i = LockToLockRotations.Count - 1; i >= 0; i--)
        {
            LockToLockRotationViewModel lockToLockRotation = LockToLockRotations[i];

            if (!lockToLockRotations.Contains(lockToLockRotation.IntValue))
            {
                _lockToLockRotationsByIntValue.Remove(lockToLockRotation.IntValue);
                LockToLockRotations.RemoveAt(i);
                wereLockToLockRotationsChanged = true;
            }
        }

        // Add new lock-to-lock rotations:
        int j = 0;

        foreach (int lockToLockRotation in lockToLockRotations
            .Except(LockToLockRotations.Select(l => l.IntValue))
            .Order())
        {
            var newLockToLockRotation = new LockToLockRotationViewModel(lockToLockRotation);
            _lockToLockRotationsByIntValue[lockToLockRotation] = newLockToLockRotation;

            if (j < LockToLockRotations.Count)
            {
                LockToLockRotationViewModel existingLockToLockRotation;

                do
                {
                    existingLockToLockRotation = LockToLockRotations[j];
                    j++;
                }
                while (lockToLockRotation < existingLockToLockRotation.IntValue && j < LockToLockRotations.Count);
            }

            LockToLockRotations.Insert(j, new LockToLockRotationViewModel(lockToLockRotation));
            wereLockToLockRotationsChanged = true;
            j++;
        }

        if (wereLockToLockRotationsChanged)
        {
            LockToLockRotationsChanged?.Invoke();
        }

        _isReCalcuationEnabled = true;

        if (reCalculate)
        {
            ReCalculate();
        }
    }

    private IReadOnlyDictionary<int, CarFfbSens> CalculateFfbSenses(IEnumerable<CarInfo> carInfos)
    {
        if (!_isReCalcuationEnabled)
        {
            return new Dictionary<int, CarFfbSens>();
        }

        return CalculationUtility.CalculateFfbSenses(
            carInfos,
            Adjustments.WeightRatio / 100M,
            new DrivetrainFactors(Adjustments.Fwd / 100M, Adjustments.Rwd / 100M, Adjustments.Awd / 100M),
            ReferenceTemplates.Select(t => t.ToCalculationCar()));
    }

    internal void ReCalculate()
    {
        if (!_isReCalcuationEnabled || _carInfos == null)
        {
            return;
        }

        IReadOnlyDictionary<int, CarFfbSens> ffbSenses = CalculateFfbSenses(_carInfos.Values);

        foreach ((int id, CarFfbSens ffbSens) in ffbSenses)
        {
            if (_carsById.TryGetValue(id, out CarViewModel? car))
            {
                car.FfbSensCalculated.ApplyFfbSens(ffbSens.ToPersonal());
            }
        }
    }

    internal async Task LoadSettingsAsync()
    {
        RootSettings? settings = await RootSettings.LoadAsync();

        if (settings != null)
        {
            _isReCalcuationEnabled = false;
            TargetCar = settings.TargetCarId.HasValue
                ? _carsById.TryGetValue(settings.TargetCarId.Value, out CarViewModel? targetCar)
                    ? targetCar
                    : null
                : null;
            IsDescriptionVisible = settings.IsDescriptionVisible;
            Adjustments.PrimarySurface = string.IsNullOrEmpty(settings.PrimarySurface)
                ? null
                : Enum.Parse<SurfaceKind>(settings.PrimarySurface);
            Adjustments.WeightRatio = settings.AdjustmentWeightRatio;
            Adjustments.Gravel = settings.AdjustmentGravel;
            Adjustments.Tarmac = settings.AdjustmentTarmac;
            Adjustments.Snow = settings.AdjustmentSnow;
            Adjustments.Fwd = settings.AdjustmentFwd;
            Adjustments.Rwd = settings.AdjustmentRwd;
            Adjustments.Awd = settings.AdjustmentAwd;
            // Clear existing templates and load from settings
            ReferenceTemplates.Clear();
            foreach (var carSettings in settings.ReferenceCarSettings)
            {
                var template = new CarTemplateViewModel(this);
                template.ApplySettings(carSettings);
                ReferenceTemplates.Add(template);
            }
            
            // Ensure at least one reference car exists
            if (ReferenceTemplates.Count == 0)
            {
                AddReferenceCar();
            }
            _isReCalcuationEnabled = true;
            ReCalculate();
        }
    }

    internal async Task SaveSettingsAsync()
    {
        var settings = new RootSettings
        {
            IsDescriptionVisible = IsDescriptionVisible,
            TargetCarId = TargetCar?.Id,
            ReferenceCarSettings = ReferenceTemplates.Select(t => t.ToSettings()).ToList(),
            PrimarySurface = Adjustments.PrimarySurface?.ToString() ?? string.Empty,
            AdjustmentWeightRatio = Adjustments.WeightRatio,
            AdjustmentGravel = Adjustments.Gravel,
            AdjustmentTarmac = Adjustments.Tarmac,
            AdjustmentSnow = Adjustments.Snow,
            AdjustmentFwd = Adjustments.Fwd,
            AdjustmentRwd = Adjustments.Rwd,
            AdjustmentAwd = Adjustments.Awd
        };
        await settings.SaveAsync();
    }

    internal async Task ExportCarsAsync(FileInfo exportFile)
        => await _personalData.ExportCarsAsync(exportFile);

    internal async Task ImportCarsAsync(FileInfo importFile)
    {
        await _personalData.ImportCarsAsync(importFile);
        await LoadCarsAsync();
    }

    internal async Task ApplyScalingAsync()
    {
        var selectedCarIds = ReferenceTemplates.Select(t => t.SelectedCarId).ToList();
        _carInfos = await CarInfo.ReadCarsAsync();
        IEnumerable<CarInfo> carInfos = TargetCar == null
            ? _carInfos.Values
            : ([_carInfos[TargetCar.Id]]);
        IReadOnlyDictionary<int, CarFfbSens> ffbSenses = CalculateFfbSenses(carInfos);
        _personalData.ReadFile();
        _personalData.ApplyFfbSens(ffbSenses.Select(kvp => (kvp.Key, kvp.Value.ToPersonal())));
        _personalData.WriteFile();
        await LoadCarsAsync();
        for (int i = 0; i < Math.Min(selectedCarIds.Count, ReferenceTemplates.Count); i++)
        {
            ReferenceTemplates[i].SelectedCarId = selectedCarIds[i];
        }
    }

    internal async Task ClearFfbSensAsync()
    {
        _personalData.ReadFile();
        _personalData.ApplyFfbSens(_personalData.GetCars().Select((i => (i.CarId, new PersonalCarFfbSens()))));
        _personalData.WriteFile();
        await LoadCarsAsync();
    }

    internal void AddReferenceCar()
    {
        ReferenceTemplates.Add(new CarTemplateViewModel(this));
        ReCalculate();
    }

    internal void RemoveReferenceCar(CarTemplateViewModel template)
    {
        if (ReferenceTemplates.Count > 1) // Keep at least one reference car
        {
            ReferenceTemplates.Remove(template);
            ReCalculate();
        }
    }

    internal void ResetToDefaults()
    {
        _isReCalcuationEnabled = false;
        int[]? carWeightsKg = _carInfos?.Values.Select(c => c.WeightKg).Distinct().Order().ToArray();
        IReadOnlyList<int> weightsKg = carWeightsKg == null || carWeightsKg.Length == 0
            ? [500, 1000]
            : carWeightsKg;
        Adjustments.ResetToDefaults();
        
        // Clear existing templates
        ReferenceTemplates.Clear();
        
        // Add two default templates
        var primaryTemplate = new CarTemplateViewModel(this);
        primaryTemplate.ResetToDefaults(LockToLockRotations.FirstOrDefault()?.IntValue ?? 0, weightsKg.First());
        ReferenceTemplates.Add(primaryTemplate);
        
        var secondaryTemplate = new CarTemplateViewModel(this);
        secondaryTemplate.ResetToDefaults(LockToLockRotations.LastOrDefault()?.IntValue ?? 0, weightsKg.Last());
        ReferenceTemplates.Add(secondaryTemplate);
        
        _isReCalcuationEnabled = true;
        ReCalculate();
    }
}
