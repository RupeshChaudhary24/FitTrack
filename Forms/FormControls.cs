namespace FitTrack.Forms;

internal static class FormControls
{
    public static NumericUpDown Number()
    {
        return new NumericUpDown
        {
            Minimum = 0.01m,
            Maximum = 1000000m,
            DecimalPlaces = 2,
            Increment = 0.1m,
            ThousandsSeparator = true,
            Dock = DockStyle.Fill
        };
    }

    public static ComboBox Choice(params string[] values)
    {
        var box = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Dock = DockStyle.Fill
        };
        box.Items.AddRange(values);
        box.SelectedIndex = 0;
        return box;
    }

    public static TableLayoutPanel Layout(Form form, string title, int rows)
    {
        form.Text = title;
        form.ClientSize = new Size(520, rows * 45 + 30);
        form.StartPosition = FormStartPosition.CenterParent;
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.MaximizeBox = false;
        form.MinimizeBox = false;
        form.AutoScaleMode = AutoScaleMode.Font;
        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            ColumnCount = 2,
            RowCount = rows
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < rows; i++)
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
        form.Controls.Add(table);
        return table;
    }

    public static void Row(TableLayoutPanel table, int row, string caption, Control control)
    {
        table.Controls.Add(new Label
        {
            Text = caption,
            AutoSize = true,
            Anchor = AnchorStyles.Left
        }, 0, row);
        control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        table.Controls.Add(control, 1, row);
    }

    public static void Buttons(Form form, TableLayoutPanel table, int row, Action save)
    {
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var ok = new Button { Text = "Save", AutoSize = true };
        var cancel = new Button
        {
            Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel
        };
        ok.Click += (_, _) =>
        {
            try
            {
                save();
                form.DialogResult = DialogResult.OK;
                form.Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(form, ex.Message, "Check your input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
        buttons.Controls.AddRange(new Control[] { ok, cancel });
        table.Controls.Add(buttons, 1, row);
        form.AcceptButton = ok;
        form.CancelButton = cancel;
    }

    public static DataGridView Grid()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };
    }
}
