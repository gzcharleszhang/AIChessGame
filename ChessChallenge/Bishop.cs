/*
 * Charles, Nemo, Henry
 * ISU
 * Jan 25, 2017
 * Bishop Class
 * Creates bishop objects
 * Can move diagonally, unlimited distance
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessChallenge
{
    class Bishop : Piece
    {
        /// <summary>
        /// Constrcutor creates the bishop object
        /// </summary>
        /// <param name="owner">the player that owns the pice</param>
        /// <param name="x">which column the bishop starts </param>
        /// <param name="y">which row the bishop starts</param>
        public Bishop(PlayerType owner, int x, int y) : base(PieceType.Bishop, owner, x, y)
        {

        }

        /// <summary>
        /// Gets the spaces of which the bishop is able to move to
        /// </summary>
        /// <param name="board">the board that the game is on</param>
        /// <param name="useCurrentKing">determines which player's king to use</param>
        public override void GetValidSpaces(ref BoardModel board, bool useCurrentKing)
        {
            // Stores the directions that the bishop is able to move towards
            Point[] unit = { new Point(1, 1), new Point(-1, 1), new Point(1, -1), new Point(-1, -1) };
            // Clears all the valid spaces and add the bishop's current point
            board.ValidSpaces.Clear();
            board.ValidSpaces.Add(new Point(X, Y));
            // The board's max length is 8
            const int MAX_LENGTH = 8;

            // Loops 4 times
            for (int i = 0; i < 4; i++)
            {
                // Check up to the maximum distance
                for (int dist = 1; dist <= MAX_LENGTH; dist++)
                {
                    // Stores the next point
                    Point nextPoint = new Point(X + unit[i].X * dist, Y + unit[i].Y * dist);

                    // Determines if the next poine is within bounds
                    bool inBounds = nextPoint.X < MAX_LENGTH && nextPoint.X >= 0 && nextPoint.Y < MAX_LENGTH && nextPoint.Y >= 0;
                    // If it is in bounds
                    if (inBounds)
                    {
                        // Stores the piece at the next locaiton
                        Piece nextPiece = board.Pieces[nextPoint.X, nextPoint.Y];
                        // Break if there is an ally piece at the location
                        if (nextPiece != null && nextPiece.Owner == this.Owner)
                        {
                            break;
                        }
                        // if the king will not be checked
                        else if (WillCheck(ref board, useCurrentKing, new Point(X, Y), nextPoint))
                        {
                            // Add the next point to valid spaces
                            board.ValidSpaces.Add(nextPoint);
                            // Break if the next piece is an enemy
                            if (nextPiece != null && nextPiece.Owner != this.Owner)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
