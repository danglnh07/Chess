using Chess.Util.ModelUtil;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Controls;

namespace Chess.Models.Game
{
    partial class ChessBoard
    {
        #region Fields
        private readonly Piece?[,] _board = default!;
        public Piece?[,] Board { get { return _board; } }

        public Piece BlackKing { get; set; } = default!;
        public Piece WhiteKing { get; set; } = default!;

        private Piece? _selectedPiece;
        public Piece? SelectedPiece 
        { 
            get => _selectedPiece; 
            set
            {
                _selectedPiece = value;
                if (_selectedPiece is not null)
                {
                    List<Position> moves = new(_selectedPiece.CanMove);
                    List<Position> invalid = [];

                    String move;
                    foreach (var pos in moves) 
                    {
                        move = Move(_selectedPiece, pos);
                        if (IsChecked(IsWhiteTurn ? BlackKing : WhiteKing))
                        {
                            Test.Log("Invalid move");
                            invalid.Add(pos);
                        }
                        UndoNormalMove(move);
                    }

                    foreach (var pos in invalid)
                    {
                        _selectedPiece.RemoveMove(pos);
                    }
                }
            } 
        }

        public bool IsWhiteTurn { get; set; } = true;

        /// <summary>
        /// We used 4 bits to represent the ability to castle. The order is: 
        /// Top left (Black Long Castling) 
        /// Top right (Black Short Castling) 
        /// Bottom right (White Short Castling) 
        /// Bottom left (White Long Castling)
        /// </summary>
        private int _canCastle = 15;

        public Piece? PawnForPromotion { get; set; } = null;

        public int _currentMove = 0;
        public int CurrentMove
        {
            get => _currentMove;
            set
            {
                if (0 <= value && value <= Moves.Count)
                {
                    _currentMove = value;
                }
            }
        }

        public List<string> Moves { get; } = [];
        #endregion //End Field

        public ChessBoard()
        {
            _board = new Piece?[8, 8];
            InitNewGame();
        }

        public void InitNewGame()
        {
            //Create the pieces
            try
            {
                //Set up the pawns at two sides
                for (int col = 0; col < 8; col++)
                {
                    _board[1, col] = new Piece(1, col, Color.BLACK, Rank.PAWN);
                    _board[6, col] = new Piece(6, col, Color.WHITE, Rank.PAWN);
                }

                //Set up the rooks at two sides
                _board[0, 0] = new Piece(0, 0, Color.BLACK, Rank.ROOK);
                _board[0, 7] = new Piece(0, 7, Color.BLACK, Rank.ROOK);
                _board[7, 0] = new Piece(7, 0, Color.WHITE, Rank.ROOK);
                _board[7, 7] = new Piece(7, 7, Color.WHITE, Rank.ROOK);

                //Set up the knights at two sides
                _board[0, 1] = new Piece(0, 1, Color.BLACK, Rank.KNIGHT);
                _board[0, 6] = new Piece(0, 6, Color.BLACK, Rank.KNIGHT);
                _board[7, 1] = new Piece(7, 1, Color.WHITE, Rank.KNIGHT);
                _board[7, 6] = new Piece(7, 6, Color.WHITE, Rank.KNIGHT);

                //Set up the bishops at two sides
                _board[0, 2] = new Piece(0, 2, Color.BLACK, Rank.BISHOP);
                _board[0, 5] = new Piece(0, 5, Color.BLACK, Rank.BISHOP);
                _board[7, 2] = new Piece(7, 2, Color.WHITE, Rank.BISHOP);
                _board[7, 5] = new Piece(7, 5, Color.WHITE, Rank.BISHOP);

                //Set up the queens at two sides
                _board[0, 3] = new Piece(0, 3, Color.BLACK, Rank.QUEEN);
                _board[7, 3] = new Piece(7, 3, Color.WHITE, Rank.QUEEN);

                //Set up the kings at two sides and record the kings to WhiteKing and BlackKing
                BlackKing = new Piece(0, 4, Color.BLACK, Rank.KING);
                _board[0, 4] = BlackKing;
                WhiteKing = new Piece(7, 4, Color.WHITE, Rank.KING);
                _board[7, 4] = WhiteKing;

                //Update all canMove list 
                UpdateMoves();

                //Clear the remaining rows (in case this is a replay action)
                for (int row = 2; row < 6; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        _board[row, col] = null;
                    }
                }

                //Set up the castling bits to all true (1111 -> 15)
                _canCastle = 15;
            }
            catch (ArgumentException e)
            {
                Test.Log($"Error setting the board -> Error: {e}");
            }
        }

        public Piece? GetPieceAt(Position pos)
        {
            return GetPieceAt(pos.Row, pos.Column);
        }

