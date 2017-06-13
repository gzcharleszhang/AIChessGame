/*
 * Charles, Nemo, Henry
 * Jan 25, 2017
 * ISU
 * Capture Class
 * stores the piece that is capturing and the piece that is beign captured
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class Capture
    {
        // stores the piece that is capturing
        private Piece _piece;
        // stores the piece that is being captured
        private Piece _piece2;

        /// <summary>
        /// constuctor that creates the capture object
        /// </summary>
        /// <param name="piece">the piece that is capturing</param>
        /// <param name="piece2">the piece that is being captured</param>
        public Capture(Piece piece, Piece piece2)
        {
            // assigns the starting value
            _piece = piece;
            _piece2 = piece2;
        }

        /// <summary>
        /// Allows piece to be visible by other classes
        /// </summary>
        public Piece Piece
        {
            get
            {
                return _piece;
            }
            // piece can be changed externally
            set
            {
                _piece = value;
            }
        }
        
        /// <summary>
        /// allows piece2 to be visible by other classes
        /// </summary>
        public Piece Piece2
        {
            get
            {
                return _piece2;
            }
            // piece2 can be changed externally
            set
            {
                _piece2 = value;
            }
        }
    }
}
