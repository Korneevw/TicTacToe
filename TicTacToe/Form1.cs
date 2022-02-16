namespace TicTacToe
{
    public partial class Form1 : Form
    {
        // Github, yay!
        private bool IsPlaying = true;
        private const int Indent = 10;
        private int _turn = 0;
        private int _crossWins;
        private int _circleWins;
        private Label _wins;
        private Label _currentTurn;
        private Button _restart;
        private Button _tttButtonPattern;
        private Size _fieldSize;
        private TTTButton[,] _buttons = new TTTButton[3, 3];
        private List<List<TTTButton>> _winCombinations = new List<List<TTTButton>>();
        public Form1()
        {
            _tttButtonPattern = new Button()
            {
                Size = new Size(50, 50),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            _wins = new Label()
            {
                Text = $"Игра ещё не началась.\n ",
                Height = Font.Height * 2 + 2,
                Location = new Point(Indent, Indent),
                BorderStyle = BorderStyle.FixedSingle,       
            };
            _currentTurn = new Label()
            {
                Text = $"Игра ещё не началась.\n ",
                Height = Font.Height + 2,
                Location = new Point(_wins.Left, _wins.Bottom - 1),
                BorderStyle = BorderStyle.FixedSingle,
            };
            CreateField(new Point(Indent, _currentTurn.Bottom), _tttButtonPattern, out _fieldSize);
            _wins.Width = _fieldSize.Width;
            _currentTurn.Width = _wins.Width;
            _restart = new Button()
            {
                Text = $"Начать заново",
                Location = new Point(Indent, _buttons[0, _buttons.GetUpperBound(1)].Bottom),
                Width = _wins.Width,
                Height = _buttons[0, 0].Height
            };
            _restart.Click += RestartButtonClick;
            Controls.Add(_wins);
            Controls.Add(_currentTurn);
            Controls.Add(_restart);
            InitializeComponent();
            Text = "Крестики-Нолики";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = MaximumSize = new Size(186, 330);
            OnStartup();
        }
        private void OnStartup()
        {
            FillWinningCombinations();
            UpdateWinsLabel();
            UpdateRestartButton();
            UpdateCurrentTurnLabel();
        }
        private void CreateField(Point startingPoint, Button pattern, out Size resultSize)
        {
            resultSize = new Size(0, 0);
            for (int i = 0; i <= _buttons.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _buttons.GetUpperBound(1); j++)
                {
                    _buttons[i, j] = new TTTButton()
                    {
                        Size = pattern.Size,
                        Font = pattern.Font,
                    };
                    _buttons[i, j].Location = new Point(startingPoint.X + pattern.Width * i, startingPoint.Y + pattern.Height * j);
                    _buttons[i, j].Symbol = TTTButton.Symbols.None;
                    _buttons[i, j].Click += TTTButtonClick;
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
                UpdateCurrentTurnLabel();
            }
        }
        private void UpdateRestartButton()
        {
            if (IsPlaying) _restart.Enabled = false;
            else _restart.Enabled = true;
        }
        private void UpdateCurrentTurnLabel()
        {
            _currentTurn.Text = $"Сейчас ходит " + (_turn % 2 == 0 ? "крестик" : "нолик");
        }
        private void UpdateWinsLabel()
        {
            _wins.Text = $"Победы креста: {_crossWins}. \nПобеды круга: {_circleWins}.";
        }
        private void RestartButtonClick(object? sender, EventArgs e)
        {
            if (sender is Button castedSender)
            {
                foreach (TTTButton button in _buttons)
                {
                    button.Symbol = TTTButton.Symbols.None;
                    button.ForeColor = Color.Black;
                }
                IsPlaying = true;
                UpdateRestartButton();
            }
        }
        private void FillWinningCombinations()
        {
            if (_winCombinations.Count > 0) _winCombinations.Clear();
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
            if (_winCombinations.All(c => c.All(b => b.Symbol != TTTButton.Symbols.None))) // If the whole field is filled
            {
                EndGame();
            }
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
        }
        private void EndGame()
        {
            IsPlaying = false;
            UpdateRestartButton();
            UpdateWinsLabel();
        }
        private void EndGame(List<TTTButton> winningCombination)
        {
            winningCombination.ForEach(b => b.ForeColor = Color.Red);
            IsPlaying = false;
            UpdateRestartButton();
            UpdateWinsLabel();
        }
    }
}