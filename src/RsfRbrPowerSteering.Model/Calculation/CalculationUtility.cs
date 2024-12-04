namespace RsfRbrPowerSteering.Model.Calculation;

public static class CalculationUtility
{
    private struct FfbSensCalculationParameters
    {
        private readonly struct Parameters
        {
            private readonly List<(decimal value, decimal? ffbSens)> _referencePoints;

            public Parameters(IEnumerable<(decimal? ffbSens, decimal value, decimal drivetrainFactor)> references)
            {
                _referencePoints = references
                    .Where(r => r.ffbSens.HasValue)
                    .Select(r => (r.value, (decimal?)r.ffbSens.Value / r.drivetrainFactor))
                    .OrderBy(p => p.value)
                    .ToList();
            }

            public int? Calculate(decimal value)
            {
                if (_referencePoints.Count == 0)
                    return null;
                if (_referencePoints.Count == 1)
                    return Convert.ToInt32(_referencePoints[0].ffbSens!.Value);

                var lower = _referencePoints.Where(p => p.value <= value).MaxBy(p => p.value);
                var upper = _referencePoints.Where(p => p.value >= value).MinBy(p => p.value);

                if (lower.value == upper.value)
                    return Convert.ToInt32(lower.ffbSens!.Value);

                decimal factor = (value - lower.value) / (upper.value - lower.value);
                decimal interpolated = lower.ffbSens!.Value + (upper.ffbSens!.Value - lower.ffbSens.Value) * factor;
                return Convert.ToInt32(interpolated);
            }
        }

        public FfbSensCalculationParameters(
            IEnumerable<CalculationCar> cars,
            DrivetrainFactors drivetrainFactors)
        {
            var references = cars.Select(car => 
                (car, drivetrainFactor: drivetrainFactors[car.Drivetrain])).ToList();

            ParametersLockToLockRotationGravel = new Parameters(
                references.Select(r => ((decimal?)r.car.FfbSens.Gravel, (decimal)r.car.MaxSteeringLock, r.drivetrainFactor)));
            ParametersLockToLockRotationTarmac = new Parameters(
                references.Select(r => ((decimal?)r.car.FfbSens.Tarmac, (decimal)r.car.MaxSteeringLock, r.drivetrainFactor)));
            ParametersLockToLockRotationSnow = new Parameters(
                references.Select(r => ((decimal?)r.car.FfbSens.Snow, (decimal)r.car.MaxSteeringLock, r.drivetrainFactor)));
            
            ParametersWeightKgGravel = new Parameters(
                references.Select(r => ((decimal?)r.car.FfbSens.Gravel, (decimal)r.car.WeightKg, r.drivetrainFactor)));
            ParametersWeightKgTarmac = new Parameters(
                references.Select(r => ((decimal?)r.car.FfbSens.Tarmac, (decimal)r.car.WeightKg, r.drivetrainFactor)));
            ParametersWeightKgSnow = new Parameters(
                references.Select(r => ((decimal?)r.car.FfbSens.Snow, (decimal)r.car.WeightKg, r.drivetrainFactor)));
        }

        private Parameters ParametersLockToLockRotationGravel { get; }
        private Parameters ParametersLockToLockRotationTarmac { get; }
        private Parameters ParametersLockToLockRotationSnow { get; }

        private Parameters ParametersWeightKgGravel { get; }
        private Parameters ParametersWeightKgTarmac { get; }
        private Parameters ParametersWeightKgSnow { get; }

        public CarFfbSens Calculate(int lockToLockRotation, int weightKg, decimal weightRatio)
        {
            int? calculateForSurface(Parameters parametersLockToLockRotation, Parameters parametersWeightKg)
            {
                if (weightRatio == 0)
                {
                    return parametersLockToLockRotation.Calculate(lockToLockRotation);
                }
                else if (weightRatio == 1)
                {
                    return parametersWeightKg.Calculate(weightKg);
                }
                else
                {
                    decimal? ffbSens = (1 - weightRatio) * parametersLockToLockRotation.Calculate(lockToLockRotation)
                        + weightRatio * parametersWeightKg.Calculate(weightKg);

                    return ffbSens.HasValue
                        ? Convert.ToInt32(Math.Round(ffbSens.Value))
                        : null;
                }
            }

            return new CarFfbSens
            {
                Gravel = calculateForSurface(ParametersLockToLockRotationGravel, ParametersWeightKgGravel),
                Tarmac = calculateForSurface(ParametersLockToLockRotationTarmac, ParametersWeightKgTarmac),
                Snow = calculateForSurface(ParametersLockToLockRotationSnow, ParametersWeightKgSnow)
            };
        }
    };

    public static IReadOnlyDictionary<int, CarFfbSens> CalculateFfbSenses(
        IEnumerable<CarInfo> cars,
        decimal weightRatio,
        DrivetrainFactors drivetrainFactors,
        IEnumerable<CalculationCar> referenceCars)
    {
        if (weightRatio < 0 || weightRatio > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(weightRatio), ModelTexts.WeightRatioRangeError);
        }

        var ffbSensCalcParams = new FfbSensCalculationParameters(
            referenceCars,
            drivetrainFactors);

        return cars
            .GroupBy(c => (c.Drivetrain, c.LockToLockRotation, c.WeightKg))
            .SelectMany(g =>
            {
                decimal drivetrainFactor = drivetrainFactors[g.Key.Drivetrain];
                CarFfbSens ffbSens = ffbSensCalcParams
                    .Calculate(g.Key.LockToLockRotation, g.Key.WeightKg, weightRatio)
                    .Normalize(drivetrainFactor);

                return g.Select(c => (c.Id, FfbSens: ffbSens));
            })
            .ToDictionary(i => i.Id, i => i.FfbSens);
    }
}
