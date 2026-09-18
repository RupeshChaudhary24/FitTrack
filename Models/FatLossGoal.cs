namespace FitTrack.Models;

public sealed class FatLossGoal : FitnessGoal
{
    public override string Kind => "Fat Loss";
    public override decimal CalculateProgress() => DecreasingProgress();

    protected override void ValidateType()
    {
        if (Unit != "%" || StartValue >= 100m || TargetValue >= StartValue)
            throw new ArgumentException("Fat loss uses body fat %, below 100, with a lower target.");
    }
}
