using System.Text.Json;
using FitTrack.Models;

namespace FitTrack.Storage;

public sealed class GoalRepository
{
    private readonly JsonSerializerOptions options = new() { WriteIndented = true };
    private List<FitnessGoal> goals = new();
    private bool loaded;

    public string FilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FitTrack", "goals.json");

    public IReadOnlyList<FitnessGoal> Goals => goals.AsReadOnly();

    public void Load()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        List<FitnessGoal> data;
        try
        {
            string json = File.ReadAllText(FilePath);
            data = JsonSerializer.Deserialize<List<FitnessGoal>>(json, options)
                ?? throw new InvalidDataException("The data file contains no goal list.");
        }
        catch (FileNotFoundException)
        {
            data = new List<FitnessGoal>();
        }

        ValidateAll(data);
        goals = data;
        loaded = true;
    }

    public FitnessGoal Copy(FitnessGoal goal)
    {
        string json = JsonSerializer.Serialize<FitnessGoal>(goal, options);
        return JsonSerializer.Deserialize<FitnessGoal>(json, options)
            ?? throw new InvalidDataException("Could not copy the goal.");
    }

    public void SaveGoal(FitnessGoal goal)
    {
        var next = new List<FitnessGoal>(goals);
        int index = next.FindIndex(g => g.GoalId == goal.GoalId);
        if (index < 0)
            next.Add(goal);
        else
            next[index] = goal;
        Save(next);
    }

    public void DeleteGoal(Guid id)
    {
        Save(goals.Where(g => g.GoalId != id).ToList());
    }

    private void Save(List<FitnessGoal> next)
    {
        if (!loaded)
            throw new InvalidOperationException("Load the data successfully before making changes.");
        ValidateAll(next);
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        string temporary = FilePath + ".tmp";
        File.WriteAllText(temporary, JsonSerializer.Serialize(next, options));
        File.Move(temporary, FilePath, true);
        goals = next;
    }

    private static void ValidateAll(List<FitnessGoal> data)
    {
        if (data.Any(g => g == null))
            throw new InvalidDataException("The file contains an empty goal.");
        foreach (FitnessGoal goal in data)
            goal.Validate();
        if (data.Select(g => g.GoalId).Distinct().Count() != data.Count)
            throw new InvalidDataException("The file contains duplicate goal IDs.");
    }
}
