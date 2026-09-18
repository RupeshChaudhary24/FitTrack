using System.Text.Json.Serialization;

namespace FitTrack.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "GoalType")]
[JsonDerivedType(typeof(WeightLossGoal), "WeightLoss")]
[JsonDerivedType(typeof(FatLossGoal), "FatLoss")]
[JsonDerivedType(typeof(StrengthGoal), "Strength")]
[JsonDerivedType(typeof(EnduranceGoal), "Endurance")]
public abstract class FitnessGoal
{
    [JsonInclude]
    public Guid GoalId { get; private set; } = Guid.NewGuid();
    [JsonInclude]
    public string Name { get; private set; } = "";
    [JsonInclude]
    public string Unit { get; private set; } = "";
    [JsonInclude]
    public DateTime StartDate { get; private set; } = DateTime.Today;
    [JsonInclude]
    public DateTime TargetDate { get; private set; }
    [JsonInclude]
    public decimal StartValue { get; private set; }
    [JsonInclude]
    public decimal TargetValue { get; private set; }
    [JsonInclude]
    public IReadOnlyList<ProgressEntry> Entries { get; private set; }
        = Array.Empty<ProgressEntry>();

    [JsonIgnore]
    public abstract string Kind { get; }
    [JsonIgnore]
    public decimal CurrentValue => Entries.OrderBy(e => e.EntryDate)
        .LastOrDefault()?.LoggedValue ?? StartValue;
    [JsonIgnore]
    public decimal Progress => Math.Round(CalculateProgress(), 2);
    public string Status => CalculateProgress() >= 100m ? "Completed" : "Active";

    public abstract decimal CalculateProgress();
    protected abstract void ValidateType();

    public void Configure(string name, string unit, decimal start,
        decimal target, DateTime targetDate)
    {
        Name = name.Trim();
        Unit = unit;
        StartValue = start;
        TargetValue = target;
        TargetDate = targetDate.Date;
        Validate();
    }

    public void LogProgress(DateTime date, decimal value)
    {
        ValidateEntry(date, value);
        if (Entries.Any(e => e.EntryDate.Date == date.Date))
            throw new ArgumentException("An entry already exists for this date. Choose another date.");

        Entries = Entries.Append(new ProgressEntry
        {
            EntryDate = date.Date,
            LoggedValue = value
        }).OrderBy(e => e.EntryDate).ToArray();
    }

    public void Validate()
    {
        if (GoalId == Guid.Empty || string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Enter a goal name.");
        if (Name.Length > 100)
            throw new ArgumentException("Use a goal name of at most 100 characters.");
        if (StartDate.Date < new DateTime(1900, 1, 1) || StartDate.Date > DateTime.Today)
            throw new ArgumentException("The goal start date is invalid.");
        if (TargetDate.Date < StartDate.Date || TargetDate.Year > 9998)
            throw new ArgumentException("The target date must be on or after the start date.");
        if (StartValue <= 0 || TargetValue <= 0 || StartValue > 1000000m || TargetValue > 1000000m)
            throw new ArgumentException("Values must be greater than zero and no more than 1,000,000.");
        if (StartValue == TargetValue)
            throw new ArgumentException("Start and target values must be different.");

        ValidateType();
        if (Entries == null || Entries.Any(e => e == null))
            throw new ArgumentException("The saved progress history is invalid.");
        foreach (var entry in Entries)
            ValidateEntry(entry.EntryDate, entry.LoggedValue);
        if (Entries.Select(e => e.EntryDate.Date).Distinct().Count() != Entries.Count)
            throw new ArgumentException("The saved history contains duplicate dates.");
    }

    private void ValidateEntry(DateTime date, decimal value)
    {
        if (date.Date < StartDate.Date || date.Date > DateTime.Today)
            throw new ArgumentException("Choose an entry date between the goal start date and today.");
        if (value <= 0 || value > 1000000m)
            throw new ArgumentException("Enter a value greater than zero and no more than 1,000,000.");
        if (this is FatLossGoal && value >= 100m)
            throw new ArgumentException("Body fat percentage must be below 100.");
    }

    protected decimal IncreasingProgress()
    {
        return Math.Clamp((CurrentValue - StartValue) /
            (TargetValue - StartValue) * 100m, 0m, 100m);
    }

    protected decimal DecreasingProgress()
    {
        return Math.Clamp((StartValue - CurrentValue) /
            (StartValue - TargetValue) * 100m, 0m, 100m);
    }
}
