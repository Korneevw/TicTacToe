using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    internal static class TranslationManager
    {
        public enum Language
        {
            English,
            Russian
        }
        /// <summary>Acts as key for Translation <see cref="Dictionary{TKey, TValue}"/>.</summary>
        public enum TranslatableText
        {
            FieldSizeLabel,
            WinsLabelDefault,
            WinsLabelCrossWins,
            WinsLabelCircleWins,
            CurrentTurnLabelDefault,
            Cross,
            Circle,
            CurrentTurnLabelPreValue,
            CurrentTurnLabelAfterValue,
            RestartButton,
            ResetWinsButton,
            MainFormName,
        }
        public static Dictionary<TranslatableText, string> Translation { get; private set; } = new Dictionary<TranslatableText, string>();
        /// <summary>Checks selected translation from LanguageSelector <see cref="ListBox"/>.</summary>
        public static void GetTranslation()
        {
            switch (MainForm.LanguageSelector.SelectedItem)
            {
                case "English":
                    Translation = FileManager.GetTranslationFromFile(Language.English);
                    break;
                case "Русский":
                    Translation = FileManager.GetTranslationFromFile(Language.Russian);
                    break;
            }
        }
        /// <summary>Updates all translatable text according to the translation.</summary>
        public static void UpdateAllText()
        {
            MainForm.FieldSizeLabel.Text = Translation[TranslatableText.FieldSizeLabel];
            MainForm.WinsLabel.Text =
                Translation[TranslatableText.WinsLabelCrossWins] + MainForm.CrossWins + '\n'
                + Translation[TranslatableText.WinsLabelCircleWins] + MainForm.CircleWins;
            MainForm.CurrentTurnLabel.Text =
                Translation[TranslatableText.CurrentTurnLabelPreValue]
                + (MainForm.Turn % 2 == 0 ? Translation[TranslatableText.Cross] : Translation[TranslatableText.Circle])
                + Translation[TranslatableText.CurrentTurnLabelAfterValue];
            MainForm.RestartButton.Text = Translation[TranslatableText.RestartButton];
            MainForm.ResetWinsButton.Text = Translation[TranslatableText.ResetWinsButton];
            MainForm.Text = Translation[TranslatableText.MainFormName];
        }
        /// <summary>Updates only the WinsLabel's text according to the translation.</summary>
        public static void UpdateWinsLabel()
        {
            MainForm.WinsLabel.Text =
                Translation[TranslatableText.WinsLabelCrossWins] + MainForm.CrossWins + '\n'
                + Translation[TranslatableText.WinsLabelCircleWins] + MainForm.CircleWins;
        }
        /// <summary>Updates only the CurrentTurnLabel's text accroding to the translation</summary>
        public static void UpdateCurrentTurnLabel()
        {
            MainForm.CurrentTurnLabel.Text =
                Translation[TranslatableText.CurrentTurnLabelPreValue]
                + (MainForm.Turn % 2 == 0 ? Translation[TranslatableText.Cross] : Translation[TranslatableText.Circle])
                + Translation[TranslatableText.CurrentTurnLabelAfterValue];
        }
    }
}
