using Chess.Models.Game;
using Chess.Util.ModelUtil;
using System.Collections.ObjectModel;
using System.Windows;

namespace Chess.ViewModels
{
    internal class ChessBoardVM : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Backing field: The reference to the ChessBoard class in Model
        /// </summary>
        private readonly ChessBoard _chessboard;

        /// <summary>
        /// Gets the The observable collection that holds all the Piece currently in the board. The UI will use this for UI binding
        /// </summary>
        public ObservableCollection<SquareVM> Squares { get; private set; } = [];

        public PawnPromotionVM PawnPromotionVM { get; }
        #endregion //End Fields

        /// <summary>
        /// The constructor method of ChessBoardVM class
        /// </summary>
        public ChessBoardVM()
        {
            //Initialize new ChessBoard -> will change to Game class instead and leave the initialization of ChessBoard to Game
            _chessboard = new ChessBoard();

            //Populate the Squares collection
            SquareVM sq;
            Piece? piece;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    try
                    {
                        //Get piece at position (row, col)
                        piece = _chessboard.GetPieceAt(row, col);
                        //Create new SquareVM object and add it to the Squares Observable List
                        sq = new SquareVM(new Position(row, col), piece, this);
                        Squares.Add(sq);
                    }
                    catch (Exception e)
                    {
                        //Will add proper logging here
                        Test.Log($"Message: {e.Message}\nError: {e}");
                    }
                }
            }

            //Initialize new PawnPromotionVM
            PawnPromotionVM = new PawnPromotionVM(this);
        }

        /// <summary>
        /// Method for handling the click event on the square. It will check for the current state of the board 
        /// to decide which action to be taken.
        /// </summary>
        /// <param name="sq">The SquareVM being clicked at</param>
        public void OnClick(SquareVM sq)
        {
            /*
             * Algorithm explains: First, we de-select/unhighlight all pieces (only the data in SquareVM classes, not the one in Model),
             * because whatever action can be taken will affect the whole board, so we reset them
             * 1. If there is a piece being selected (in model class), and the user choose a valid square that can move to -> Move action
             * 2. If the user choose an empty square or opponent piece but not valid move, or choosing the same selected piece -> De-select 
             * action
             * 3. Otherwise -> Select action
             */

            Piece? selectedPiece = _chessboard.SelectedPiece;
            ResetAllSquaresState();

            //Check for special case (castling)
            if (selectedPiece is not null && selectedPiece.Rank == Rank.KING &&
                    sq.Piece is not null && sq.Piece.Rank == Rank.ROOK && sq.Piece.Color == selectedPiece.Color)
            {
                if (sq.Position.Equals(new Position(0, 0)) || sq.Position.Equals(new Position(7, 0)))
                {
                    _chessboard.LongCastling();
                }
                else if (sq.Position.Equals(new Position(0, 7)) || sq.Position.Equals(new Position(7, 7)))
                {
                    _chessboard.ShortCastling();
                }
                //De-select the King
                _chessboard.SelectedPiece = null;

                //Update the state of the board
                Update();

                //Check if this leads to a checkmate (very unlikely, but it may be)
                if (_chessboard.IsCheckmate())
                {
                    MessageBox.Show("Checkmate!");
                }

                return;
            }

            //If the there is a piece being selected, and the square user choose can be move to for that selected piece, move piece
            if (selectedPiece is not null && selectedPiece.HasMove(sq.Position))
            {
                _chessboard.Move(selectedPiece, sq.Position);
                _chessboard.SelectedPiece = null;

                //After the move, we check if this move is the pawn reaching the other end -> pawn promotion event
                if (_chessboard.PawnForPromotion is not null)
                {
                    PawnPromotionVM.GetColor(_chessboard.PawnForPromotion.Color);
                    PawnPromotionVM.IsVisible = true;
                }

                //After moving, the state of the board changes, so we have to update the Observable Collections Squares
                Update();

                //Check if move leads to end game(checkmate)
                if (_chessboard.IsCheckmate())
                {
                    MessageBox.Show("Checkmate!");
                }

                return;
            }

            //If the user click another side's piece or an empty square, but it not a valid move, de-select the piece
            if (sq.Piece is null || (_chessboard.IsWhiteTurn ? sq.Piece.Color == Color.BLACK : sq.Piece.Color == Color.WHITE))
            {
                _chessboard.SelectedPiece = null;
                return;
            }

            //If the user click the same piece as the selected piece, release piece (set selected piece to null)
            if (selectedPiece is not null && sq.Piece is not null && selectedPiece.Equals(sq.Piece))
            {
                _chessboard.SelectedPiece = null;
                return;
            }

            //Else, simply select the piece (update the state in Model)
            _chessboard.SelectedPiece = sq.Piece;
            //We update the selected state of this square after select a piece (update the state in View Model)
            sq.IsPieceSelected = true;
            //We loop through the list of move list and update their highlight state
            foreach (var square in Squares)
            {
                //This cannot be null, so we don't have to check for this
                if (_chessboard.SelectedPiece is not null && _chessboard.SelectedPiece.HasMove(square.Position))
                {
                    square.IsHighlighted = true;
                }
            }
        }

        /// <summary>
        /// Method for updating the new state of the the board after moving a Piece
        /// </summary>
        public void Update()
        {
            foreach (var square in Squares)
            {
                var newPiece = _chessboard.GetPieceAt(square.Position.Row, square.Position.Column);

                //Update the Piece in this square corresponding to the data in Model
                square.Piece = newPiece;
                //Update the image
                square.ImageSource = newPiece is null ? "" : square.GetImageSource();
            }
        }

        /// <summary>
        /// Method for reseting the state of all square (all states here are IsHighlighted and IsPieceSelected boolean values)
        /// </summary>
        private void ResetAllSquaresState()
        {
            //We loop through the List and check if the current square holds the select piece and de-select + unhighlight it
            foreach (var square in Squares)
            {
                square.IsPieceSelected = false;
                square.IsHighlighted = false;
            }
        }

        public void PawnPromote(Rank newRank)
        {
            if (_chessboard.PawnForPromotion is not null)
            {
                _chessboard.PawnPromotion(_chessboard.PawnForPromotion, newRank);
                Update();
                _chessboard.PawnForPromotion = null;
            }
        }

    }
}
