using Chess.Util.ModelUtil;

namespace Chess.Models.Game
{
    /// <summary>
    /// Position class, used to demote the position of a square/piece in the chessboard
    /// </summary>
    class Position
    {
        #region Fields
        /// <summary>
        /// Backing value: The row attribute of Position
        /// </summary>
        private int _row;
        /// <summary>
        /// Gets or sets the value of row
        /// </summary>
        public int Row
        {
            get { return _row; }
            set
            {
                //Check if row is invalid
                if (!Validator.IsPositionValid(value))
                {
                    throw new ArgumentException("Row must be an integer between 0 and 7");
                }
                //Update new value of row
                _row = value;
            }
        }

        /// <summary>
        /// Backing value: The column attribute of Position
        /// </summary>
        private int _column;
        /// <summary>
        /// Gets or sets the value of column
        /// </summary>
        public int Column
        {
            get { return _column; }
            set
            {
                //Check if column is invalid
                if (!Validator.IsPositionValid(value))
                {
                    throw new ArgumentException("Column must be an integer between 0 and 7");
                }
                //Set the new value of column
                _column = value;
            }
        }
        #endregion //End Fields

        /// <summary>
        /// Default constuctor, which will set row and column to 0 
        /// </summary>
        public Position()
        {
            Row = 0;
            Column = 0; 
        }

        /// <summary>
        /// Constructor of class Position
        /// </summary>
        /// <param name="row">The value of row</param>
        /// <param name="col">The value of column</param>
        public Position(int row, int col)
        {
            Row = row;
            Column = col;
        }

        /// <summary>
        /// Overridden method: used to perform a deep comparison on Position object
        /// </summary>
        /// <param name="obj">The nullable object to be compared</param>
        /// <returns>Boolean value, true if this object and obj is deeply equals, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            //C# syntax: is -> check if obj is the same type as Position (if not equals or obj is null -> false)
            //Position pos -> type casting
            //obj is Position pos -> combine the type checking and type casting into the same line
            if (obj is Position pos)
            {
                return pos.Row == _row && pos.Column == _column;
            }

            return false;
        }

