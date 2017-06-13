/*
 * Charles, Nemo, Henry
 * ISU
 * Jan 25, 2017
 * Rook Class
 * Craetes rook objects
 * Moves straight
 * unlimited distance
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;

namespace ChessChallenge
{
    class Rook : Piece
    {
        /// <summary>
        /// constructor the creates rook objects
        /// </summary>
        /// <param name="owner">player that owns the rook</param>
        /// <param name="x">the column the rook starts</param>
        /// <param name="y">the row the rook starts on</param>
        public Rook(PlayerType owner, int x, int y) : base(PieceType.Rook, owner, x, y)
        {

        }

        /// <summary>
        /// get the spaces that the rook is able to move to
        /// </summary>
        /// <param name="board">the board the game is on</param>
        /// <param name="useCurrentKing">which player's king to use</param>
        public override void GetValidSpaces(ref BoardModel board, bool useCurrentKing)
        {
            // the directions of the rook
            Point[] unit = { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
            // clears and add the starting point to valid spaces
            board.ValidSpaces.Clear();
            board.ValidSpaces.Add(new Point(X, Y));
            // loops twice
            for (int i = 0; i < 2; ++i)
            {
                // loops 4 times
                for (int dist = 1; dist <= 4; ++dist)
                {
                    // stores a point
                    Point point;

                    // moves to the left
                    if (i == 0)
                    {
                        point = new Point(X - dist, Y);
                    }
                    else
                    {
                        // moves to the right
                        point = new Point(X + dist, Y);
                    }

                    // if its in bounds
                    if (point.X < 8 && point.X >= 0 && point.Y < 8 && point.Y >= 0)
                    {
                        // stores a piece at the location
                        Piece check = board.Pieces[point.X, point.Y];

                        // if it doesnt not exist
                        if (check != null)
                        {
                            // if it is castling
                            if (check.Type == PieceType.King && !check.HasMoved && !this.HasMoved && ((i == 0 && dist == 3) || (i == 1 && dist == 4)))
                            {
                                // create a clone of the board
                                Piece[,] testBoard = (Piece[,])board.Pieces.Clone();

                                // stores the king's location
                                Point kingLoc;

                                if (i == 0)
                                {
                                    // the king moves right
                                    kingLoc = new Point(point.X + 2, point.Y);

                                    testBoard[point.X, point.Y] = this;
                                    testBoard[point.X + 2, point.Y] = board.CurrentPlayer.KingPiece;
                                }
                                else
                                {
                                    // the king moves left
                                    kingLoc = new Point(point.X - 2, point.Y);

                                    testBoard[point.X, point.Y] = this;
                                    testBoard[point.X - 2, point.Y] = board.CurrentPlayer.KingPiece;
                                }

                                if (board.CurrentPlayer.KingPiece.isSafe(ref testBoard, kingLoc))
                                {
                                    // add to valid spaces
                                    board.ValidSpaces.Add(point);
                                }
                            }
                            else
                            {
                                // exit the loop
                                break;
                            }
                        }
                    }
                    else
                    {
                        // exit the loop
                        break;
                    }
                }
            }

            // the board has a max length of 8
            const int MAX_LENGTH = 8;

            //the rook can move towradsd 4 directions
            for (int i = 0; i < 4; i++)
            {
                // it can move to masx length
                for (int dist = 1; dist <= MAX_LENGTH; dist++)
                {
                    // store sthe next point
                    Point nextPoint = new Point(X + unit[i].X * dist, Y + unit[i].Y * dist);
                    // determines if it is in bounds
                    bool inBounds = nextPoint.X < MAX_LENGTH && nextPoint.X >= 0 && nextPoint.Y < MAX_LENGTH && nextPoint.Y >= 0;
                    // if it is in bounds
                    if (inBounds)
                    {
                        // stores the piece at the next loaiton
                        Piece nextPiece = board.Pieces[nextPoint.X, nextPoint.Y];

                        // if the piece is an ally, break
                        if (nextPiece != null && nextPiece.Owner == this.Owner)
                        {
                            break;
                        }
                        //if the king is not checked
                        else if (WillCheck(ref board, useCurrentKing, new Point(X, Y), nextPoint))
                        {
                            // add to valid spaces
                            board.ValidSpaces.Add(nextPoint);
                            // if the next piece is an enemy, break
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

