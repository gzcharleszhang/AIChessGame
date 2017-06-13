/*
 * Charles, Nemo, Henry
 * Culminating Project
 * Jan 25, 2017
 * Piece class
 * Creates piece objects to be moved on the board
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace ChessChallenge
{
    public abstract class Piece
    {
        // State variable shows whether a piece has moved or not
        protected bool _hasMoved;
        // Stores the type of the piece
        protected PieceType _type;
        // Stores which player owns the piece
        protected PlayerType _owner;
        // Stores the piece's image
        protected Bitmap _image;
        // Stores the x and y location of the piece on the board
        protected int _x, _y;

        /// <summary>
        /// Constructor creates the piece object
        /// </summary>
        /// <param name="type">the type of the piece</param>
        /// <param name="owner">the player that owns the piece</param>
        /// <param name="x">which column the piece is on</param>
        /// <param name="y">which row the piece is on</param>
        public Piece(PieceType type, PlayerType owner, int x, int y)
        {
            // Assigns the object's starting values
            _type = type;
            _owner = owner;
            _x = x;
            _y = y;
            _hasMoved = false;
        }

        /// <summary>
        /// Allows the piece's X to be visible to other classes
        /// </summary>
        public int X
        {
            get
            {
                return _x;
            }
        }
        /// <summary>
        /// Allows the piece's Y to be visible to other classes
        /// </summary>
        public int Y
        {
            get
            {
                return _y;
            }
        }
        /// <summary>
        /// Allows the piece's image to be visible to other classes
        /// </summary>
        public Bitmap Image
        {
            get
            {
                return _image;
            }
        }
        /// <summary>
        /// Allows the piece's type to be visible to other classes
        /// </summary>
        public PieceType Type
        {
            get
            {
                return _type;
            }
        }
        /// <summary>
        /// Allows the piece's hasMoved to be visible to other classes
        /// </summary>
        public bool HasMoved
        {
            get
            {
                return _hasMoved;
            }
            set
            {
                _hasMoved = value;
            }
        }

        /// <summary>
        /// Allows the piece's owner to be visible to other classes
        /// </summary>
        public PlayerType Owner
        {
            get
            {
                return _owner;
            }
        }
        /// <summary>
        /// The child classes of this class can get valid spaces
        /// where the piece is able to move to
        /// </summary>
        /// <param name="board">the board that the game is running on</param>
        /// <param name="flag">a flag</param>
        public abstract void GetValidSpaces(ref BoardModel board, bool flag);

        /// <summary>
        /// Moves the piece
        /// </summary>
        /// <param name="board">the board that the game is running on</param>
        /// <param name="x">destination's x coordinate</param>
        /// <param name="y">destination's y coordinate</param>
        public void Move(BoardModel board, int x, int y)
        {
            // Moves the piece
            _x = x;
            _y = y;
            _hasMoved = true;
        }

        /// <summary>
        /// Determines if the king will be checked
        /// </summary>
        /// <param name="board">The board that the game runs on</param>
        /// <param name="flag">determines which player is checking</param>
        /// <param name="currLoc">The current location</param>
        /// <param name="nextLoc">Next location of the piece</param>
        /// <returns></returns>
        public bool WillCheck(ref BoardModel board, bool flag, Point currLoc, Point nextLoc)
        {
            // Make a clone of the board
            Piece[,] testBoard = (Piece[,])board.Pieces.Clone();

            // Make a copy of the king piece
            King king = board.CurrentPlayer.KingPiece;
            // Make a cop of the other player's king piece if the flag is false
            if (!flag)
            {
                king = board.OtherPlayer.KingPiece;
            }
            // Move the piece to the next location
            testBoard[nextLoc.X, nextLoc.Y] = testBoard[currLoc.X, currLoc.Y];
            testBoard[currLoc.X, currLoc.Y] = null;

            bool isSafe = king.isSafe(ref testBoard, new Point(king.X, king.Y));

            return isSafe;
        }
    }
}
