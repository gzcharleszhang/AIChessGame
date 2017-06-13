/*
 * Charles, Nemo, Henry
 * ISU
 * Jan 25, 2017
 * King class
 * Creates king objects
 * Moves one space straight or diagonally
 * Player loses when King is captured
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChessChallenge
{
    public class King : Piece
    {
        /// <summary>
        /// Constructor that creates the king class
        /// </summary>
        /// <param name="owner">The player that owns the king</param>
        /// <param name="x">Which column the king starts</param>
        /// <param name="y">Which row the king starts</param>
        public King(PlayerType owner, int x, int y) : base(PieceType.King, owner, x, y)
        {

        }

        /// <summary>
        /// Gets the spaces that the king is able to move to
        /// </summary>
        /// <param name="board">the board that the game is on</param>
        /// <param name="useCurrentKing">Determines which player's king to use</param>
        public override void GetValidSpaces(ref BoardModel board, bool useCurrentKing)
        {
            // the directions that the king is able to move towards
            Point[] unit = { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), new Point(1, 1), new Point(-1, 1), new Point(1, -1), new Point(-1, -1) };
            // Clears the list of valid sapces and add the piece's current location
            board.ValidSpaces.Clear();
            board.ValidSpaces.Add(new Point(X, Y));
            // Loops twice
            for (int i = 0; i < 2; ++i)
            {
                // Loops 4 times, distance increases by 1 each time
                for (int dist = 1; dist <= 4; ++dist)
                {
                    // Stores a point
                    Point point;
                    // For the first time it loops
                    if (i == 0)
                    {
                        // Point is on the left
                        point = new Point(X - dist, Y);
                    }
                    else
                    {
                        // Point is on the right
                        point = new Point(X + dist, Y);
                    }
                    // If the point is in bounds
                    if (point.X < 8 && point.X >= 0 && point.Y < 8 && point.Y >= 0)
                    {
                        // Create a temporary piece
                        Piece check = board.Pieces[point.X, point.Y];
                        // If the piece exists
                        if (check != null)
                        {
                            // if the piece's type is a rook and it has not moved and the king hs not moved
                            if (check.Type == PieceType.Rook && !check.HasMoved && !this.HasMoved && ((i == 0 && dist == 4) || (i == 1 && dist == 3)))
                            {
                                // Creates a clone of the board
                                Piece[,] testBoard = (Piece[,])board.Pieces.Clone();
                                // Store sthe king's location
                                Point kingLoc;
                                // for the first time it loops
                                if (i == 0)
                                {
                                    //Move the king's location to the right
                                    kingLoc = new Point(point.X + 2, point.Y);
                                    //Move the kign
                                    testBoard[point.X + 3, point.Y] = testBoard[point.X, point.Y];
                                    testBoard[point.X + 2, point.Y] = this;
                                }
                                else
                                {
                                    // Move the king's location to the left
                                    kingLoc = new Point(point.X - 2, point.Y);
                                    // Move the king
                                    testBoard[point.X - 2, point.Y] = testBoard[point.X, point.Y];
                                    testBoard[point.X - 1, point.Y] = this;
                                }

                                //If it's safe to move to the location.
                                if (this.isSafe(ref testBoard, kingLoc))
                                {
                                    // Add to valid sapces
                                    board.ValidSpaces.Add(point);
                                }
                            }
                            else
                            {
                                // Break out of the inner loop
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Break out of the outer loop
                        break;
                    }
                }
            }
            // The maximum length of the board, which is 8
            const int MAX_LENGTH = 8;
            // Loops 8 times
            for (int i = 0; i < 8; i++)
            {
                // Stores the king's next point
                Point nextPoint = new Point(X + unit[i].X, Y + unit[i].Y);
                // Determines if the point is gonna be in bounds
                bool inBounds = nextPoint.X < MAX_LENGTH && nextPoint.X >= 0 && nextPoint.Y < MAX_LENGTH && nextPoint.Y >= 0;
                // If it is in bounds
                if (inBounds)
                {
                    // stores a copy of the pieces
                    Piece[,] pieces = board.Pieces;
                    // Stores the piece at the next point
                    Piece nextPiece = pieces[nextPoint.X, nextPoint.Y];

                    //Warning: Cancer code ahead - proceed with caution.
                    // If the next piece does nto exist and it is an enemy
                    if (nextPiece == null || nextPiece.Owner != this.Owner)
                    {
                        //If it's safe to move the king to the position.
                        if (isSafe(ref pieces, nextPoint))
                        {
                            // Add to valid spaces
                            board.ValidSpaces.Add(nextPoint);
                            // No piece is checking the king
                            board.CurrentPlayer.PiecesChecking.Clear();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns if the king is safe at it's current position.
        /// </summary>
        /// <param name="pieces">Reference to the board holding the pieces</param>
        /// <param name="nextPoint">The point the king is located on</param>
        /// <returns></returns>
        public bool isSafe(ref Piece[,] pieces, Point nextPoint)
        {
            // Stores the directions that the king is able to move towards
            Point[] newUnit = { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), new Point(1, 1), new Point(-1, 1), new Point(1, -1), new Point(-1, -1), new Point(2, 1), new Point(-2, 1), new Point(2, -1), new Point(-2, -1), new Point(1, 2), new Point(-1, 2), new Point(1, -2), new Point(-1, -2) };

            // The king is able to move towards 8 directions and check all over the board if anyone is checking
            for (int j = 0; j < newUnit.Length; ++j)
            {
                for (int dist = 1; dist <= 8; ++dist)
                {
                    // Stores the next point
                    Point newPoint2 = new Point(nextPoint.X + newUnit[j].X * dist, nextPoint.Y + newUnit[j].Y * dist);
                    // IF the point is in bounds
                    if (newPoint2.X < 8 && newPoint2.X >= 0 && newPoint2.Y < 8 && newPoint2.Y >= 0)
                    {
                        // Stores the piece at the next point
                        Piece piece = pieces[newPoint2.X, newPoint2.Y];
                        // IF the piece exists and if it is an enemy
                        if (piece != null)
                        {
                            if (piece.Owner != this.Owner)
                            {
                                // It is not safe if a King, queen, or rook can capture the king
                                if (j <= 3 && j >= 0 && ((dist == 1 && piece.Type == PieceType.King) || piece.Type == PieceType.Queen || piece.Type == PieceType.Rook))
                                {

                                    return false;
                                }
                                // It is not safe if a pawn cap capture the king
                                if (((j <= 7 && j >= 6 && piece.Owner == PlayerType.Player2) || (j <= 5 && j >= 4 && piece.Owner == PlayerType.Player1)) && dist == 1 && piece.Type == PieceType.Pawn)
                                {

                                    return false;
                                }
                                // It isn ot safe if a bishop, king, or queen can capture the king
                                if (j <= 7 && j >= 4 && (piece.Type == PieceType.Bishop || (dist == 1 && piece.Type == PieceType.King) || piece.Type == PieceType.Queen))
                                {

                                    return false;
                                }
                                // It is not safe if a knight can capture the king
                                if (j > 7 && piece.Type == PieceType.Knight && dist == 1)
                                {

                                    return false;
                                }
                            }
                            // exists the loop if the piece is another ally piece
                            if (piece != this)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            // The king is safe
            return true;
        }
    }
}
