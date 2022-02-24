#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace TicTacToe
{
    /// <summary>The main game window.</summary>
    public partial class MainForm : Form
    {
        private const int Indent = 4;
        public static Label WinsLabel { get; set; }
        public static Label CurrentTurnLabel { get; set; }
        public static Label FieldSizeLabel { get; set; }
        public static Button RestartButton { get; set; }
        public static Button ResetWinsButton { get; set; }
        public static ListBox LanguageSelector { get; set; }
        new public static string Text { get; set; }
        public static int CrossWins { get; set; }
        public static int CircleWins { get; set; }
        public static int Turn { get; private set; } = 0;
        private bool IsPlaying = true;
        private int _fieldWidth = 3;
        private int _fieldHeight = 3;
        private Size _createdFieldSize;
        private NumericUpDown _fieldWidthInput;
        private NumericUpDown _fieldHeightInput;
        private TTTButton _tttButtonPattern;
        private TTTButton[,] _buttons = new TTTButton[0, 0];
        private List<List<TTTButton>> _winCombinations = new List<List<TTTButton>>();
        public MainForm()
        {
            LanguageSelector = new ListBox()
            {
                Location = new Point(Indent, Indent),
            };
            LanguageSelector.Items.Add("English");
            LanguageSelector.Items.Add("Русский");
            LanguageSelector.SelectedItem = LanguageSelector.Items[0];
            LanguageSelector.Height = LanguageSelector.Items.Count * LanguageSelector.Font.Height + Indent;
            LanguageSelector.SelectedValueChanged += LanguageSelectorSelectedValueChanged;
            Controls.Add(LanguageSelector);
            TranslationManager.GetTranslation();
            CreateControls();
            TranslationManager.UpdateAllText();
            SetFormSettings();
            DoStartupActions();
        }
        private void LanguageSelectorSelectedValueChanged(object? sender, EventArgs e)
        {
            TranslationManager.GetTranslation();
            TranslationManager.UpdateAllText();
        }
        private void CreateControls()
        {
            FieldSizeLabel = new Label() // Labels the field size NumericUpDowns.
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.FieldSizeLabel],
                Height = Font.Height + 2,
                Location = new Point(LanguageSelector.Left, LanguageSelector.Bottom - 3),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
            };
            _fieldWidthInput = new NumericUpDown()
            {
                Maximum = 12,
                Minimum = 2,
                Value = 3,
                Location = new Point(FieldSizeLabel.Left, FieldSizeLabel.Bottom - 1),
            };
            _fieldWidthInput.ValueChanged += FieldWidthChanged;
            _fieldHeightInput = new NumericUpDown()
            {
                Maximum = 12,
                Minimum = 2,
                Value = 3,
                Location = new Point(_fieldWidthInput.Right, _fieldWidthInput.Top),
            };
            _fieldHeightInput.ValueChanged += FieldHeightChanged;
            _tttButtonPattern = new TTTButton()
            {
                Size = new Size(50, 50),
                Font = new Font("Arial", 10, FontStyle.Bold),
                BackColor = Color.White,
            };
            WinsLabel = new Label() // Displays the wins.
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.WinsLabelDefault],
                Height = Font.Height * 2 + 2,
                Location = new Point(FieldSizeLabel.Left, _fieldWidthInput.Bottom - 1),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
            };
            CurrentTurnLabel = new Label() // Displays the current turn.
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.CurrentTurnLabelDefault],
                Height = Font.Height + 2,
                Location = new Point(WinsLabel.Left, WinsLabel.Bottom - 1),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
            };
            CreateField(new Point(Indent, CurrentTurnLabel.Bottom), _tttButtonPattern, out _createdFieldSize);
            UpdateControlSizes();
            RestartButton = new Button() // Restarts the game (clears the field).
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.RestartButton],
                Location = new Point(_buttons[0, _buttons.GetUpperBound(1)].Left, _buttons[0, _buttons.GetUpperBound(1)].Bottom),
                Width = _createdFieldSize.Width,
                Height = _buttons[0, 0].Height,
                BackColor = Color.White,
            };
            RestartButton.Click += RestartButtonClick;
            ResetWinsButton= new Button() // Resets the wins file.
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.RestartButton],
                Location = new Point(RestartButton.Left, RestartButton.Bottom),
                Width = RestartButton.Width,
                Height = _buttons[0, 0].Height / 2,
                BackColor = Color.White,
            };
            ResetWinsButton.Click += ResetWinsButtonClick;
            Controls.AddRange(new Control[] { FieldSizeLabel, _fieldWidthInput, _fieldHeightInput, WinsLabel, CurrentTurnLabel, RestartButton, ResetWinsButton });
            InitializeComponent();
        }
        /// <summary>Creates TTTButtons for the field.</summary>
        /// <param name="startingPoint">Where to create the field.</param>
        /// <param name="pattern">Pattern for each field cell (TTTButton).</param>
        /// <param name="resultSize">Size of the created field.</param>
        private void CreateField(Point startingPoint, TTTButton pattern, out Size resultSize)
        {
            if (_buttons is not null)
            {
                foreach (TTTButton b in _buttons)
                {
                    Controls.Remove(b);
                }
            }
            _buttons = new TTTButton[_fieldWidth, _fieldHeight];
            resultSize = new Size(0, 0);
            for (int i = 0; i <= _buttons.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _buttons.GetUpperBound(1); j++)
                {
                    _buttons[i, j] = new TTTButton()
                    {
                        Size = pattern.Size,
                        Font = pattern.Font,
                        BackColor = pattern.BackColor,
                        Symbol = TTTButton.Symbols.None,
                        Location = new Point(startingPoint.X + pattern.Width * i, startingPoint.Y + pattern.Height * j),
                    };
                    _buttons[i, j].Click += TTTButtonClick;
                    Controls.Add(_buttons[i, j]);
                    resultSize = new Size(pattern.Width * (i + 1), pattern.Height * (i + 1));
                }
            }
        }
        private void CreateWinningCombinations()
        {
            if (_winCombinations.Any()) _winCombinations.Clear();
            for (int i = 0; i <= _buttons.GetUpperBound(1); i++) // Horizontal.
            {
                List<TTTButton> horizontal = new List<TTTButton>();
                for (int j = 0; j <= _buttons.GetUpperBound(0); j++)
                {
                    horizontal.Add(_buttons[j, i]);
                }
                _winCombinations.Add(horizontal);
            }
            for (int i = 0; i <= _buttons.GetUpperBound(0); i++) // Vertical.
            {
                List<TTTButton> vertical = new List<TTTButton>();
                for (int j = 0; j <= _buttons.GetUpperBound(1); j++)
                {
                    vertical.Add(_buttons[i, j]);
                }
                _winCombinations.Add(vertical);
            }
            if (_buttons.GetLength(0) == _buttons.GetLength(1)) // If field is a square:
            {
                List<TTTButton> leftDiagonal = new List<TTTButton>();
                for (int i = 0; i < Math.Sqrt(_buttons.Length); i++) // Left diagonal.
                {
                    leftDiagonal.Add(_buttons[i, i]);
                }
                _winCombinations.Add(leftDiagonal);
                List<TTTButton> rightDiagonal = new List<TTTButton>();
                for (int i = 0; i < Math.Sqrt(_buttons.Length); i++) // Right diagonal.
                {
                    rightDiagonal.Add(_buttons[i, (int)Math.Sqrt(_buttons.Length) - 1 - i]);
                }
                _winCombinations.Add(rightDiagonal);
            }
        }
        private void TTTButtonClick(object? sender, EventArgs e)
        {
            if (!IsPlaying)
            {
                return;
            }
            if (sender is TTTButton castedSender)
            {
                if (castedSender.Symbol != TTTButton.Symbols.None) return;
                castedSender.Symbol = Turn % 2 == 0 ? TTTButton.Symbols.X : TTTButton.Symbols.O;
                Turn++;
                CheckWin();
                TranslationManager.UpdateCurrentTurnLabel();
            }
        }
        private void SetFormSettings()
        {
            Text = TranslationManager.Translation[TranslationManager.TranslatableText.MainFormName];
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
        private void DoStartupActions()
        {
            CreateWinningCombinations();
            UpdateFieldSizeInputEnabled();
            UpdateRestartButtonEnabled();
            UpdateResetWinsButtonEnabled();
            FileManager.ReadWinsFile();
            TranslationManager.UpdateWinsLabel();
            TranslationManager.UpdateCurrentTurnLabel();
        }
        private void CheckWin()
        {
            foreach (List<TTTButton> combination in _winCombinations)
            {
                if (combination.All(b => b.Symbol == TTTButton.Symbols.X)) // If combination is filled with X:
                {
                    CrossWins++;
                    EndGame(combination);
                    break;
                }
                else if (combination.All(b => b.Symbol == TTTButton.Symbols.O)) // If combination is filled with O:
                {
                    CircleWins++;
                    EndGame(combination);
                    break;
                }
            }
            if (_winCombinations.All(c => c.All(b => b.Symbol != TTTButton.Symbols.None))) // If the whole field is filled:
            {
                EndGame();
            }
        }
        private void StartGame()
        {
            CreateField(new Point(Indent, CurrentTurnLabel.Bottom), _tttButtonPattern, out _createdFieldSize);
            CreateWinningCombinations();
            UpdateRestartButtonPosSize();
            UpdateResetWinsButtonPosSize();
            UpdateFieldSizeInputEnabled();
            UpdateRestartButtonEnabled();
            UpdateResetWinsButtonEnabled();
            UpdateControlSizes();
            TranslationManager.UpdateWinsLabel();
            TranslationManager.UpdateCurrentTurnLabel();
        }
        private void EndGame()
        {
            IsPlaying = false;
            UpdateRestartButtonEnabled();
            UpdateResetWinsButtonEnabled();
            UpdateFieldSizeInputEnabled();
            TranslationManager.UpdateWinsLabel();
            FileManager.WriteWinsFile(CrossWins, CircleWins);
        }
        /// <summary>Ends the game, and highlights the <paramref name="winningCombination"/>.</summary>
        private void EndGame(List<TTTButton> winningCombination)
        {
            winningCombination.ForEach(b => b.ForeColor = Color.Red);
            IsPlaying = false;
            UpdateRestartButtonEnabled();
            UpdateResetWinsButtonEnabled();
            UpdateFieldSizeInputEnabled();
            TranslationManager.UpdateWinsLabel();
            FileManager.WriteWinsFile(CrossWins, CircleWins);
        }
        private void UpdateFieldSizeInputEnabled()
        {
            if (IsPlaying)
            {
                _fieldWidthInput.Enabled = false;
                _fieldHeightInput.Enabled = false;
            }
            else
            {
                _fieldWidthInput.Enabled = true;
                _fieldHeightInput.Enabled = true;
            }
        }
        private void UpdateRestartButtonEnabled()
        {
            if (IsPlaying)
            {
                RestartButton.Enabled = false;
            }
            else
            {
                RestartButton.Enabled = true;
            }
        }
        private void UpdateResetWinsButtonEnabled()
        {
            if (IsPlaying)
            {
                ResetWinsButton.Enabled = false;
            }
            else
            {
                ResetWinsButton.Enabled = true;
            }
        }
        /// <summary>Scales all controls to created field size.</summary>
        private void UpdateControlSizes()
        {
            LanguageSelector.Width = _createdFieldSize.Width;
            FieldSizeLabel.Width = LanguageSelector.Width;
            _fieldWidthInput.Width = FieldSizeLabel.Width / 2;
            _fieldHeightInput.Width = FieldSizeLabel.Width / 2;
            _fieldHeightInput.Location = new Point(_fieldWidthInput.Right, _fieldWidthInput.Top);
            WinsLabel.Width = FieldSizeLabel.Width;
            CurrentTurnLabel.Width = WinsLabel.Width;
        }
        private void UpdateRestartButtonPosSize()
        {
            RestartButton.Location = new Point(_buttons[0, _buttons.GetUpperBound(1)].Left, _buttons[0, _buttons.GetUpperBound(1)].Bottom);
            RestartButton.Width = _createdFieldSize.Width;
        }
        private void UpdateResetWinsButtonPosSize()
        {
            ResetWinsButton.Location = new Point(RestartButton.Left, RestartButton.Bottom);
            ResetWinsButton.Width = _createdFieldSize.Width;
        }
        private void RestartButtonClick(object? sender, EventArgs e)
        {
            if (sender is Button)
            {
                foreach (TTTButton button in _buttons)
                {
                    button.Symbol = TTTButton.Symbols.None;
                    button.ForeColor = Color.Black;
                }
                IsPlaying = true;
                StartGame();
            }
        }
        private void ResetWinsButtonClick(object? sender, EventArgs e)
        {
            FileManager.ResetWinsFile();
        }
        private void FieldWidthChanged(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown castedSender)
            {
                _fieldWidth = (int)castedSender.Value;
            }
        }
        private void FieldHeightChanged(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown castedSender)
            {
                _fieldHeight = (int)castedSender.Value;
            }
        }
    }
}