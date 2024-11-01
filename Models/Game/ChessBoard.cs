using Chess.Util.ModelUtil;
using System.Security.RightsManagement;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chess.Models.Game
{
    class Move
    {
        private int _moveType; //1 is Normal move, 2 is Pawn promotion, 3 is Castling
        public int MoveType
        {
            get => _moveType;
            set
            {
                //There are only 3 types of move: normal move, pawn promotion, castling, so we used 2 bits to denote it
                if (0 < value && value <= 4)
                {
                    _moveType = value;
                }
            }
        }

        public Move(int moveType)
        {
            MoveType = moveType;
        }

        public bool IsNormalMove()
        {
            return MoveType == 1;
        }

        public bool IsPawnPromotion()
        {
            return MoveType == 2;
        }

        public bool IsCastling()
        {
            return MoveType == 3;
        }
    }

    class NormalMove : Move
    {
        public Position Source { get; set; }
        public Piece MovedPiece { get; set; }
        public Position Destination { get; set; }
        public Piece? TakenPiece { get; set; }

        public NormalMove(Position source, Piece movedPiece, Position dest, Piece? takenPiece) : base(1)
        {
            Source = source;
            MovedPiece = movedPiece;
            Destination = dest;
            TakenPiece = takenPiece;
        }
    }

    class PawnPromotion : Move
    {
        public Piece Pawn { get; set; }
        public Rank NewRank { get; set; }

        public PawnPromotion(Piece pawn, Rank newRank) : base(2)
        {
            Pawn = pawn;
            NewRank = newRank;
        }
    }

    class Castling : Move
    {
        private int _castle;
        public int Castle
        {
            get => _castle;
            set
            {
                //We used 4 bist to represent the castle side: Black Long Castling - Black Short Castling - White Short Castling - White Long Castling
                if (value == 1 || value == 2 || value == 4 && value == 8)
                {
                    _castle = value;
                }
            }
        }

        public Castling(int castle) : base(3)
        {
            Castle = castle;
        }

        public bool IsBlackLongCastling()
        {
            return Castle == 1;
        }

        public bool IsBlackShortCastling()
        {
            return Castle == 2;
        }

        public bool IsWhiteShortCastling()
        {
            return Castle == 4;
        }

        public bool IsWhiteLongCastling()
        {
            return Castle == 8;
        }
    }

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

                    foreach (var pos in moves)
                    {
                        Move(_selectedPiece, pos);
                        if (IsChecked(IsWhiteTurn ? BlackKing : WhiteKing))
                        {
                            Test.Log("Invalid move");
                            invalid.Add(pos);
                        }
                        UndoNormalMove();
                    }

                    foreach (var pos in invalid)
                    {
                        _selectedPiece.RemoveMove(pos);
                    }
                }
            }
        }

        public bool IsWhiteTurn { get; set; } = true;

        public Piece? PawnForPromotion { get; set; } = null;

        public Stack<Move> moves = [];
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
                //Clear the moves stack
                moves.Clear();

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

        public void Move(Piece piece, Position pos)
        {
            Move(piece, pos.Row, pos.Column);
        }

        public void Move(Piece piece, int row, int column)
        {
            if (piece is null)
            {
                return;
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

            //Increase the number of move for the piece
            piece.MoveMade++;

            //If a move lead to pawn promotion
            if (piece.Rank == Rank.PAWN && (row == 0 || row == 7))
            {
                PawnForPromotion = piece;
            }

            //Update all move list
            UpdateMoves();

            //Generate move info and add it to the stack
            NormalMove move = new(new Position(oldRow, oldColumn), piece, new Position(row, column), takenPiece);
            moves.Push(move);

            //Change turn
            IsWhiteTurn = !IsWhiteTurn;
        }

        //When calling any undo, it should be right after a corresponding move so that it can work correctly
        public void UndoNormalMove()
        {
            //If the stack is empty, we cannot perform undo, so do nothing here
            if (moves.Count == 0)
            {
                return;
            }

            //Check if the current move in the stack is NormalMove type before process it
            if (!moves.Peek().IsNormalMove())
            {
                return;
            }
            var move = (NormalMove)moves.Pop();

            //Get the moved piece from destination position and change it internal position
            move.MovedPiece.Position = new Position(move.Source.Row, move.Source.Column);
            //Move the piece to the original position
            _board[move.Source.Row, move.Source.Column] = move.MovedPiece;
            _board[move.Destination.Row, move.Destination.Column] = move.TakenPiece;

            //Restore castle ability by decrease the numbe of move made
            move.MovedPiece.MoveMade--;

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

                //Set pawn promotion to null
                PawnForPromotion = null;
            }
        }

        public void UndoPawnPromotion()
        {
            //If the moves stack is empty, then we do nothing here
            if (moves.Count == 0)
            {
                return;
            }

            //Check if the current move is Pawn Promotion
            if (!moves.Peek().IsPawnPromotion())
            {
                return;
            }
            var pp = (PawnPromotion)moves.Pop();

            //Undo pawn promotion
            pp.Pawn.Rank = Rank.PAWN;
        }

        public bool CanWhiteLongCastling()
        {
            //First, we get the Piece at position (7, 0) -> if it not a White rook, then it cannot castling
            Piece? rk = GetPieceAt(7, 0);
            if (rk is null || rk.Rank != Rank.ROOK || rk.Color == Color.BLACK)
            {
                return false;
            }

            //If the King or the Rook has moved, then it cannot be castling anymore
            if (WhiteKing.MoveMade != 0 || rk.MoveMade != 0)
            {
                return false;
            }

            //Check if the 3 squares (7, 1) - (7, 2) - (7, 3) are currently occupied
            if (GetPieceAt(7, 1) is not null || GetPieceAt(7, 2) is not null || GetPieceAt(7, 3) is not null)
            {
                return false;
            }

            //Check if the King is currently being check
            if (IsChecked(WhiteKing))
            {
                return false;
            }

            //Check if the 2 squares (7, 2) and (7, 3) is under attack of the opponent
            Position p1 = new(7, 2), p2 = new(7, 3);
            Piece? piece;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    piece = GetPieceAt(row, col);   
                    if (piece is not null && piece.Color == Color.BLACK && (piece.HasMove(p1) || piece.HasMove(p2)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanBlackLongCastling()
        {
            //First, we get the Piece at position (0, 0) -> if it not a Black rook, then it cannot castling
            Piece? rk = GetPieceAt(0, 0);
            if (rk is null || rk.Rank != Rank.ROOK || rk.Color == Color.WHITE)
            {
                return false;
            }

            //If the King or the Rook has moved, then it cannot be castling anymore
            if (BlackKing.MoveMade != 0 || rk.MoveMade != 0)
            {
                return false;
            }

            //Check if the 3 squares (0, 1) - (0, 2) - (0, 3) are currently occupied
            if (GetPieceAt(0, 1) is not null || GetPieceAt(0, 2) is not null || GetPieceAt(0, 3) is not null)
            {
                return false;
            }

            //Check if the King is currently being check
            if (IsChecked(BlackKing))
            {
                return false;
            }

            //Check if the 2 squares (0, 2) and (0, 3) is under attack of the opponent
            Position p1 = new(0, 2), p2 = new(0, 3);
            Piece? piece;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    piece = GetPieceAt(row, col);
                    if (piece is not null && piece.Color == Color.WHITE && (piece.HasMove(p1) || piece.HasMove(p2)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void LongCastling()
        {
            if (IsWhiteTurn ? CanWhiteLongCastling() : CanBlackLongCastling())
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

                //Change turn
                IsWhiteTurn = !IsWhiteTurn;

                //Add the move to moves stack
                moves.Push(new Castling(IsWhiteTurn ? 1 : 8));
            }
        }

        private void UndoLongCastling()
        {
            //Check if the stack is empty
            if (moves.Count == 0)
            {
                return;
            }

            //Check if the top of the stack is not a castling
            if (!moves.Peek().IsCastling())
            {
                return;
            }
            var cs = (Castling)moves.Pop();

            //Check if this is a short castling 
            if (cs.IsWhiteShortCastling() || cs.IsBlackShortCastling())
            {
                return;
            }

            //Every time we do/undo any move, the turn changed accordingly, so we can just use IsWhiteTurn to decide the logic 
            Piece? rook = GetPieceAt(IsWhiteTurn ? 7 : 0, 3);
            Piece king = IsWhiteTurn ? WhiteKing : BlackKing;

            //Update the internal coordinate
            rook!.Position.Column = 0;
            king.Position.Column = 4;

            //Move the rook
            _board[IsWhiteTurn ? 7 : 0, 0] = rook;
            _board[IsWhiteTurn ? 7 : 0, 3] = null;

            //Move the king
            _board[IsWhiteTurn ? 7 : 0, 4] = king;
            _board[IsWhiteTurn ? 7 : 0, 2] = null;
        }

        public bool CanWhiteShortCastling()
        {
            //First, we get the Piece at position (7, 7) -> if it not a White rook, then it cannot castling
            Piece? rk = GetPieceAt(7, 7);
            if (rk is null || rk.Rank != Rank.ROOK || rk.Color == Color.BLACK)
            {
                return false;
            }

            //If the King or the Rook has moved, then it cannot be castling anymore
            if (WhiteKing.MoveMade != 0 || rk.MoveMade != 0)
            {
                return false;
            }

            //Check if the 2 squares (7, 6) - (7, 5) are currently occupied
            if (GetPieceAt(7, 6) is not null || GetPieceAt(7, 5) is not null)
            {
                return false;
            }

            //Check if the King is currently being check
            if (IsChecked(WhiteKing))
            {
                return false;
            }

            //Check if the 2 squares (7, 6) and (7, 5) is under attack of the opponent
            Position p1 = new(7, 6), p2 = new(7, 5);
            Piece? piece;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    piece = GetPieceAt(row, col);
                    if (piece is not null && piece.Color == Color.BLACK && (piece.HasMove(p1) || piece.HasMove(p2)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanBlackShortCastling()
        {
            //First, we get the Piece at position (0, 7) -> if it not a Black rook, then it cannot castling
            Piece? rk = GetPieceAt(0, 7);
            if (rk is null || rk.Rank != Rank.ROOK || rk.Color == Color.WHITE)
            {
                return false;
            }

            //If the King or the Rook has moved, then it cannot be castling anymore
            if (BlackKing.MoveMade != 0 || rk.MoveMade != 0)
            {
                return false;
            }

            //Check if the 2 squares (0, 5) - (0, 6) are currently occupied
            if (GetPieceAt(0, 5) is not null || GetPieceAt(0, 6) is not null)
            {
                return false;
            }

            //Check if the King is currently being check
            if (IsChecked(BlackKing))
            {
                return false;
            }

            //Check if the 2 squares (0, 5) and (0, 6) is under attack of the opponent
            Position p1 = new(0, 5), p2 = new(0, 6);
            Piece? piece;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    piece = GetPieceAt(row, col);
                    if (piece is not null && piece.Color == Color.WHITE && (piece.HasMove(p1) || piece.HasMove(p2)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void ShortCastling()
        {
            if (IsWhiteTurn ? CanWhiteShortCastling() : CanBlackShortCastling())
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

                //Change turn
                IsWhiteTurn = !IsWhiteTurn;

                //Add the move to moves stack
                moves.Push(new Castling(IsWhiteTurn ? 2 : 4));
            }
        }

        public void UndoShortCastling()
        {
            //Check if the stack is empty
            if (moves.Count == 0)
            {
                return;
            }

            //Check if the top of the stack is not a castling
            if (!moves.Peek().IsCastling())
            {
                return;
            }
            var cs = (Castling)moves.Pop();

            //Check if this is a long castling 
            if (cs.IsWhiteLongCastling() || cs.IsBlackLongCastling())
            {
                return;
            }

            Piece? rook = GetPieceAt(IsWhiteTurn ? 7 : 0, 5);
            Piece king = IsWhiteTurn ? WhiteKing : BlackKing;

            //Update the internal coordinate
            rook!.Position.Column = 7;
            king.Position.Column = 4;

            //Move the rook
            _board[IsWhiteTurn ? 7 : 0, 7] = rook;
            _board[IsWhiteTurn ? 7 : 0, 5] = null;

            //Move the king
            _board[IsWhiteTurn ? 7 : 0, 4] = king;
            _board[IsWhiteTurn ? 7 : 0, 6] = null;

            //Change turn
            IsWhiteTurn = !IsWhiteTurn;
        }

        public void GetAllMovesPossible()
        {

        }
    }
}
