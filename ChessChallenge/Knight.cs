/*
 * Charles, Nemo, Henry
 * ISU
 * Jan 25, 2017
 * Knight class
 * Creates knight objects
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessChallenge
{
    class Knight : Piece
    {
        /// <summary>
        /// Cosntructor that creates knight objects
        /// </summary>
        /// <param name="owner">the player tht owns the knight</param>
        /// <param name="x">the column the knight starts</param>
        /// <param name="y">the row the knight starts</param>
        public Knight(PlayerType owner, int x, int y) : base(PieceType.Knight, owner, x, y)
        {

        }

        /// <summary>
        /// Gets the spaces that the knight is able to move to
        /// </summary>
        /// <param name="board">the board that the game is on</param>
        /// <param name="useCurrentKing">determines which player's king to use</param>
        public override void GetValidSpaces(ref BoardModel board, bool useCurrentKing)
        {
            // The directions that the knight is able to move towards
            Point[] unit = { new Point(2, 1), new Point(-2, 1), new Point(2, -1), new Point(-2, -1), new Point(1, 2), new Point(-1, 2), new Point(1, -2), new Point(-1, -2) };
            // Clears the validspaces and add its current location to valid spaces
            board.ValidSpaces.Clear();
            board.ValidSpaces.Add(new Point(X, Y));
            // The board has a max length of 8
            const int MAX_LENGTH = 8;
            // Loops 8 times
            for (int i = 0; i < 8; i++)
            {
                // Stores the next point
                Point nextPoint = new Point(X + unit[i].X, Y + unit[i].Y);
                // Determines if the next ponit is within bounds
                bool inBounds = nextPoint.X < MAX_LENGTH && nextPoint.X >= 0 && nextPoint.Y < MAX_LENGTH && nextPoint.Y >= 0;
                // If it is in bounds
                if (inBounds)
                {
                    // Stores the piece at the next point
                    Piece nextPiece = board.Pieces[nextPoint.X, nextPoint.Y];
                    // If there isn't a piece or if it is an enemy and if it will not get the king checked
                    if ((nextPiece == null || nextPiece.Owner != this.Owner) && WillCheck(ref board, useCurrentKing, new Point(X, Y), nextPoint))
                    {
                        // Add valid spaces
                        board.ValidSpaces.Add(nextPoint);
                    }
                }
            }
        }
    }
}
