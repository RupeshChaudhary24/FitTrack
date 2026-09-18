using FitTrack.Models;
using FitTrack.Storage;

namespace FitTrack.Forms;

public sealed class MainForm : Form
{
    private readonly GoalRepository repository = new();
    private readonly DataGridView grid = FormControls.Grid();
    private readonly ComboBox filter = FormControls.Choice("All", "Active", "Completed");
    private readonly FlowLayoutPanel actions = new()
    {
        Dock = DockStyle.Top, Height = 55, Padding = new Padding(8), AutoSize = true
    };
    private readonly Label summary = new()
    {
        Dock = DockStyle.Bottom, Height = 65, Padding = new Padding(10)
    };

    public MainForm()
    {
        Text = "FitTrack";
        Size = new Size(1120, 620);
        MinimumSize = new Size(850, 420);
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleMode = AutoScaleMode.Font;
        AddButton("Add Goal", AddGoal);
        AddButton("Log Progress", LogProgress);
        AddButton("View History", ViewHistory);
        AddButton("Edit Goal", EditGoal);
        AddButton("Delete Goal", DeleteGoal);
        actions.Controls.Add(new Label { Text = "Show:", AutoSize = true, Padding = new Padding(5, 7, 0, 0) });
        filter.Width = 120;
        actions.Controls.Add(filter);
        filter.SelectedIndexChanged += (_, _) => RefreshGoals();
        Controls.Add(grid);
        Controls.Add(summary);
        Controls.Add(actions);
        Shown += (_, _) => LoadGoals();
    }

    private void AddButton(string text, Action action)
    {
        var button = new Button { Text = text, AutoSize = true };
        button.Click += (_, _) =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "The action could not be completed.\n" + ex.Message,
                    "FitTrack", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
        actions.Controls.Add(button);
    }

    private void LoadGoals()
    {
        try
        {
            repository.Load();
            RefreshGoals();
        }
        catch (Exception ex)
        {
            actions.Enabled = false;
            summary.Text = "Data could not be loaded. File: " + repository.FilePath;
            MessageBox.Show(this,
                "FitTrack could not load your saved data. Changes are disabled to protect the file.\n\n" +
                ex.Message + "\n\nFile: " + repository.FilePath +
                "\nClose FitTrack, back up this file, correct the problem, then reopen the app.",
                "Could not load goals", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private FitnessGoal SelectedGoal()
    {
        if (grid.CurrentRow == null)
            throw new ArgumentException("Select a goal first.");
        Guid id = (Guid)grid.CurrentRow.Cells["GoalId"].Value;
        return repository.Goals.First(g => g.GoalId == id);
    }

    private void RefreshGoals(Guid? selectedId = null)
    {
        grid.DataSource = repository.Goals
            .Where(g => filter.Text == "All" || g.Status == filter.Text)
            .Select(g => new
            {
                g.GoalId,
                g.Name,
                Type = g.Kind,
                g.Unit,
                Start = g.StartValue,
                Current = g.CurrentValue,
                Target = g.TargetValue,
                Due = g.TargetDate.ToString("dd/MM/yyyy"),
                Progress = g.Progress.ToString("0.##") + "%",
                g.Status
            }).ToList();
        if (grid.Columns.Contains("GoalId"))
            grid.Columns["GoalId"].Visible = false;
        if (selectedId.HasValue)
        {
            foreach (DataGridViewRow row in grid.Rows)
                if ((Guid)row.Cells["GoalId"].Value == selectedId.Value)
                    grid.CurrentCell = row.Cells["Name"];
        }
        summary.Text = $"Showing {grid.Rows.Count} of {repository.Goals.Count} goals. Changes save automatically.\n" +
            "Data file: " + repository.FilePath;
    }

    private void AddGoal()
    {
        using var form = new GoalForm();
        if (form.ShowDialog(this) != DialogResult.OK || form.Result == null)
            return;
        repository.SaveGoal(form.Result);
        RefreshGoals(form.Result.GoalId);
    }

    private void EditGoal()
    {
        FitnessGoal copy = repository.Copy(SelectedGoal());
        using var form = new GoalForm(copy);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;
        repository.SaveGoal(copy);
        RefreshGoals(copy.GoalId);
    }

    private void LogProgress()
    {
        FitnessGoal copy = repository.Copy(SelectedGoal());
        using var form = new LogProgressForm(copy);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;
        repository.SaveGoal(copy);
        RefreshGoals(copy.GoalId);
    }

    private void ViewHistory()
    {
        using var form = new HistoryForm(SelectedGoal());
        form.ShowDialog(this);
    }

    private void DeleteGoal()
    {
        FitnessGoal goal = SelectedGoal();
        if (MessageBox.Show(this, $"Delete '{goal.Name}' and all its progress entries?",
            "Delete Goal", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        repository.DeleteGoal(goal.GoalId);
        RefreshGoals();
    }
}
