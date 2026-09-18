namespace FitTrack.Models;

public sealed class EnduranceGoal : FitnessGoal
{
    public override string Kind => "Endurance";
    public override decimal CalculateProgress() =>
        Unit == "minutes" ? DecreasingProgress() : IncreasingProgress();

    protected override void ValidateType()
    {
        if (Unit != "km" && Unit != "minutes")
            throw new ArgumentException("Choose km or minutes for endurance.");
        if (Unit == "km" && TargetValue <= StartValue)
            throw new ArgumentException("A distance goal requires a higher target.");
        if (Unit == "minutes" && TargetValue >= StartValue)
            throw new ArgumentException("A running-time goal requires a lower target.");
    }
}
