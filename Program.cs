using FitTrack.Forms;

namespace FitTrack;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var singleInstance = new Mutex(true, "FitTrack_Student_Project", out bool first);
        if (!first)
        {
            MessageBox.Show("FitTrack is already running.");
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
