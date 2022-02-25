using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    internal static class FileManager
    {
        public static string DataFolderPath { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        public static string WinsFilePath { get; private set; } = Path.Combine(DataFolderPath, "wins.txt");
        public static string TranslationsFolderPath { get; private set; } = Path.Combine(DataFolderPath, "Translations");
        public static Dictionary<TranslationManager.TranslatableText, string> GetTranslationFromFile(TranslationManager.Language language)
        {
            return File
                    .ReadAllLines(Path.Combine(TranslationsFolderPath, $"{language}.txt")) // Read lines of type: Key|Value
                    .Select(l => l.Split('|')) // Split lines into { Key, Value }
                    .ToDictionary(
                        splitL => (TranslationManager.TranslatableText)Enum.Parse(typeof(TranslationManager.TranslatableText), splitL[0]), // Key
                        splitL => splitL[1]); // Value
        }
        public static void ReadWinsFile()
        {
            string[] winsData = File.ReadAllLines(WinsFilePath);
            if (winsData.Length == 3) // If either circle, cross and draw wins values exist
            {
                MainForm.CrossWins = int.Parse(winsData[0]);
                MainForm.CircleWins = int.Parse(winsData[1]);
                MainForm.Draws = int.Parse(winsData[2]);
            }
        }
        public static void WriteWinsFile()
        {
            string[] data = new string[] { MainForm.CrossWins.ToString(), MainForm.CircleWins.ToString(), MainForm.Draws.ToString() };
            File.WriteAllLines(WinsFilePath, data);
        }
        public static void ResetWinsFile()
        {
            File.WriteAllLines(WinsFilePath, new string[] { "0", "0", "0" }); // Set each value to 0
            ReadWinsFile();
            TranslationManager.UpdateWinsLabelText();
        }
        /// <summary>Checks if all required files exist.</summary>
        /// <returns> <see cref="bool"/> describing whether all the files exist.</returns>
        public static bool CheckFilesExistance()
        {
            return Directory.Exists(DataFolderPath) // Data folder exists
                && Directory.Exists(TranslationsFolderPath)  // And Translations folder exists
                && File.Exists(WinsFilePath) // And wins.txt file exists
                && Directory.GetFiles(TranslationsFolderPath).Length == Enum.GetNames(typeof(TranslationManager.Language)).Length; // And all translations exist:
        }
    }
}
