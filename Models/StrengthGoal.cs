namespace FitTrack.Models;

public sealed class StrengthGoal : FitnessGoal
{
    public override string Kind => "Strength";
    public override decimal CalculateProgress() => IncreasingProgress();

    protected override void ValidateType()
    {
        if (Unit != "kg" || TargetValue <= StartValue)
            throw new ArgumentException("Strength uses kg and requires a higher target value.");
    }
}
