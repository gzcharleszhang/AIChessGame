/*
 * Charles, Nemo, Charles
 * ISU
 * Jan 25,2017
 * Pawn class
 * Creates pawn objects
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessChallenge
{
    class Pawn : Piece
    {
        /// <summary>
        /// Constructor that creates pawn objects
        /// </summary>
        /// <param name="owner">the player that owns the piece</param>
        /// <param name="x">which column the pawn starts</param>
        /// <param name="y">which row the pawn starts</param>
        public Pawn(PlayerType owner, int x, int y) : base(PieceType.Pawn, owner, x, y)
        {


        }

        /// <summary>
        /// Get the spaces that the pawn is able to move to
        /// </summary>
        /// <param name="board">The board that the game is on</param>
        /// <param name="useCurrentKing">Determines which player's king to use</param>
        public override void GetValidSpaces(ref BoardModel board, bool useCurrentKing)
        {
            // Stores the directions that the pawn is able to move towards
            Point[] unit = { new Point(0, -1), new Point(1, -1), new Point(-1, -1) };
            // IF it is the player 2's pawns
            if (Owner == PlayerType.Player2)
            {
                // Stores the opposite directions
                for (int i = 0; i < 3; ++i)
                {
                    unit[i] = new Point(unit[i].X, -unit[i].Y);
                }
            }
            // Clears all the valid spaces and add in the current point
            board.ValidSpaces.Clear();
            board.ValidSpaces.Add(new Point(X, Y));
            // The board has a max length of 8
            const int MAX_LENGTH = 8;
            // The pawn is 3 directions
            for (int i = 0; i < 3; i++)
            {
                // The pawn can move up to 2 grids
                for (int dist = 1; dist <= 2; dist++)
                {
                    
                    // Stores the next point
                    Point nextPoint = new Point(X + unit[i].X * dist, Y + unit[i].Y * dist);
                    // Determines if the next point is going to be inbounds
                    bool inBounds = nextPoint.X < MAX_LENGTH && nextPoint.X >= 0 && nextPoint.Y < MAX_LENGTH && nextPoint.Y >= 0;
                    // If it is in bounds
                    if (inBounds)
                    {
                        // Stores the piece at the next point
                        Piece nextPiece = board.Pieces[nextPoint.X, nextPoint.Y];
                        // Break if the pawn is not able to move onto that point
                        if ((i == 0 && ((dist == 2 && HasMoved) || nextPiece != null)) || (i > 0 && (dist == 2 || nextPiece == null || nextPiece.Owner == this.Owner)))
                        {
                            break;
                        }
                        // See if the king is not checked
                        else if (WillCheck(ref board, useCurrentKing, new Point(X, Y), nextPoint))
                        {
                            // Add the point to valid spaces
                            board.ValidSpaces.Add(nextPoint);
                        }
                    }
                    else
                    {
                        // Exists the loop
                        break;
                    }
                }
            }
        }
    }
}
