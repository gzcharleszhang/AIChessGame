/*
 * Charles, Nemo, Henry
 * ISU
 * JAn25, 2017
 * Player Class
 * Creates player objects that play the game
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ChessChallenge
{
    public class Player
    {
        // Stores the player's name
        private string _name;
        // Stores the pieces that the player captured
        private List<Piece> _piecesCaptured;
        // State variable whehter the player is checked or not
        private bool _isChecked;
        // Stores the number of moves the player made
        private int _numMoves;
        // Stores the type of player the player is
        private PlayerType _id;
        // Stores the pieces that are checking the player
        private List<Piece> _piecesChecking;
        // store the start time of the player's turn
        private int _turnStartTime = 90 * 60;
        // Stores the king piece
        private King _kingPiece;

        /// <summary>
        /// Allows the king piece to be visible to other classes
        /// </summary>
        public King KingPiece
        {
            get
            {
                return _kingPiece;
            }
            // king piece can be changed externally
            set
            {
                _kingPiece = value;
            }
        }
        /// <summary>
        /// Allows the turn start time to be visible to other classes
        /// </summary>
        public int TurnStartTime
        {
            get
            {
                return _turnStartTime;
            }
            // turn start time can be changed externally
            set
            {
                _turnStartTime = value;
            }
        }

        /// <summary>
        /// Allows the player's name to be visible to other classes
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }
        /// <summary>
        /// Allows the player's id to be visible to other classes
        /// </summary>
        public PlayerType ID
        {
            get
            {
                return _id;
            }
            // id can only be changed internally
            private set
            {
                _id = value;
            }
        }
        /// <summary>
        /// Allows the pieces captured to be visible to other classes
        /// </summary>
        public List<Piece> PiecesCaptured
        {
            get
            {
                return _piecesCaptured;
            }
        }
        /// <summary>
        /// Constructor that creates the player object
        /// </summary>
        /// <param name="name">the name of the player</param>
        /// <param name="id">the type of player</param>
        public Player(string name, PlayerType id)
        {
            // Assigns the player's starting values
            _name = name;
            _id = id;

            // Creates the lists of pieces captured and checking
            _piecesCaptured = new List<Piece>();
            _piecesChecking = new List<Piece>();
        }

        /// <summary>
        /// Allows the number of moves to be visible to other classes
        /// </summary>
        public int NumMoves
        {
            get
            {
                return _numMoves;
            }
        }
        /// <summary>
        /// Starts the turn
        /// </summary>
        public void StartTurn()
        {
            // increaes the number of moves by 1
            _numMoves++;
            // increases the start tim by 30 seconds
            _turnStartTime += 30;
            // If the there are more than one piece checking
            if (PiecesChecking.Count > 0)
            {
                // The player is checked
                _isChecked = true;
            }
        }
        /// <summary>
        /// Allows is checked to be visible to other classes
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            // is checked can be changed externally
            set
            {
                _isChecked = value;
            }
        }

        /// <summary>
        /// Allows the pieces checking to be visible to other classes
        /// </summary>
        public List<Piece> PiecesChecking
        {
            get
            {
                return _piecesChecking;
            }
        }

    }
}
