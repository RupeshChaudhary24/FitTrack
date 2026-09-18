using FitTrack.Models;

namespace FitTrack.Forms;

public sealed class HistoryForm : Form
{
    public HistoryForm(FitnessGoal goal)
    {
        Text = "Progress History - " + goal.Name;
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(660, 430);
        MinimumSize = new Size(500, 300);
        var grid = FormControls.Grid();
        grid.DataSource = goal.Entries.OrderBy(e => e.EntryDate).Select(e => new
        {
            Date = e.EntryDate.ToString("dd/MM/yyyy"),
            Value = e.LoggedValue,
            goal.Unit
        }).ToList();
        var summary = new Label
        {
            Dock = DockStyle.Top,
            Height = 65,
            Padding = new Padding(10),
            Text = $"Start: {goal.StartValue} {goal.Unit}   Target: {goal.TargetValue} {goal.Unit}\n" +
                $"Progress: {goal.Progress:0.##}%   Status: {goal.Status}" +
                (goal.Entries.Count == 0 ? "   No entries recorded yet." : "")
        };
        Controls.Add(grid);
        Controls.Add(summary);
    }
}
