/*
 * Charles, Nemo, Henry
 * ISU
 * Jan 25, 2017
 * Queen Class
 * Craetes queen objects
 * Moves diagonally and straight
 * unlimited distance
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessChallenge
{
    class Queen : Piece
    {
        /// <summary>
        /// Constructor that creates queen objects
        /// </summary>
        /// <param name="owner">the piece that owns the queen</param>
        /// <param name="x">the column the queen starts</param>
        /// <param name="y">the row the queen starts</param>
        public Queen(PlayerType owner, int x, int y) : base(PieceType.Queen, owner, x, y)
        {

        }

        /// <summary>
        /// Get spaces that the queen is able to move to
        /// </summary>
        /// <param name="board">the board the game is on</param>
        /// <param name="useCurrentKing">which player's king to use</param>
        public override void GetValidSpaces(ref BoardModel board, bool useCurrentKing)
        {
            // the direcitons of the queen
            Point[] unit = { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), new Point(1, 1), new Point(-1, 1), new Point(1, -1), new Point(-1, -1) };
            // clears and add current location to valid spaces
            board.ValidSpaces.Clear();
            board.ValidSpaces.Add(new Point(X, Y));
            // board has max length of 8
            const int MAX_LENGTH = 8;
            // queen has 8 directions
            for (int i = 0; i < 8; i++)
            {
                // queen can move to max length
                for (int dist = 1; dist <= MAX_LENGTH; dist++)
                {
                    // stores the next point
                    Point nextPoint = new Point(X + unit[i].X * dist, Y + unit[i].Y * dist);
                    // determines if it is inbounds
                    bool inBounds = nextPoint.X < MAX_LENGTH && nextPoint.X >= 0 && nextPoint.Y < MAX_LENGTH && nextPoint.Y >= 0;
                    // if it is imbounds
                    if (inBounds)
                    {
                        // stores the piece at the next point
                        Piece nextPiece = board.Pieces[nextPoint.X, nextPoint.Y];

                        // if it is an ally piece, break
                        if (nextPiece != null && nextPiece.Owner == this.Owner)
                        {
                            break;
                        }
                        //if king is not checked
                        else if (WillCheck(ref board, useCurrentKing, new Point(X, Y), nextPoint))
                        {
                            // add to valid spaces
                            board.ValidSpaces.Add(nextPoint);

                            // if it is an enemy piece, break
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