        public Piece? GetPieceAt(int row, int col)
        {
            if (!Validator.IsPositionValid(row, col))
            {
                throw new ArgumentException("Invalid row and column");
            }
            return _board[row, col];
        }

        public bool IsChecked(Piece king)
        {
            Piece? piece;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    piece = GetPieceAt(row, col);
                    //If this current piece is of the opponent side and its has the king position in its canMove list -> checked
                    if (piece is not null && piece.Color != king.Color && piece.HasMove(king.Position))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsCheckmate()
        {
            /*
             * Algorithm explains: because the SelectedPiece will calculate all the possible valid move a Piece can make, so we just have
             * to check that, does the current player can still make a move by selecting every Piece that player currently has in the board. 
             * That also means that it cannot detect a draw state (for example, both side has only King left, they can still make a move
             * but the game cannot be ended).
             */

            Piece? p;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    p = GetPieceAt(row, col);
                    if (p is not null && (IsWhiteTurn ? p.Color == Color.WHITE : p.Color == Color.BLACK))
                    {
                        Piece? old = SelectedPiece;
                        SelectedPiece = p;
                        if (p.CanMove.Count != 0)
                        {
                            SelectedPiece = old;
                            return false;
                        }
                        SelectedPiece = old;
                    }
                }
            }

            return true;
        }



        public string Move(Piece piece, Position pos)
        {
            return Move(piece, pos.Row, pos.Column);
        }

        public string Move(Piece piece, int row, int column)
        {
            if (piece is null)
            {
                return "";
            }

            //Check if the destination is valid
            if (!Validator.IsPositionValid(row, column))
            {
                throw new ArgumentException("Invalid destination!");
            }

            //Record the old row and col
            var oldRow = piece.Position.Row;
            var oldColumn = piece.Position.Column;

            //Set the old position in the board to null
            _board[oldRow, oldColumn] = null;
            //Update the internal coordinate of the piece
            piece.Position = new Position(row, column);
            //We get the current piece at the destination square
            Piece? takenPiece = GetPieceAt(row, column);
            //Move the piece
            _board[row, column] = piece;

            //If the King has move, we disqualify both long and short castling for that side (King cannot be taken by rule so we don't check)
            if (piece.Rank == Rank.KING)
            {
                /*
                 * abcd AND 0011 = 00cd (disqualify the Black castling)
                 * abcd ANd 1100 = ab00 (disqualify the White castling)
                 */
                _canCastle &= (IsWhiteTurn ? 3 : 12);
            }
            //Else, if the moved piece is a Rook, or the taken piece is a Rook, disqualified only one side
            else
            {
                if (oldRow == 0 && oldColumn == 0 || takenPiece is not null && takenPiece.Rank == Rank.ROOK && row == 0 && column == 0)
                {
                    //Disqualify Black Long Castling (abcd AND 0111 = 0bcd)
                    _canCastle &= 7;
                }
                else if (oldRow == 0 && oldColumn == 7 || takenPiece is not null && takenPiece.Rank == Rank.ROOK && row == 0 && column == 7)
                {
                    //Disqualify Black Short Castling (abcd AND 1011 = a0cd)
                    _canCastle &= 11;
                }
                else if (oldRow == 7 && oldColumn == 7 || takenPiece is not null && takenPiece.Rank == Rank.ROOK && row == 7 && column == 7)
                {
                    //Disqualify White Short Castling (abcd AND 1101 = ab0d)
                    _canCastle &= 13;
                }
                else if (oldRow == 7 && oldColumn == 0 || takenPiece is not null && takenPiece.Rank == Rank.ROOK && row == 7 && column == 0)
                {
                    //Disqualify White Long Castling (abcd AND 1110 = abc0)
                    _canCastle &= 14;
                }
            }

            //If a move lead to pawn promotion
            if (piece.Rank == Rank.PAWN && (row == 0 || row == 7))
            {
                PawnForPromotion = piece;
            }

            //Update all move list
            UpdateMoves();

            //Generate a string move
            string move = ChessHelper.GenerateMove(new Position(oldRow, oldColumn), piece, new Position(row, column), takenPiece);

            //Change turn
            IsWhiteTurn = !IsWhiteTurn;

            return move;
        }

        public void UndoNormalMove(string move)
        {
            //Extracting move data
            Position source = new(), destination = new();
            Piece? taken = null;
            ChessHelper.ExtractMove(move, source, destination, ref taken);

            /*Undo the move*/

            //Get the moved piece from destination position and change it internal position
            Piece? piece = GetPieceAt(destination);
            piece!.Position = new Position(source.Row, source.Column);
            //Move the piece to the original position
            _board[source.Row, source.Column] = piece;
            _board[destination.Row, destination.Column] = taken;
            //Update all move list
            UpdateMoves();

            //Change the turn
            IsWhiteTurn = !IsWhiteTurn;
        }

        public void PawnPromotion(Rank newRank)
        {
            //Check if there is a pawn for promotion before promoting
            if (PawnForPromotion is not null)
            {
                //Update to new rank
                PawnForPromotion.Rank = newRank;

                //Update move list
                UpdateMove(PawnForPromotion);

                Test.Log($"From PP: {PawnForPromotion}\n");

                //Set pawn promotion to null
                PawnForPromotion = null;
            }
        }

        public void UndoPawnPromotion(Piece pawn)
        {
            pawn.Rank = Rank.PAWN;
        }

        public void LongCastling()
        {
            //abcd AND 0001 -> 000d -> d = 1 then value = 1
            //abcd AND 1000 -> a000 -> a = 1 then value = 8
            if (IsWhiteTurn ? (_canCastle & 1) == 1 : (_canCastle & 8) == 8)
            {
                Piece? rook = GetPieceAt(IsWhiteTurn ? 7 : 0, 0);
                Piece king = IsWhiteTurn ? WhiteKing : BlackKing;

                //Update the internal coordinate of the rook and king
                rook!.Position.Column = 3; //If the castling condition is correct, then rook should not be null
                king.Position.Column = 2;

                //Move the rook
                _board[IsWhiteTurn ? 7 : 0, 3] = rook;
                _board[IsWhiteTurn ? 7 : 0, 0] = null;

                //Move the king
                _board[IsWhiteTurn ? 7 : 0, 2] = king;
                _board[IsWhiteTurn ? 7 : 0, 4] = null;

                //Disqualify the ability to castle
                _canCastle &= IsWhiteTurn ? 14 : 7;

                //Change turn
                IsWhiteTurn = !IsWhiteTurn;

            }
        }

        private void UndoLongCastling()
        {
            Piece? rook = GetPieceAt(IsWhiteTurn ? 7 : 0, 3);
            Piece king = IsWhiteTurn ? WhiteKing: BlackKing;

            //Update the internal coordinate
            rook!.Position.Column = 0;
            king.Position.Column = 4;

            //Move the rook
            _board[IsWhiteTurn ? 7 : 0, 0] = rook;
            _board[IsWhiteTurn ? 7 : 0, 3] = null;

            //Move the king
            _board[IsWhiteTurn ? 7 : 0, 4] = king;
            _board[IsWhiteTurn ? 7 : 0, 2] = null;

            //Restore the ability to castle
            _canCastle |= IsWhiteTurn ? 1 : 8;
        }

        public void ShortCastling()
        {
            //abcd AND 0010 -> 00c0 -> c = 1 then value = 2
            //abcd AND 0100 -> 0b00 -> b = 1 then value = 4
            if (IsWhiteTurn ? (_canCastle & 2) == 2 : (_canCastle & 4) == 4)
            {
                Piece? rook = GetPieceAt(IsWhiteTurn ? 7 : 0, 7);
                Piece king = IsWhiteTurn ? WhiteKing : BlackKing;

                //Update the internal coordinate of the rook and king
                rook!.Position.Column = 5; //If the castling condition is correct, then rook should not be null
                king.Position.Column = 6;

                //Move the rook
                _board[IsWhiteTurn ? 7 : 0, 5] = rook;
                _board[IsWhiteTurn ? 7 : 0, 7] = null;

                //Move the king
                _board[IsWhiteTurn ? 7 : 0, 4] = null;
                _board[IsWhiteTurn ? 7 : 0, 6] = king;

                //Disqualify the ability to castle
                _canCastle &= IsWhiteTurn ? 13 : 11;

                //Change turn
                IsWhiteTurn = !IsWhiteTurn;


            }
        }

        public void UndoShortCastling()
        {
            Piece? rook = GetPieceAt(IsWhiteTurn ? 7 : 0, 5);
            Piece king = IsWhiteTurn ? WhiteKing: BlackKing;

            //Update the internal coordinate
            rook!.Position.Column = 7;
            king.Position.Column = 4;

            //Move the rook
            _board[IsWhiteTurn ? 7 : 0, 7] = rook;
            _board[IsWhiteTurn ? 7 : 0, 5] = null;

            //Move the king
            _board[IsWhiteTurn ? 7 : 0, 4] = king;
            _board[IsWhiteTurn ? 7 : 0, 6] = null;

            //Restore the ability to castle
            _canCastle |= IsWhiteTurn ? 2 : 4;

            //Change turn
            IsWhiteTurn = !IsWhiteTurn;
        }

        public void GetAllMovesPossible()
        {

        }
    }
}
