namespace TicTacToe
{
    internal class TTTButton : Button
    {
        public enum Symbols
        {
            X,
            O,
            None
        }
        private Symbols _symbol;
        public (int, int) pos;
        public Symbols Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
                Text = value == Symbols.None ? "" : value.ToString();
            }
        }
    }
}
