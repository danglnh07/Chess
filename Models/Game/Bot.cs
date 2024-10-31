using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models.Game
{
    class Bot
    {
        //Visit https://www.chessprogramming.org/Simplified_Evaluation_Function for implementation details
        private readonly Dictionary<Rank, int[,]> _positionalScore = new()
        {
            {
                Rank.PAWN, new int[,] {
                    { 0,  0,  0,  0,  0,  0,  0,  0 },
                    { 50, 50, 50, 50, 50, 50, 50, 50 },
                    { 10, 10, 20, 30, 30, 20, 10, 10 },
                    { 5,  5, 10, 25, 25, 10,  5,  5 },
                    { 0,  0,  0, 20, 20,  0,  0,  0 },
                    { 5, -5,-10,  0,  0,-10, -5,  5 },
                    { 5, 10, 10,-20,-20, 10, 10,  5 },
                    { 0,  0,  0,  0,  0,  0,  0,  0 }
                }
            },

            {
                Rank.ROOK, new int[,]
                {
                    { 0,  0,  0,  0,  0,  0,  0,  0 },
                    { 5, 10, 10, 10, 10, 10, 10,  5 },
                    { -5,  0,  0,  0,  0,  0,  0, -5 },
                    { -5,  0,  0,  0,  0,  0,  0, -5 },
                    { -5,  0,  0,  0,  0,  0,  0, -5 },
                    { -5,  0,  0,  0,  0,  0,  0, -5 },
                    { -5,  0,  0,  0,  0,  0,  0, -5 },
                    { 0,  0,  0,  5,  5,  0,  0,  0 }
                }
            },

            {
                Rank.KNIGHT, new int[,]
                {
                    { -50,-40,-30,-30,-30,-30,-40,-50 },
                    { -40,-20,  0,  0,  0,  0,-20,-40 },
                    { -30,  0, 10, 15, 15, 10,  0,-30 },
                    { -30,  5, 15, 20, 20, 15,  5,-30 },
                    { -30,  0, 15, 20, 20, 15,  0,-30 },
                    { -30,  5, 10, 15, 15, 10,  5,-30 },
                    { -40,-20,  0,  5,  5,  0,-20,-40 },
                    { -50,-40,-30,-30,-30,-30,-40,-50 }
                }
            },

            {
                Rank.BISHOP, new int[,]
                {
                    { -20,-10,-10,-10,-10,-10,-10,-20 },
                    { -10,  0,  0,  0,  0,  0,  0,-10 },
                    { -10,  0,  5, 10, 10,  5,  0,-10 },
                    { -10,  5,  5, 10, 10,  5,  5,-10 },
                    { -10,  0, 10, 10, 10, 10,  0,-10 },
                    { -10, 10, 10, 10, 10, 10, 10,-10 },
                    { -10,  5,  0,  0,  0,  0,  5,-10 },
                    { -20,-10,-10,-10,-10,-10,-10,-20 }
                }
            },

            {
                Rank.QUEEN, new int[,]
                {
                    { -20,-10,-10, -5, -5,-10,-10,-20 },
                    { -10,  0,  0,  0,  0,  0,  0,-10 },
                    { -10,  0,  5,  5,  5,  5,  0,-10 },
                    { -5,  0,  5,  5,  5,  5,  0, -5 },
                    { 0,  0,  5,  5,  5,  5,  0, -5 },
                    { -10,  5,  5,  5,  5,  5,  0,-10 },
                    { -10,  0,  5,  0,  0,  0,  0,-10 },
                    { -20,-10,-10, -5, -5,-10,-10,-20 }
                }
            },

            {
                Rank.KING, new int[,]
                {
                    //King mid game
                    { -30,-40,-40,-50,-50,-40,-40,-30, },
                    { -30,-40,-40,-50,-50,-40,-40,-30, },
                    { -30,-40,-40,-50,-50,-40,-40,-30, },
                    { -30,-40,-40,-50,-50,-40,-40,-30, },
                    { -20,-30,-30,-40,-40,-30,-30,-20, },
                    { -10,-20,-20,-20,-20,-20,-20,-10, },
                    { 20, 20,  0,  0,  0,  0, 20, 20, },
                    { 20, 30, 10,  0,  0, 10, 30, 20 }
                }
            }
        };

        private readonly Dictionary<Rank, int> _materialScore = new()
        {
            //This value here is for White. If Black, just multiply to -1
            { Rank.PAWN, 100 },
            { Rank.ROOK, 500 },
            { Rank.KNIGHT, 300 },
            { Rank.BISHOP, 300 },
            { Rank.QUEEN, 900 },
            { Rank.KING, 100000 } //The King will have a really large value -> We don't want the King to be capture at all cost
        };

        //This will be used for running simulation. We still need to synchronize with the real board
        public ChessBoard ChessBoard { get; } = new ChessBoard();

        public int Evaluate()
        {
            int totalVal = 0, temp;
            Piece? p;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    p = ChessBoard.GetPieceAt(row, col);
                    if (p is not null)
                    {
                        //Calculate the material score
                        temp = _materialScore[p.Rank];
                        totalVal += p.Color == Color.WHITE ? temp : temp * -1;
                        //Calculate the positional score
                        temp = _positionalScore[p.Rank][p.Position.Row, p.Position.Column];
                        totalVal += p.Color == Color.WHITE ? temp : temp * -1;
                    }
                }
            }

            return totalVal;
        }

        /*
         * Algorithm pseudo code: 
         * func minimax(position, depth, alpha, beta, isMax)
         *      if depth == 0 or game over in position
         *          return static evaluation of position
         *      if isMax
         *          maxEva = -Infinity
         *          for each child of position
         *              eva = minimax(child, depth - 1, alpha, beta, !isMax)
         *              maxEva = max(eva, maxEva)
         *              alpha = max(alpha, eva)
         *              if beta <= alpha
         *                  break
         *          return maxEva
         *      else
         *          minEva = +Infinity
         *          for each child of position
         *              eva = minimax(child, depth - 1, alpha, beta, !isMax)
         *              minEva = min(eva, minEva)
         *              beta = min(beta, eva)
         *          return minEva        
         * 
         */

        //public int Minimax(int depth, int alpha, int beta) 
        //{
        //    if (depth == 0 || ChessBoard.IsCheckmate())
        //    {
        //        return Evaluate();
        //    }

        //    if (ChessBoard.IsWhiteTurn)
        //    {
        //        int maxEvaluation = Int32.MaxValue;
        //        List<MoveInfo> allMoves = [];
        //        //Populate all moves list with the current state of the board
        //        ChessBoard.GetAllMovesPossible(allMoves, ChessBoard.IsWhiteTurn ? Color.WHITE : Color.BLACK);
        //        //We do a DFS for each move it can make
        //        foreach (var p in allMoves)
        //        {
        //            //Make a move to 
        //        }
        //    }
        //    return 0;
        //}
    }

    class Node
    {
        public int Val { get; set; }

        public List<Node> Children { get; set; } = [];

        public Node? Parent { get; set; }

        public string Move { get; set; }

        public Piece? State { get; set; }

        public Node(Piece? state)
        {
            State = state;

        }
    }

}
