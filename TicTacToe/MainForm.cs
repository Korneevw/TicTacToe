#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace TicTacToe
{
    /// <summary>The main game window.</summary>
    public partial class MainForm : Form
    {
        public static bool Dev { get; } = true;
        private const int Indent = 4;
        public static Label WinsLabel { get; set; }
        public static Label CurrentTurnLabel { get; set; }
        public static Label FieldSizeLabel { get; set; }
        public static Button StartEndButton { get; set; }
        public static Button ResetWinsButton { get; set; }
        public static Button ImportWinsButton { get; set; }
        public static Button ExportWinsButton { get; set; }
        public static ListBox LanguageSelector { get; set; }
        public static ListBox ThemeSelector { get; set; }
        public static int CrossWins { get; set; }
        public static int CircleWins { get; set; }
        public static int Draws { get; set; }
        public static int Turn { get; private set; } = 0;
        public static bool IsPlaying { get; private set; } = false;
        private int _fieldWidth = 3;
        private int _fieldHeight = 3;
        private Size _createdFieldSize;
        private NumericUpDown _fieldWidthInput;
        private NumericUpDown _fieldHeightInput;
        private TTTButton _tttButtonPattern;
        private TTTButton[,] _field = new TTTButton[0, 0];
        private List<List<TTTButton>> _winCombinations = new List<List<TTTButton>>();
        public MainForm()
        {
            LanguageSelector = new ListBox()
            {
                Location = new Point(Indent, Indent),
                BorderStyle = BorderStyle.FixedSingle,
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
            this.Text = TranslationManager.Translation[TranslationManager.TranslatableText.MainFormName];
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FileManager.ReadWinsFile();
            TranslationManager.UpdateWinsLabelText();
            TranslationManager.UpdateCurrentTurnLabelText();
        }
        private void LanguageSelectorSelectedValueChanged(object? sender, EventArgs e)
        {
            TranslationManager.GetTranslation();
            TranslationManager.UpdateAllText();
            this.Text = TranslationManager.Translation[TranslationManager.TranslatableText.MainFormName];
        }
        private void CreateControls()
        {
            FieldSizeLabel = new Label() // Labels the field size NumericUpDowns.
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.FieldSizeLabel],
                Height = Font.Height + 2,
                Location = new Point(LanguageSelector.Left, LanguageSelector.Bottom - 5),
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
            _fieldWidthInput.ValueChanged += FieldWidthInputValueChanged;
            
            _fieldHeightInput = new NumericUpDown()
            {
                Maximum = 12,
                Minimum = 2,
                Value = 3,
                Location = new Point(_fieldWidthInput.Right, _fieldWidthInput.Top),
            };
            _fieldHeightInput.ValueChanged += FieldHeightInputValueChanged;
            
            _tttButtonPattern = new TTTButton()
            {
                Size = new Size(75, 75),
                Font = new Font("Arial", 35, FontStyle.Bold),
                BackColor = Color.White,
            };

            WinsLabel = new Label() // Displays the wins.
            {
                Height = Font.Height * 3 + 2,
                Location = new Point(LanguageSelector.Left, _fieldWidthInput.Bottom - 1),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
            };

            CurrentTurnLabel = new Label() // Displays the current turn.
            {
                Height = Font.Height + 2,
                Location = new Point(LanguageSelector.Left, WinsLabel.Bottom - 1),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
            };
            CreateField(new Point(Indent, CurrentTurnLabel.Bottom), _tttButtonPattern, out _createdFieldSize);
            UpdateControlSizes();

            StartEndButton = new Button() // Restarts the game (clears the field).
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.StartEndButtonStart],
                Location = new Point(_field[0, _field.GetUpperBound(1)].Left, _field[0, _field.GetUpperBound(1)].Bottom),
                Width = _createdFieldSize.Width,
                Height = _tttButtonPattern.Height,
                BackColor = Color.White,
            };
            StartEndButton.Click += EndStartButtonClick;

            ResetWinsButton = new Button() // Resets the wins file.
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.StartEndButtonStart],
                Location = new Point(StartEndButton.Left, StartEndButton.Bottom),
                Width = StartEndButton.Width,
                Height = StartEndButton.Height / 2,
                BackColor = Color.White,
            };
            ResetWinsButton.Click += (object? sender, EventArgs e) => FileManager.ResetWinsFile();

            ImportWinsButton = new Button()
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.ImportWinsButton],
                Location = new Point(ResetWinsButton.Left, ResetWinsButton.Bottom),
                Width = ResetWinsButton.Width / 2,
                Height = ResetWinsButton.Height,
                BackColor = Color.White,
            };
            ImportWinsButton.Click += (object? sender, EventArgs e) => FileManager.ImportWinsFile();

            ExportWinsButton = new Button()
            {
                Text = TranslationManager.Translation[TranslationManager.TranslatableText.ExportWinsButton],
                Location = new Point(ImportWinsButton.Right, ResetWinsButton.Bottom),
                Width = ResetWinsButton.Width / 2,
                Height = ResetWinsButton.Height,
                BackColor = Color.White,
            };
            ExportWinsButton.Click += (object? sender, EventArgs e) => FileManager.ExportWinsFile();

            Controls.AddRange(new Control[] { FieldSizeLabel, _fieldWidthInput, _fieldHeightInput, WinsLabel, 
                CurrentTurnLabel, StartEndButton, ResetWinsButton, ImportWinsButton, ExportWinsButton });
            InitializeComponent();
        }
        /// <summary>Creates TTTButtons for the field.</summary>
        /// <param name="startingPoint">Where to create the field.</param>
        /// <param name="pattern">Pattern for each field cell (TTTButton).</param>
        /// <param name="resultSize">Size of the created field.</param>
        private void CreateField(Point startingPoint, TTTButton pattern, out Size resultSize)
        {
            if (_field is not null)
            {
                foreach (TTTButton b in _field)
                {
                    Controls.Remove(b);
                }
            }
            _field = new TTTButton[_fieldWidth, _fieldHeight];
            resultSize = new Size(0, 0);
            for (int i = 0; i <= _field.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _field.GetUpperBound(1); j++)
                {
                    _field[i, j] = new TTTButton()
                    {
                        Size = pattern.Size,
                        Font = pattern.Font,
                        BackColor = pattern.BackColor,
                        Symbol = TTTButton.Symbols.None,
                        Location = new Point(startingPoint.X + pattern.Width * i, startingPoint.Y + pattern.Height * j),
                    };
                    _field[i, j].Click += TTTButtonClick;
                    Controls.Add(_field[i, j]);
                    resultSize = new Size(pattern.Width * (i + 1), pattern.Height * (i + 1));
                }
            }
        }
        private void CreateWinningCombinations()
        {
            if (_winCombinations.Any()) _winCombinations.Clear();
            for (int i = 0; i <= _field.GetUpperBound(1); i++) // Horizontal.
            {
                List<TTTButton> horizontal = new List<TTTButton>();
                for (int j = 0; j <= _field.GetUpperBound(0); j++)
                {
                    horizontal.Add(_field[j, i]);
                }
                _winCombinations.Add(horizontal);
            }
            for (int i = 0; i <= _field.GetUpperBound(0); i++) // Vertical.
            {
                List<TTTButton> vertical = new List<TTTButton>();
                for (int j = 0; j <= _field.GetUpperBound(1); j++)
                {
                    vertical.Add(_field[i, j]);
                }
                _winCombinations.Add(vertical);
            }
            if (_field.GetLength(0) == _field.GetLength(1)) // If field is a square:
            {
                List<TTTButton> leftDiagonal = new List<TTTButton>();
                for (int i = 0; i < Math.Sqrt(_field.Length); i++) // Left diagonal.
                {
                    leftDiagonal.Add(_field[i, i]);
                }
                _winCombinations.Add(leftDiagonal);
                List<TTTButton> rightDiagonal = new List<TTTButton>();
                for (int i = 0; i < Math.Sqrt(_field.Length); i++) // Right diagonal.
                {
                    rightDiagonal.Add(_field[i, (int)Math.Sqrt(_field.Length) - 1 - i]);
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
                TranslationManager.UpdateCurrentTurnLabelText();
                CheckWin();
            }
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
                Draws++;
                EndGame();
            }
        }
        private void StartGame()
        {
            IsPlaying = true;
            CreateField(new Point(Indent, CurrentTurnLabel.Bottom), _tttButtonPattern, out _createdFieldSize);
            UpdateControlSizes();
            AlignBottomButtons();
            CreateWinningCombinations();
            _fieldHeightInput.Enabled = false;
            _fieldWidthInput.Enabled = false;
            ResetWinsButton.Enabled = false;
            ImportWinsButton.Enabled = false;
            ExportWinsButton.Enabled = false;
            TranslationManager.UpdateWinsLabelText();
            TranslationManager.UpdateCurrentTurnLabelText();
            TranslationManager.UpdateStartEndButtonText();
        }
        private void EndGame()
        {
            IsPlaying = false;
            StartEndButton.Enabled = true;
            ResetWinsButton.Enabled = true;
            ImportWinsButton.Enabled = true;
            ExportWinsButton.Enabled = true;
            _fieldHeightInput.Enabled = true;
            _fieldWidthInput.Enabled = true;
            TranslationManager.UpdateWinsLabelText();
            FileManager.WriteWinsFile();
            TranslationManager.UpdateCurrentTurnLabelText();
            TranslationManager.UpdateStartEndButtonText();
        }
        /// <summary>Ends the game, and highlights the <paramref name="winningCombination"/>.</summary>
        private void EndGame(List<TTTButton> winningCombination)
        {
            winningCombination.ForEach(b => b.ForeColor = Color.Red);
            EndGame();
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
        /// <summary>Aligns startEnd and reset buttons according to created field size.</summary>
        private void AlignBottomButtons()
        {
            StartEndButton.Location = new Point(_field[0, _field.GetUpperBound(1)].Left, _field[0, _field.GetUpperBound(1)].Bottom);
            StartEndButton.Width = _createdFieldSize.Width;
            ResetWinsButton.Location = new Point(StartEndButton.Left, StartEndButton.Bottom);
            ResetWinsButton.Width = _createdFieldSize.Width;
            ImportWinsButton.Location = new Point(ResetWinsButton.Left, ResetWinsButton.Bottom);
            ImportWinsButton.Width = _createdFieldSize.Width / 2;
            ExportWinsButton.Location = new Point(ImportWinsButton.Right, ResetWinsButton.Bottom);
            ExportWinsButton.Width = _createdFieldSize.Width / 2;
        }
        /// <summary>Ends the game if playing. Starts the game if not playing.</summary>
        private void EndStartButtonClick(object? sender, EventArgs e)
        {
            if (IsPlaying)
            {
                foreach (TTTButton button in _field)
                {
                    button.Symbol = TTTButton.Symbols.None;
                    button.ForeColor = Color.Black;
                }
                EndGame();
            }
            else if (!IsPlaying)
            {
                StartGame();
            }
        }
        private void FieldWidthInputValueChanged(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown castedSender)
            {
                _fieldWidth = (int)castedSender.Value;
            }
        }
        private void FieldHeightInputValueChanged(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown castedSender)
            {
                _fieldHeight = (int)castedSender.Value;
            }
        }
    }
}