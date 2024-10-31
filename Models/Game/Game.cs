

namespace Chess.Models.Game
{
    class Game
    {
        #region Fields
        public ChessBoard ChessBoard { get; }

        //This variable track the current move is in the board, used for move back/move forward action
        private int _currentMove = -1; 
        public List<string> Moves { get; } = [];

        #endregion

        public Game()
        {
            ChessBoard = new ChessBoard();
        }

        public void NewGame()
        {
            Save();
            ChessBoard.InitNewGame();
            Moves.Clear();
        }

        public void Moveback()
        {
            if (_currentMove > 0)
            {
                _currentMove--;
                //We get the last move and recreate the state of the boarduy
            }
        }

        public void Save()
        {

        }

    }
}
