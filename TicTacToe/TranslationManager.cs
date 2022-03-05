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
            WinsLabelCrossWins,
            WinsLabelCircleWins,
            WinsLabelDraws,
            CurrentTurnLabelDefault,
            Cross,
            Circle,
            CurrentTurnLabelPreValue,
            CurrentTurnLabelAfterValue,
            StartEndButtonStart,
            StartEndButtonEnd,
            ResetWinsButton,
            ImportWinsButton,
            ExportWinsButton,
            ReadWinsFailedMessage,
            ReadWinsFailedHeading,
            ImportSuccesfulMessage,
            ImportSuccesfulHeading,
            ExportSuccesfulMessage,
            ExportSuccesfulHeading,
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
            UpdateWinsLabelText();
            UpdateCurrentTurnLabelText();
            UpdateStartEndButtonText();
            MainForm.ResetWinsButton.Text = Translation[TranslatableText.ResetWinsButton];
            MainForm.ImportWinsButton.Text = Translation[TranslatableText.ImportWinsButton];
            MainForm.ExportWinsButton.Text = Translation[TranslatableText.ExportWinsButton];
        }
        /// <summary>Updates only the WinsLabel's text according to the translation.</summary>
        public static void UpdateWinsLabelText()
        {
            MainForm.WinsLabel.Text =
                Translation[TranslatableText.WinsLabelCrossWins] + MainForm.CrossWins + '\n'
                + Translation[TranslatableText.WinsLabelCircleWins] + MainForm.CircleWins + '\n'
                + Translation[TranslatableText.WinsLabelDraws] + MainForm.Draws;
        }
        /// <summary>Updates only the CurrentTurnLabel's text accroding to the translation and playing state.</summary>
        public static void UpdateCurrentTurnLabelText()
        {
            if (!MainForm.IsPlaying)
            {
                MainForm.CurrentTurnLabel.Text = Translation[TranslatableText.CurrentTurnLabelDefault];
            }
            else if (MainForm.IsPlaying)
            {
                MainForm.CurrentTurnLabel.Text =
                     Translation[TranslatableText.CurrentTurnLabelPreValue]
                     + (MainForm.Turn % 2 == 0 ? Translation[TranslatableText.Cross] : Translation[TranslatableText.Circle])
                     + Translation[TranslatableText.CurrentTurnLabelAfterValue];
            }
        }
        /// <summary>If playing, sets end, if not playing, sets start, according to the translation.</summary>
        public static void UpdateStartEndButtonText()
        {
            if (MainForm.IsPlaying)
            {
                MainForm.StartEndButton.Text = Translation[TranslatableText.StartEndButtonEnd];
            }
            else if (!MainForm.IsPlaying)
            {
                MainForm.StartEndButton.Text = Translation[TranslatableText.StartEndButtonStart];
            }
        }
    }
}
