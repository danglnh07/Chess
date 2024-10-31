using Chess.Models.Game;
using System.IO;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;

namespace Chess.Util.ModelUtil
{
    /// <summary>
    /// Helper class for validate Position in Chess game.
    /// </summary>
    class Validator
    {
        /// <summary>
        /// Overloading method: used to check if a value is valid to be a Position attribute (either a row or column).
        /// </summary>
        /// <param name="position">The value to be checked</param>
        /// <returns>Boolean value, true if position is valid (an integer between 0 and 7), false otherwise</returns>
        public static bool IsPositionValid(int position)
        {
            return 0 <= position && position <= 7;
        }

        /// <summary>
        /// Overloading method: used to check if a pair of values is valid to be Position attribute (both row and column)
        /// </summary>
        /// <param name="row">The row value to be checked</param>
        /// <param name="col">The column value to be checked</param>
        /// <returns>Boolean value, true if both row and column is valid (integers between 0 and 7), false otherwise</returns>
        public static bool IsPositionValid(int row, int col)
        {
            return IsPositionValid(row) && IsPositionValid(col);
        }

        /// <summary>
        /// Overloading method: used to check if a Position object is valid
        /// </summary>
        /// <param name="position">The Position object to be checked</param>
        /// <returns>
        /// Boolean value, true if Position is valid (both its row and column value are integers between 0 and 7), false otherwise
        /// </returns>
        public static bool IsPositionValid(Position position)
        {
            return IsPositionValid(position.Row) && IsPositionValid(position.Column);
        }
    }

    class ChessHelper
    {
        /*
         * The format for a string move will have 3 cases: A normal move, castling and pawn promotion. Every move will start with a code
         * to identify which case is it: MV (normal MoVe), CS (CaStling), PP (Pawn Promotion)
         * 1. A normal move will have 5 parts: code - selected piece - old position - taken piece - new position
         * + The piece (both selected and taken) will have the form: Color Rank. color will be uppercase W (White) or B (Black). 
         * The Rank would be: Pn (Pawn), Rk (Rook), Kn (Knight), Bs (Bishop), Qu (Queen), Kg (King). So, a piece may look like: 
         * BPn (Black Pawn)
         * + The position would be (row, col)
         * 2. Pawn promotion will have the form: code - pawn - pawn position - new rank. The pawn and rank will also use the same code as normal 
         * move
         * 3. Castling will have the form: code - color - side (S for Short and L for Long). So a Black Short Castling is: CS - B - S
         */

        private static readonly Dictionary<string, Rank> StringToRank = new()
        {
            { "Pn", Rank.PAWN },
            { "Rk", Rank.ROOK },
            { "Kn", Rank.KNIGHT },
            { "Bs", Rank.BISHOP },
            { "Qu", Rank.QUEEN },
            { "Kg", Rank.KING },
        };

        private static readonly Dictionary<Rank, string> RankToString = new()
        {
            { Rank.PAWN, "Pn" },
            { Rank.ROOK, "Rk" },
            { Rank.KNIGHT, "Kn" },
            { Rank.BISHOP, "Bs" },
            { Rank.QUEEN, "Qu" },
            { Rank.KING, "Kg" }
        };


        public static void ExtractMove(string move, Position source, Position destination, ref Piece? taken)
        {
            //Extract data from from a normal move
            var movesData = move.Split(" - ");

            //Get selected piece position (old position). Format: (row, col)
            source.Row = movesData[2][1] - '0';
            source.Column = movesData[2][4] - '0';

            //Get the destination position
            destination.Row = movesData[4][1] - '0';
            destination.Column =  movesData[4][4] - '0';

            //Get taken piece (If none, then the string will be literally none)
            if (movesData[3].ToLower().Equals("none"))
            {
                taken = null;
            }
            else
            {
                //Get taken piece's color
                Color c = movesData[3][0].Equals('B') ? Color.BLACK : Color.WHITE;
                //Get taken piece's rank
                Rank r = StringToRank[movesData[3][1..]];
                taken = new Piece(destination, c, r);
            }
        }

        public static void ExtractMove(string move, ref int castle)
        {
            castle = 15; //We set the default to all true, then use AND to set the value based on move
            var movesData = move.Split(" - ");

            castle &= movesData[1].Equals("B") ? 12 : 3;
            castle &= movesData[2].Equals("S") ? 6 : 9;

        }

        public static void ExtractMove(string move, Piece pawn, ref Rank newRank)
        {
            var movesData = move.Split(" - ");

            //Get Pawn data
            pawn.Color = movesData[1][0].Equals('B') ? Color.BLACK : Color.WHITE;
            pawn.Rank = Rank.PAWN;
            pawn.Position = new Position(movesData[2][1] - '0', movesData[2][4] - '0');

            //Get new Rank
            newRank = StringToRank[movesData[3]];
        }

        public static string GenerateMove(Position source, Piece selected, Position destination, Piece? taken)
        {
            return $"MV - {(selected.Color == Color.WHITE ? "W" : "B")}{RankToString[selected.Rank]} - {source} - " +
                $"{(taken is null ? "none" : $"{(taken.Color == Color.WHITE ? "W" : "B")}{RankToString[taken.Rank]}")} - {destination}";
        }

        public static string GenerateMove(int castle)
        {
            if (castle != 1 && castle != 2 && castle != 4 && castle != 8) 
            {
                throw new ArgumentException("Invalid Castle description. It should only be 1, 2, 4 or 8");
            }

            if (castle == 8)
            {
                return "CS - B - L";
            }

            if (castle == 4)
            {
                return "CS - B - S";
            }

            if (castle == 2)
            {
                return "CS - W - S";
            }

            return "CS - W - L";
        }

        public static string GenerateMove(Piece pawn, Rank newRank)
        {
            return $"PP - {(pawn.Color == Color.BLACK ? "B" : "W")}{RankToString[pawn.Rank]} - {pawn.Position} - {RankToString[newRank]}";
        }
    }

    class Test
    {
        public static void Log(string content)
        {
            string file = $"{AppDomain.CurrentDomain.BaseDirectory}/test.txt";
            File.AppendAllText(file, content);
        }
    }
}
