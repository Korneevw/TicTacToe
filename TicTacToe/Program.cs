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
            if (FileManager.CheckFilesExistance() == false) // If files don't exist
            {
                MessageBox.Show("Required translation or wins files are not found, try reinstalling the app, or look up Instalation in README.md on GitHub.", "Files not found");
                return; // Cancel starting the app
            }
            Application.Run(new MainForm());
        }
    }
}