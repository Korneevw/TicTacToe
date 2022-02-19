using System.IO;
namespace TicTacToe
{
    public partial class Form1 : Form
    {
        private bool IsPlaying = true;
        private const int Indent = 4;
        private int _turn = 0;
        private int _crossWins;
        private int _circleWins;
        private int _fieldWidth = 3;
        private int _fieldHeight = 3;
        private Label _wins;
        private Label _currentTurn;
        private Button _restart;
        private Label _sizeLabel;
        private NumericUpDown _fieldWidthInput;
        private NumericUpDown _fieldHeightInput;
        private Button _tttButtonPattern;
        private Size _fieldSize;
        private TTTButton[,] _buttons = new TTTButton[0, 0];
        private List<List<TTTButton>> _winCombinations = new List<List<TTTButton>>();
        public Form1()
        {
            _sizeLabel = new Label()
            {
                Text = "Field size (width, height):",
                Height = Font.Height + 2,
                Location = new Point(Indent, Indent),
                BorderStyle = BorderStyle.FixedSingle,
            };
            _fieldWidthInput = new NumericUpDown()
            {
                Maximum = 12,
                Minimum = 2,
                Value = 3,
                Location = new Point(_sizeLabel.Left, _sizeLabel.Bottom - 1),
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

            _tttButtonPattern = new Button()
            {
                Size = new Size(50, 50),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            _wins = new Label()
            {
                Text = $"Game hasn't started yet.\n ",
                Height = Font.Height * 2 + 2,
                Location = new Point(_sizeLabel.Left, _fieldWidthInput.Bottom - 1),
                BorderStyle = BorderStyle.FixedSingle,       
            };
            _currentTurn = new Label()
            {
                Text = $"Game hasn't started yet.\n ",
                Height = Font.Height + 2,
                Location = new Point(_wins.Left, _wins.Bottom - 1),
                BorderStyle = BorderStyle.FixedSingle,
            };
            CreateField(new Point(Indent, _currentTurn.Bottom), _tttButtonPattern, out _fieldSize);
            UpdateFieldSizes();
            _restart = new Button()
            {
                Text = $"Restart",
                Location = new Point(Indent, _buttons[0, _buttons.GetUpperBound(1)].Bottom),
                Width = _wins.Width,
                Height = _buttons[0, 0].Height
            };
            
            _restart.Click += RestartButtonClick;
            Controls.Add(_wins);
            Controls.Add(_currentTurn);
            Controls.Add(_restart);
            Controls.Add(_sizeLabel);
            Controls.Add(_fieldWidthInput);
            Controls.Add(_fieldHeightInput);
            InitializeComponent();
            Text = "TicTacToe";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FillWinningCombinations();
            UpdateFieldSizeInputEnabled();
            UpdateRestartButtonEnabled();
            CreateDataFile();
            ReadDataFile();
            UpdateWinsLabelText();
            UpdateCurrentTurnLabelText();
        }
        private void CreateField(Point startingPoint, Button pattern, out Size resultSize)
        {
            if (_buttons != null)
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
                        Symbol = TTTButton.Symbols.None,
                        Location = new Point(startingPoint.X + pattern.Width * i, startingPoint.Y + pattern.Height * j)
                    };
                    _buttons[i, j].Click += TTTButtonClick;
                    _buttons[i, j].pos = (i, j);
                    Controls.Add(_buttons[i, j]);
                    resultSize = new Size(pattern.Width * (i + 1), pattern.Height * (i + 1));
                }
            }
        }
        private void TTTButtonClick(object? sender, EventArgs e)
        {
            if (!IsPlaying ) return;
            if (sender is TTTButton castedSender)
            {
                if (castedSender.Symbol != TTTButton.Symbols.None) return;
                castedSender.Symbol = _turn % 2 == 0 ? TTTButton.Symbols.X : TTTButton.Symbols.O;
                _turn++;
                CheckWin();
                UpdateCurrentTurnLabelText();
            }
        }
        private void FillWinningCombinations()
        {
            if (_winCombinations.Any()) _winCombinations.Clear();
            for (int i = 0; i <= _buttons.GetUpperBound(1); i++) // Horizontal
            {
                List<TTTButton> horizontal = new List<TTTButton>();
                for (int j = 0; j <= _buttons.GetUpperBound(0); j++)
                {
                    horizontal.Add(_buttons[j, i]);
                }
                _winCombinations.Add(horizontal);
            }
            for (int i = 0; i <= _buttons.GetUpperBound(0); i++) // Vertical
            {
                List<TTTButton> vertical = new List<TTTButton>();
                for (int j = 0; j <= _buttons.GetUpperBound(1); j++)
                {
                    vertical.Add(_buttons[i, j]);
                }
                _winCombinations.Add(vertical);
            }
            if (_buttons.GetLength(0) == _buttons.GetLength(1)) // If field is a square
            {
                List<TTTButton> leftDiagonal = new List<TTTButton>();
                for (int i = 0; i < Math.Sqrt(_buttons.Length); i++) // Left diagonal
                {
                    leftDiagonal.Add(_buttons[i, i]);
                }
                _winCombinations.Add(leftDiagonal);
                List<TTTButton> rightDiagonal = new List<TTTButton>();
                for (int i = 0; i < Math.Sqrt(_buttons.Length); i++) // Right diagonal
                {
                    rightDiagonal.Add(_buttons[i, (int)Math.Sqrt(_buttons.Length) - 1 - i]);
                }
                _winCombinations.Add(rightDiagonal);
            }
        }
        private void CheckWin()
        {
            foreach (List<TTTButton> combination in _winCombinations)
            {
                if (combination.All(b => b.Symbol == TTTButton.Symbols.X)) // If combination is filled with X
                {
                    _crossWins++;
                    EndGame(combination);
                    break;
                }
                else if (combination.All(b => b.Symbol == TTTButton.Symbols.O)) // If combination is filled with O
                {
                    _circleWins++;
                    EndGame(combination);
                    break;
                }
            }
            if (_winCombinations.All(c => c.All(b => b.Symbol != TTTButton.Symbols.None))) // If the whole field is filled
            {
                EndGame();
            }
        }
        private void EndGame()
        {
            IsPlaying = false;
            UpdateRestartButtonEnabled();
            UpdateFieldSizeInputEnabled();
            UpdateWinsLabelText();
            WriteDataFile();
        }
        private void EndGame(List<TTTButton> winningCombination)
        {
            winningCombination.ForEach(b => b.ForeColor = Color.Red);
            IsPlaying = false;
            UpdateRestartButtonEnabled();
            UpdateFieldSizeInputEnabled();
            UpdateWinsLabelText();
            WriteDataFile();
        }
        private void StartGame()
        {
            CreateField(new Point(Indent, _currentTurn.Bottom), _tttButtonPattern, out _fieldSize);
            FillWinningCombinations();
            UpdateRestartButtonPosSize();
            UpdateFieldSizeInputEnabled();
            UpdateRestartButtonEnabled();
            UpdateFieldSizes();
            UpdateWinsLabelText();
            UpdateCurrentTurnLabelText();
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
        private void UpdateRestartButtonEnabled()
        {
            if (IsPlaying) _restart.Enabled = false;
            else _restart.Enabled = true;
        }
        private void UpdateRestartButtonPosSize()
        {
            _restart.Location = new Point(_buttons[0, _buttons.GetUpperBound(1)].Left, _buttons[0, _buttons.GetUpperBound(1)].Bottom);
            _restart.Width = _fieldSize.Width;
        }
        private void UpdateFieldSizes()
        {
            _sizeLabel.Width = _fieldSize.Width;
            _fieldWidthInput.Width = _sizeLabel.Width / 2;
            _fieldHeightInput.Width = _sizeLabel.Width / 2;
            _fieldHeightInput.Location = new Point(_fieldWidthInput.Right, _fieldWidthInput.Top);
            _wins.Width = _sizeLabel.Width;
            _currentTurn.Width = _wins.Width;
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
        private void UpdateCurrentTurnLabelText()
        {
            _currentTurn.Text = $"Now it's " + (_turn % 2 == 0 ? "cross" : "circle") + "'s turn.";
        }
        private void UpdateWinsLabelText()
        {
            _wins.Text = $"Cross wins: {_crossWins}. \nCircle wins: {_circleWins}.";
        }
        private void CreateDataFile()
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TicTacToeData")))
            {
                Directory.CreateDirectory(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TicTacToeData"));
            }
            if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"TicTacToeData/TicTacToeData.txt")))
            {
                File.Create(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"TicTacToeData/TicTacToeData.txt")).Close();
            }
        }
        private void ReadDataFile()
        {
            string[] data = File.ReadAllLines(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"TicTacToeData/TicTacToeData.txt"));
            if (data.Length == 2)
            {
                _crossWins = int.Parse(data[0]);
                _circleWins = int.Parse(data[1]);
            }
        }
        private void WriteDataFile()
        {
            string[] data = new string[] { _crossWins.ToString(), _circleWins.ToString() };
            File.WriteAllLines(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"TicTacToeData/TicTacToeData.txt"), data);
        }
    }
}