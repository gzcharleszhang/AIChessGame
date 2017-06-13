/*
 * Henry Gao
 * Stores the movement and capturing information when a piece moves
 * So that the board can be reverted to before the move occurs.
 * Jan 24th, 2017
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class MoveNode
    {
        //Stores the movement information.
        private Move _move;

        //Stores the capturing information.
        private Capture _capture;

        /// <summary>
        /// Constructs a move node that stores a certain move and capture node.
        /// </summary>
        /// <param name="move">Copy of the movement information.</param>
        /// <param name="capture">Copy of the capture information.</param>
        public MoveNode(Move move, Capture capture)
        {
            //Stores the movement and captured information into private variables.
            _move = move;
            _capture = capture;
        }

        /// <summary>
        /// Restores and returns the board BEFORE the move stored in this move node occured. 
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <returns>The 2D array of pieces representing the board of pieces</returns>
        public Piece[,] Restore(BoardModel board)
        {
            //Checks to see if the move has been reverted already, if it has, then stop restoring.
            if (board.Pieces[_move.EndX, _move.EndY] != null)
            {
                //If a piece has been captured, restore the capturing information too.
                if (_capture != null)
                {
                    

                    //Restores the captured piece to it's previous position on the BOARD.
                    board.Pieces[_move.EndX, _move.EndY] = _capture.Piece;

                    //Stores if the piece has moved before in PIECE.
                    bool hasMoved = _capture.Piece.HasMoved;

                    //Moves the piece to it's previous location.
                    board.Pieces[_move.EndX, _move.EndY].Move(board, _move.EndX, _move.EndY);

                    //Restores if the piece moved before, since move automatically sets hasMoved to true.
                    _capture.Piece.HasMoved = hasMoved;

                    //Restores the moved piece to it's previous position.
                    board.Pieces[_move.StartX, _move.StartY] = _capture.Piece2;
                    board.Pieces[_move.StartX, _move.StartY].Move(board, _move.StartX, _move.StartY);

                }
                //If a piece has not been captured, just restore the moved piece.
                else
                {
                    //Restores the moved piece to it's previous position, and sets the current position to empty.
                    board.Pieces[_move.StartX, _move.StartY] = board.Pieces[_move.EndX, _move.EndY];
                    board.Pieces[_move.EndX, _move.EndY] = null;

                    board.Pieces[_move.StartX, _move.StartY].Move(board, _move.StartX, _move.StartY);
                }
            }

            //Returns the board of pieces.
            return board.Pieces;
        }
    }
}