        /// <summary>
        /// Overridden method: used to get the hash code of this object. This method used the base HashCode combine with value
        /// of row and column
        /// </summary>
        /// <returns>The integer value of this object hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        /// <summary>
        /// Overridden method: used to get the information of this Position object in a formatted string. The format is (row,col).
        /// For example: (5,7)
        /// </summary>
        /// <returns>The formatted string of Position object</returns>
        public override string ToString()
        {
            return string.Format($"({_row}, {_column})");
        }
    }

    /// <summary>
    /// The Rank of the Piece. Currently has 6 value: PAWN, ROOK, KNIGHT, BISHOP, QUEEN, KING
    /// </summary>
    enum Rank
    {
        PAWN, ROOK, KNIGHT, BISHOP, QUEEN, KING
    }

    /// <summary>
    /// The Color of the Piece. Currently has 2 value: WHITE, BLACK
    /// </summary>
    enum Color
    {
        WHITE, BLACK
    }

    /// <summary>
    /// The Piece class, the Piece in the chess game
    /// </summary>
    class Piece
    {
        #region Fields
        /// <summary>
        /// Backing field: The current position of this piece on the chessboard
        /// </summary>
        private Position _position = default!; //Setting the default! is to tell the compiler that the initialization will be done later 
        /// <summary>
        /// Gets or sets the value of Piece's Position
        /// </summary>
        public Position Position
        {
            get { return _position; }
            set
            {
                //Check if position is invalid
                if (!Validator.IsPositionValid(value))
                {
                    throw new ArgumentException("Invalid position! Row and column must be integers between 0 and 7");
                }
                //Set new value for position
                _position = value;
            }
        }

        /// <summary>
        /// Gets the value of Piece's Color. Note that, this value denote the side of the piece, not the actual color in the UI. 
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the value of Piece's Rank. 
        /// </summary>
        public Rank Rank { get; set; }

        /// <summary>
        /// Backing field: the List of Position which contains all the Position this Piece can move to
        /// </summary>
        private readonly List<Position> _canMove;
        /// <summary>
        /// Gets the value of _canMove list
        /// </summary>
        public List<Position> CanMove { get { return _canMove; } }

        /// <summary>
        /// Backing field: the number of moves this current Piece has made throughout the game. Most ly useful for determine castling ability
        /// </summary>
        private int _moveMade = 0;
        /// <summary>
        /// Gets or sets the number of moves this Piece has made
        /// </summary>
        public int MoveMade
        {
            get => _moveMade;
            set
            {
                if (value >= 0)
                {
                    _moveMade = value;
                }
            }
        }
        #endregion //End Fields

        /// <summary>
        /// Default constructor, which will set it to the Black Rook at (0, 0)
        /// </summary>
        public Piece()
        {
            Position = new Position();
            Color = Color.BLACK;
            Rank = Rank.ROOK;
            _canMove = [];
        }

        /// <summary>
        /// Overloading constructor method of class Piece
        /// </summary>
        /// <param name="position">The current Position value of this Piece</param>
        /// <param name="color">The Color value of this Piece</param>
        /// <param name="rank">The Rank value of this Piece</param>
        public Piece(Position position, Color color, Rank rank)
        {
            //Assign new value for position, color and rank, also initialize empty CanMove list
            Position = position; //Since this is not declare as nullable, we don't have to check for null here
            Color = color;
            Rank = rank;
            _canMove = []; //The same as using new List<Position>();
        }

        /// <summary>
        /// Overloading constructor method of class Piece
        /// </summary>
        /// <param name="row">The row where this Piece is located</param>
        /// <param name="column">The column where this Piece is located</param>
        /// <param name="color">The Color of the Piece</param>
        /// <param name="rank">The Rank of the Piece</param>
        public Piece(int row, int column, Color color, Rank rank)
        {
            //Assign new value for position, color and rank, also initialize empty CanMove list
            Position = new Position(row, column); //The Position constructor will do the validation for row and column values
            Color = color;
            Rank = rank;
            _canMove = []; //The same as using new List<Position>();
        }

        /// <summary>
        /// Overloading method: check if this Piece's canMove list contains a specific Position. 
        /// Remember that we override the Equals method, so this is a deep search
        /// </summary>
        /// <param name="pos">The Position object to be checked</param>
        /// <returns>Boolean value, true if canMove list contains this Position, false otherwise</returns>
        public bool HasMove(Position pos)
        {
            return _canMove.Contains(pos);
        }

        /// <summary>
        /// Overloading method: check if this Piece's canMove list contains a specific Position. 
        /// Remember that we override the Equals method, so this is a deep search
        /// </summary>
        /// <param name="row">The row attribute of Position for checking</param>
        /// <param name="col">The col attribure of Position for checking</param>
        /// <returns>Boolean value, true if canMove list contains this Position, false otherwise</returns>
        public bool HasMove(int row, int col)
        {
            return HasMove(new Position(row, col)); 
        }


        /// <summary>
        /// Method for adding new Position to canMove list
        /// </summary>
        /// <param name="pos">The new position to be added</param>
        public void AddNewMove(Position pos)
        {
            _canMove.Add(pos);
        }

        public void RemoveMove(int index)
        {
            _canMove.RemoveAt(index);
        }

        /// <summary>
        /// Method for removing the a specific Position in canMove list. If the Position originally not in the list, 
        /// this method simply do nothing
        /// </summary>
        /// <param name="pos">The Position to be removed</param>
        public void RemoveMove(Position pos)
        {
            _canMove.Remove(pos);
        }

        /// <summary>
        /// Method for clearing the entire canMove list (set it to empty)
        /// </summary>
        public void ClearAllMoves()
        {
            _canMove.Clear();
        }

        /// <summary>
        /// Overridden method: used to perform a deep comparison on Piece object
        /// </summary>
        /// <param name="obj">The nullable object to be compared</param>
        /// <returns>Boolean value, true if this object and obj is deeply equals, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Piece piece)
            {
                return piece.Position.Equals(_position) && piece.Color == Color && piece.Rank == Rank;
            }
            return false;
        }

        /// <summary>
        /// Overridden method: used to get the hash code of this object. This method used the base HashCode combine with value of 
        /// Position, Color and Rank
        /// </summary>
        /// <returns>The integer value of this object hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Color, Rank);
        }

        /// <summary>
        /// Overridden method: used to get the information of this Position object in a formatted string. 
        /// The format is Color Rank at (row, col). For example: BLACK PAWN at (5,5)
        /// </summary>
        /// <returns>The formatted string of Piece object</returns>
        public override string ToString()
        {
            return string.Format($"{Color} {Rank} at {Position}");
        }
    }

    partial class ChessBoard
    {
        private void CalculatePawnMove(Piece pawn)
        {
            /*
             * Algorithm explains: There are 3 basic rules for the pawn movement
             * 1. It can only move forward one square (if that square is not blocked by either our side or opponent side)
             * 2. If the pawn is at the initial position, it can move 1 square (first rule) or 2 square forward (doubling). The condition for
             * douling is that, either two square ahead didn't be blocked by our side or opponent
             * 3. A pawn can taken opponent diagonally one square ahead
             * So, first we calculate the forward vector of the pawn based on its Color (1 if it's Black, -1 if it's White)
             * Then we checkd for the 3 rules, if whichever rule meet the condition, we add the Position to the canMove list
             * Again, this is not accounting for checked state checking. This also not cover the pawn promotion (which will be handled later),
             * en passant (not implemented yet)
             */

            //Clear the candidates list
            pawn.ClearAllMoves();

            //Get the initial coordinates and Color of pawn
            int row = pawn.Position.Row, col = pawn.Position.Column;
            Color color = pawn.Color;

            //The direction unit vector
            int forward = (color == Color.BLACK) ? 1 : -1;

            //Temporary piece object (since there is a change that the square has no piece located at -> can be null -> nullable)
            Piece? p;

            try
            {
                //Check one square forward
                if (Validator.IsPositionValid(row + forward, col) && GetPieceAt(row + forward, col) is null)
                {
                    pawn.AddNewMove(new Position(row + forward, col));

                    //Check two squares forward if it's the pawn's first move (row 1 if BLACK or row 6 if WHITE)
                    if ((pawn.Color == Color.BLACK && row == 1) || (pawn.Color == Color.WHITE && row == 6))
                    {
                        if (Validator.IsPositionValid(row + 2 * forward, col) && GetPieceAt(row + 2 * forward, col) is null)
                        {
                            pawn.AddNewMove(new Position(row + 2 * forward, col));
                        }
                    }
                }

                //Check diagonal captures
                Color oppositeColor = (pawn.Color == Color.BLACK) ? Color.WHITE : Color.BLACK;

                //Check right diagonal
                if (Validator.IsPositionValid(row + forward, col + 1))
                {
                    p = GetPieceAt(row + forward, col + 1);
                    if (p is not null && p.Color == oppositeColor)
                    {
                        pawn.AddNewMove(new Position(row + forward, col + 1));
                    }
                }

                //Check left diagonal
                if (Validator.IsPositionValid(row + forward, col - 1))
                {
                    p = GetPieceAt(row + forward, col - 1);
                    if (p is not null && p.Color == oppositeColor)
                    {
                        pawn.AddNewMove(new Position(row + forward, col - 1));
                    }
                }
            }
            catch (ArgumentException ex)
            {
                //Will have a proper logging and testing later
                Test.Log($"Error at: CalculatePawnMove -> Error message: {ex}\nStack trace: {ex.StackTrace}");
            }
        }

        private void CalculateRookMove(Piece rook)
        {
            /*
             * Algorithm explains: Rook has simple rule for moving: it can only move horizontally or vertically both ways. It cannot move
             * further in a direction if it reach the edge of the board, of it meets an obstacle
             * So, we calculate the 4 vectors for each direction, moving along each direction, if the current square can still be move to, 
             * add them to the list
             */

            //Clear the moves list
            rook.ClearAllMoves();

            //The direction 2D array, respectively: N, E, S, W {row, col}
            int[,] dir = { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

            //The coordinate of the Rook and its color
            int row = rook.Position.Row;
            int col = rook.Position.Column;
            Color c = rook.Color;

            //Temporary piece object (since there is a change that the square has no piece located at -> can be null -> nullable)
            Piece? p;

            try
            {
                //We handle each directionin this order: N -> E -> S -> W
                for (int i = 0; i < 4; i++)
                {
                    while (Validator.IsPositionValid(row + dir[i, 0], col + dir[i, 1]) && GetPieceAt(row + dir[i, 0], col + dir[i, 1]) is null)
                    {
                        //Update startPoint
                        row += dir[i, 0];
                        col += dir[i, 1];
                        //Add to moves list
                        rook.AddNewMove(new Position(row, col));
                    }

                    /*
                     * If the while loop is break out, then there are two cases: 
                     * 1. The NEXT square is outside the board (we don't care about this case)
                     * 2. The NEXT square has an obsatcle (we're interested about this case)
                     * Whichever the case is, we have to move forward 1 square before checking
                     */
                    row += dir[i, 0];
                    col += dir[i, 1];

                    //Check if the obstacle is our side or opponent side
                    if (Validator.IsPositionValid(row, col) && (p = GetPieceAt(row, col)) is not null && p.Color != c)
                    {
                        rook.AddNewMove(new Position(row, col));
                    }

                    //Reset the x and y coordinate to the rook position
                    row = rook.Position.Row;
                    col = rook.Position.Column;
                }
            }
            catch (ArgumentException ex)
            {
                Test.Log($"Error at: CalculateRookMove -> Error message: {ex}\nStack trace: {ex.StackTrace}");
            }
        }

        private void CalculateKnightMove(Piece knight)
        {
            /*
             * Algorithm explain: Since the Knight can only move to 8 squares at most, we just have to check each square
             */

            //Clear the moves list
            knight.ClearAllMoves();

            //Get the intial position of knight and its color
            int row = knight.Position.Row;
            int col = knight.Position.Column;
            Color c = knight.Color;

            //Temporary piece object (since there is a change that the square has no piece located at -> can be null -> nullable)
            Piece? p;

            try
            {
                //(row - 1, col - 2)
                if (Validator.IsPositionValid(row - 1, col - 2) && ((p = GetPieceAt(row - 1, col - 2)) is null || p.Color != c))
                {
                    knight.AddNewMove(new Position(row - 1, col - 2));
                }

                //(row - 2, col - 1)
                if (Validator.IsPositionValid(row - 2, col - 1) && ((p = GetPieceAt(row - 2, col - 1)) is null || p.Color != c))
                {
                    knight.AddNewMove(new Position(row - 2, col - 1));
                }

                //(row - 2, col + 1)
                if (Validator.IsPositionValid(row - 2, col + 1) && ((p = GetPieceAt(row - 2, col + 1)) is null || p.Color != c))
                {
                    knight.AddNewMove(new Position(row - 2, col + 1));
                }

                //(row - 1, col + 2)
                if (Validator.IsPositionValid(row - 1, col + 2) && ((p = GetPieceAt(row - 1, col + 2)) is null || p.Color != c))
                {
                    knight.AddNewMove(new Position(row - 1, col + 2));
                }

                //(row + 1, col + 2)
                if (Validator.IsPositionValid(row + 1, col + 2) && ((p = GetPieceAt(row + 1, col + 2)) is null || p.Color != c))
                {
                    knight.AddNewMove(new Position(row + 1, col + 2));
                }

                //(row + 2, col + 1)
                if (Validator.IsPositionValid(row + 2, col + 1) && ((p = GetPieceAt(row + 2, col + 1)) is null || p.Color != c))
                {
                    knight.AddNewMove(new Position(row + 2, col + 1));
                }

                //(row + 2, col - 1)
                if (Validator.IsPositionValid(row + 2, col - 1) && ((p = GetPieceAt(row + 2, col - 1)) is null || p.Color != c))
                {
                    knight.AddNewMove(new Position(row + 2, col - 1));
                }

                //(row + 1, col - 2)
                if (Validator.IsPositionValid(row + 1, col - 2) && ((p = GetPieceAt(row + 1, col - 2)) is null || p.Color != c))
                {
                    knight.AddNewMove(new Position(row + 1, col - 2));
                }
            }
            catch (ArgumentException ex)
            {
                Test.Log($"Error at: CalculateKnightMove -> Error message: {ex}\nStack trace: {ex.StackTrace}");
            }
        }

        private void CalculateBishopMove(Piece bishop)
        {
            /*
             * Algorithm explains: Just like Rook, Bishop also move freely until it meets obstacle or the edge of the board, but diagonally
             * So, we calculate the 4 directional vectors, then moving along each direction, if the square can be moved to, add it to list
             */

            //Clear the moves list
            bishop.ClearAllMoves();

            //The direction 2D array, respectively: NW, NE, SE, SW {row, col}
            int[,] dir = { { -1, -1 }, { -1, 1 }, { 1, 1 }, { 1, -1 } };

            //The coordinate of the Bishop and its color
            int row = bishop.Position.Row;
            int col = bishop.Position.Column;
            Color c = bishop.Color;

            //Temporary piece object (since there is a change that the square has no piece located at -> can be null -> nullable)
            Piece? p;

            try
            {
                //We handle each directionin this order: NW -> NE-> SE -> SW
                for (int i = 0; i < 4; i++)
                {
                    while (Validator.IsPositionValid(row + dir[i, 0], col + dir[i, 1]) && GetPieceAt(row + dir[i, 0], col + dir[i, 1]) is null)
                    {
                        //Update startPoint
                        row += dir[i, 0];
                        col += dir[i, 1];
                        //Add to move
                        bishop.AddNewMove(new Position(row, col));
                    }

                    /*
                     * If the while loop is break out, then there are two cases: 
                     * 1. The NEXT square is outside the board (we don't care about this case)
                     * 2. The NEXT square has an obsatcle (we're interested about this case)
                     * Whichever the case is, we have to move forward 1 square before checking
                     */
                    //Move to the next square
                    row += dir[i, 0];
                    col += dir[i, 1];

                    //Check if the obstacle is our side or opponent side
                    if (Validator.IsPositionValid(row, col) && (p = GetPieceAt(row, col)) is not null && p.Color != c)
                    {
                        bishop.AddNewMove(new Position(row, col));
                    }

                    //Reset the x and y to Bishop position
                    row = bishop.Position.Row;
                    col = bishop.Position.Column;
                }
            }
            catch (ArgumentException ex)
            {
                Test.Log($"Error at: CalculateBishopMove -> Error message: {ex}\nStack trace: {ex.StackTrace}");
            }
        }

        private void CalculateQueenMove(Piece queen)
        {
            /*
             * Algorithm explains: The Queen is just a combination of Rook and Bishop, so we do the same
             */

            //Clear the moves list
            queen.ClearAllMoves();

            //The direction 2D array, respectively: NW, NE, SE, SW, N, E, S, W {rol, col}
            int[,] dir = { { -1, -1 }, { -1, 1 }, { 1, 1 }, { 1, -1 }, { -1, 0 }, { 0, 1 }, { 1, 0 }, { 0, -1 } };

            //The coordinate of the Queen and its color
            int row = queen.Position.Row;
            int col = queen.Position.Column;
            Color c = queen.Color;

            //Temporary piece object (since there is a change that the square has no piece located at -> can be null -> nullable)
            Piece? p;

            try
            {
                //We handle each directionin this order: NW -> NE-> SE -> SW -> N -> E -> S -> W
                for (int i = 0; i < 8; i++)
                {
                    while (Validator.IsPositionValid(row + dir[i, 0], col + dir[i, 1]) && GetPieceAt(row + dir[i, 0], col + dir[i, 1]) is null)
                    {
                        //Update startPoint
                        row += dir[i, 0];
                        col += dir[i, 1];
                        //Add to move
                        queen.AddNewMove(new Position(row, col));
                    }

                    /*
                     * If the while loop is break out, then there are two cases: 
                     * 1. The NEXT square is outside the board (we don't care about this case)
                     * 2. The NEXT square has an obsatcle (we're interested about this case)
                     * Whichever the case is, we have to move forward 1 square before checking
                     */
                    row += dir[i, 0];
                    col += dir[i, 1];

                    //Check if the obstacle is our side or opponent side
                    if (Validator.IsPositionValid(row, col) && (p = GetPieceAt(row, col)) is not null && p.Color != c)
                    {
                        queen.AddNewMove(new Position(row, col));
                    }

                    //Reset the x and y coordinate to queen position
                    row = queen.Position.Row;
                    col = queen.Position.Column;
                }
            }
            catch (ArgumentException ex)
            {
                Test.Log($"Error at: CalculateQueenMove -> Error message: {ex}\nStack trace: {ex.StackTrace}");
            }
        }

        private void CalculateKingMove(Piece king)
        {
            /*
             * Algorithm explains: basically, the King is the same as Knight, just different corrdinate for comparison, so we also the same
             */

            //Clear the moves list
            king.ClearAllMoves();

            //Get the intial position of King and its color
            int row = king.Position.Row;
            int col = king.Position.Column;
            Color c = king.Color;

            //Temporary piece object (since there is a change that the square has no piece located at -> can be null -> nullable)
            Piece? p;

            try
            {
                //(row - 1, col - 1)
                if (Validator.IsPositionValid(row - 1, col - 1) && ((p = GetPieceAt(row - 1, col - 1)) is null || p.Color != c))
                {
                    king.AddNewMove(new Position(row - 1, col - 1));
                }

                //(row - 1, col)
                if (Validator.IsPositionValid(row - 1, col) && ((p = GetPieceAt(row - 1, col)) is null || p.Color != c))
                {
                    king.AddNewMove(new Position(row - 1, col));
                }

                //(row - 1, col + 1)
                if (Validator.IsPositionValid(row - 1, col + 1) && ((p = GetPieceAt(row - 1, col + 1)) is null || p.Color != c))
                {
                    king.AddNewMove(new Position(row - 1, col + 1));
                }

                //(row, col + 1)
                if (Validator.IsPositionValid(row, col + 1) && ((p = GetPieceAt(row, col + 1)) is null || p.Color != c))
                {
                    king.AddNewMove(new Position(row, col + 1));
                }

                //(row + 1, col + 1)
                if (Validator.IsPositionValid(row + 1, col + 1) && ((p = GetPieceAt(row + 1, col + 1)) is null || p.Color != c))
                {
                    king.AddNewMove(new Position(row + 1, col + 1));
                }

                //(row + 1, col)
                if (Validator.IsPositionValid(row + 1, col) && ((p = GetPieceAt(row + 1, col)) is null || p.Color != c))
                {
                    king.AddNewMove(new Position(row + 1, col));
                }

                //(row + 1, col - 1)
                if (Validator.IsPositionValid(row + 1, col - 1) && ((p = GetPieceAt(row + 1, col - 1)) is null || p.Color != c))
                {
                    king.AddNewMove(new Position(row + 1, col - 1));
                }

                //(row, col - 1)
                if (Validator.IsPositionValid(row, col - 1) && ((p = GetPieceAt(row, col - 1)) is null || p.Color != c))
                {
                    king.AddNewMove(new Position(row, col - 1));
                }
            }
            catch (ArgumentException ex)
            {
                Test.Log($"Error at: CalculateQueenMove -> Error message: {ex}\nStack trace: {ex.StackTrace}");
            }
        }

        private void UpdateMove(Piece piece)
        {
            switch (piece.Rank)
            {
                case Rank.PAWN:
                    CalculatePawnMove(piece);
                    break;
                case Rank.ROOK:
                    CalculateRookMove(piece);
                    break;
                case Rank.KNIGHT:
                    CalculateKnightMove(piece);
                    break;
                case Rank.BISHOP:
                    CalculateBishopMove(piece);
                    break;
                case Rank.QUEEN:
                    CalculateQueenMove(piece);
                    break;
                case Rank.KING:
                    CalculateKingMove(piece);
                    break;
            }
        }

        public void UpdateMoves()
        {
            Piece? piece;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    piece = GetPieceAt(row, col);
                    if (piece is not null)
                    {
                        UpdateMove(piece);
                    }
                }
            }
        }
    }
}
