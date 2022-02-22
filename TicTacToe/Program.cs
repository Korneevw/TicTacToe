namespace TicTacToe
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            if (!Directory.Exists(MainForm.DataFolderPath) || !Directory.Exists(MainForm.TranslationFolderPath) || !File.Exists(MainForm.WinsFilePath)
                || Directory.GetFiles(MainForm.TranslationFolderPath).Length < Enum.GetNames(typeof(MainForm.Language)).Length)
            {
                MessageBox.Show("Required translation or wins files are not found, try reinstalling the app.", "Files not found");
                return;
            }
            Application.Run(new MainForm());
        }
    }
}