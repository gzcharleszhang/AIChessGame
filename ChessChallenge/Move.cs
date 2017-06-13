/*
 * Charles, Nemo, Henry
 * ISU
 * Jan 25, 2017
 * Move class
 * creates a move object
 * indicates the move action of a piece
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class Move
    {
        // stores the ending point's x and y coordinate
        private int _endX;
        private int _endY;

        // stores the starting point's x and y coordinate
        private int _startX;
        private int _startY;

        // state variable whether the piece has moved or not
        private bool _hasMoved;

        /// <summary>
        /// Constructor that creates a move object
        /// </summary>
        /// <param name="startX">the starting point's x</param>
        /// <param name="startY">the starting point's y</param>
        /// <param name="endX">the ending point's x</param>
        /// <param name="endY">the ending point's y</param>
        /// <param name="hasMoved">whether the piece has moved or not</param>
        public Move(int startX, int startY, int endX, int endY, bool hasMoved)
        {
            // Assigns the starting values
            _endX = endX;
            _endY = endY;
            _startX = startX;
            _startY = startY;
            _hasMoved = hasMoved;
        }
        /// <summary>
        /// allows startx to be visible by other classes
        /// </summary>
        public int StartX
        {
            get
            {
                return _startX;
            }
        }
        /// <summary>
        /// allows starty to be visible by other classes
        /// </summary>
        public int StartY
        {
            get
            {
                return _startY;
            }
        }
        /// <summary>
        /// allows endx to be visible by other classes
        /// </summary>
        public int EndX
        {
            get
            {
                return _endX;
            }
        }
        /// <summary>
        /// allows end y to be visibl by other classes
        /// </summary>
        public int EndY
        {
            get
            {
                return _endY;
            }
        }

        /// <summary>
        /// allows hasmoved to be visible by other classes
        /// </summary>
        public bool HasMoved
        {
            get
            {
                return _hasMoved;
            }
        }
    }
}
