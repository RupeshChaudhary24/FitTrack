using FitTrack.Models;

namespace FitTrack.Forms;

public sealed class GoalForm : Form
{
    private readonly ComboBox type = FormControls.Choice(
        "Weight Loss", "Fat Loss", "Strength", "Endurance");
    private readonly TextBox name = new() { MaxLength = 100, Dock = DockStyle.Fill };
    private readonly ComboBox unit = FormControls.Choice("kg");
    private readonly NumericUpDown start = FormControls.Number();
    private readonly NumericUpDown target = FormControls.Number();
    private readonly DateTimePicker date = new() { Format = DateTimePickerFormat.Short };
    private readonly FitnessGoal? existing;

    public FitnessGoal? Result { get; private set; }

    public GoalForm(FitnessGoal? editableCopy = null)
    {
        existing = editableCopy;
        var layout = FormControls.Layout(this,
            existing == null ? "Add Goal" : "Edit Goal", 8);
        FormControls.Row(layout, 0, "Goal type", type);
        FormControls.Row(layout, 1, "Goal name", name);
        FormControls.Row(layout, 2, "Unit", unit);
        FormControls.Row(layout, 3, "Start value", start);
        FormControls.Row(layout, 4, "Target value", target);
        FormControls.Row(layout, 5, "Target date", date);
        FormControls.Row(layout, 6, "Time goals", new Label
        {
            Text = "Use the same running distance each time.", AutoSize = true
        });
        FormControls.Buttons(this, layout, 7, Save);
        date.MinDate = existing?.StartDate.Date ?? DateTime.Today;
        date.Value = DateTime.Today.AddMonths(1);
        type.SelectedIndexChanged += (_, _) => SetUnits();

        if (existing != null)
        {
            type.SelectedItem = existing.Kind;
            SetUnits();
            unit.SelectedItem = existing.Unit;
            name.Text = existing.Name;
            start.Value = existing.StartValue;
            target.Value = existing.TargetValue;
            date.Value = existing.TargetDate;
            type.Enabled = false;
            unit.Enabled = false;
            start.Enabled = false;
        }
    }

    private void SetUnits()
    {
        unit.Items.Clear();
        if (type.Text == "Endurance")
            unit.Items.AddRange(new object[] { "km", "minutes" });
        else
            unit.Items.Add(type.Text == "Fat Loss" ? "%" : "kg");
        unit.SelectedIndex = 0;
    }

    private void Save()
    {
        FitnessGoal goal;
        if (existing != null)
            goal = existing;
        else
            goal = type.Text switch
            {
                "Weight Loss" => new WeightLossGoal(),
                "Fat Loss" => new FatLossGoal(),
                "Strength" => new StrengthGoal(),
                "Endurance" => new EnduranceGoal(),
                _ => throw new ArgumentException("Select a goal type.")
            };
        goal.Configure(name.Text, unit.Text, start.Value, target.Value, date.Value);
        Result = goal;
    }
}
