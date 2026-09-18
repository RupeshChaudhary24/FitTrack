namespace FitTrack.Models;

public sealed class WeightLossGoal : FitnessGoal
{
    public override string Kind => "Weight Loss";
    public override decimal CalculateProgress() => DecreasingProgress();

    protected override void ValidateType()
    {
        if (Unit != "kg" || TargetValue >= StartValue)
            throw new ArgumentException("Weight loss uses kg and requires a lower target value.");
    }
}
