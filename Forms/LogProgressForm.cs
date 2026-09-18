using FitTrack.Models;

namespace FitTrack.Forms;

public sealed class LogProgressForm : Form
{
    public LogProgressForm(FitnessGoal editableCopy)
    {
        var date = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            MinDate = editableCopy.StartDate.Date,
            MaxDate = DateTime.Today,
            Value = DateTime.Today
        };
        var value = FormControls.Number();
        value.Value = editableCopy.CurrentValue;
        var layout = FormControls.Layout(this, "Log Progress", 4);
        FormControls.Row(layout, 0, "Goal", new Label
        {
            Text = editableCopy.Name, AutoSize = true
        });
        FormControls.Row(layout, 1, "Entry date", date);
        FormControls.Row(layout, 2, "Value (" + editableCopy.Unit + ")", value);
        FormControls.Buttons(this, layout, 3, () =>
            editableCopy.LogProgress(date.Value, value.Value));
    }
}
