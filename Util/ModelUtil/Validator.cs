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

    class Test
    {
        public static void Log(string content)
        {
            string file = $"{AppDomain.CurrentDomain.BaseDirectory}/test.log";
            File.AppendAllText(file, content);
        }
    }
}
