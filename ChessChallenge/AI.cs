/*
 * Henry Gao
 * The AI that takes the place of a player and is able to
 * make a decision regarding the next move to make.
 * It can then execute that move.
 * Jan 26th, 2017
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class AI : Player
    {
        //Stores the user selected difficulty from form
        private int _difficulty;

        //Reference to the local board model for game-related data.
        private BoardModel _board;

        // stores the piece that moved
        private Piece _movedPiece;

        //The worst possible move that this player can make. Used to find minimax of the other player.
        private int _worst = 0;

        /// <summary>
        /// Alliows moved piece to be visible publically
        /// </summary>
        public Piece MovedPiece
        {
            get
            {
                return _movedPiece;
            }
            // can be changed externally
            set
            {
                _movedPiece = value;
            }

        }

        // stores the point of the piece that moved
        private Point _movedPoint;

        /// <summary>
        /// Alliows moved point to be visible publically
        /// </summary>
        public Point MovedPoint
        {
            get
            {
                return _movedPoint;
            }
            // can be changed externally
            set
            {
                _movedPoint = value;
            }

        }
        // stores the max worth of a move
        private int _maxWorth = -99999;
        /// <summary>
        /// Allows moved piece to be visible publically
        /// </summary>
        public int MaxWorth
        {
            get
            {
                return _maxWorth;
            }
            // can be changed externally
            set
            {
                _maxWorth = value;
            }

        }

        /// <summary>
        /// Constructs an AI on the specified board.
        /// </summary>
        /// <param name="board">Reference to the local board model for game-related data</param>
        public AI(BoardModel board) : base("AI", PlayerType.Player2)
        {
            //Stores the local board model
            _board = board;

            //Stores the difficulty from the board.
            _difficulty = (int)_board.GameDifficulty;
        }

        /// <summary>
        /// Simulates the mini-max algorithmn to find the best move.
        /// </summary>
        /// <returns>Nothing, but the movedPiece and movedPoint will contain the best move</returns>
        public void FindBestMove()
        {
            //The move stack to simulate dfs, without using recursion.
            Stack<MoveNode> moveStack = new Stack<MoveNode>();

            //Store the current player's turn.
            PlayerType turn = PlayerType.Player2;

            //Stores the max worth to be an unreachably low number (ie. always the worst move).
            _maxWorth = -99999;

            //Stores the best piece.
            _movedPiece = null;

            //Runs the minimax algorithmn to find the best move and store it in bestPiece and point.
            Minimax(moveStack, turn, 0, null, new Point());

        }

        /// <summary>
        /// Uses the minimax algorithmn to find the best piece to move.
        /// </summary>
        /// <param name="moveStack">Reference to the stack of moves</param>
        /// <param name="turn">Which player's turn it is</param>
        /// <param name="height">The height of the dfs tree</param>
        /// <param name="basePiece">The parent piece to this child's node</param>
        /// <param name="basePoint">The parent point to this child's node</param>
        public void Minimax(Stack<MoveNode> moveStack, PlayerType turn, int height, Piece basePiece, Point basePoint)
        {
            //Loops from the first column to last column of the board.
            for (int y = 0; y < 8; ++y)
            {
                //Loops from the first column to last column of the board.
                for (int x = 0; x < 8; ++x)
                {
                    //Finds the piece at the current x and y location.
                    Piece piece = _board.Pieces[x, y];

                    //If the board is not empty at that spot and it reflects the owner's turn, it can be moved.
                    if (piece != null && piece.Owner == turn)
                    {

                        //Gets all valid spaces of the piece.
                        piece.GetValidSpaces(ref _board, true);

                        //Stores the valid spaces.
                        List<Point> validSpaces = new List<Point>();

                        /*Loops for all points thatt the piece can move to, 
                        and stores it into the validSpaces array (so it can't be overwritten by getValidSpaces again)*/
                        for (int i = 0; i < _board.ValidSpaces.Count; ++i)
                        {
                            //Stores the valid space from board into AI.
                            validSpaces.Add(new Point(_board.ValidSpaces[i].X, _board.ValidSpaces[i].Y));
                        }

                        //Loops for each valid space, see if it's the best move.
                        for (int i = 0; i < validSpaces.Count; ++i)
                        {
                            //Since the first valid space is the space itself, only check the second valid space and above.
                            if (i > 0)
                            {

                                //Stores where the piece moved.
                                Move moved = new Move(piece.X, piece.Y, validSpaces[i].X, validSpaces[i].Y, piece.HasMoved);

                                //Stores which piece was captured if any (otherwise, it's null).
                                Capture captured = null;

                                //If the piece is capturing an enemy piece.
                                if (_board.Pieces[validSpaces[i].X, validSpaces[i].Y] != null)
                                {

                                    //Store which pieces were captured and capturing.
                                    captured = new Capture(_board.Pieces[validSpaces[i].X, validSpaces[i].Y], piece);
                                }

                                //Stores the x and y location before the move.
                                int prevX = piece.X, prevY = piece.Y;

                                //Stores the current movement and capture information.
                                moveStack.Push(new MoveNode(moved, captured));

                                //Moves the piece on the board.
                                _board.Move(piece, validSpaces[i].X, validSpaces[i].Y, false);

                                //Makes the worth of a move
                                int worth = GetMoveWorth(_board);

                                //If the height of DFS tree is 1
                                if (height == 1)
                                {
                                    //If the worth is less than the worst possible move
                                    if (worth < _worst)
                                    {
                                        //Make the worst possible move the worth of the move
                                        _worst = worth;
                                    }
                                }
                                else
                                {
                                    //Grandmasters checks 2 moves. Everything else searches for 1 move.
                                    if (_difficulty == (int)Difficulty.Grandmaster)
                                    {
                                        _worst = 999999;

                                        //Runs the algorithmn again, with the board AFTER this move.
                                        Minimax(moveStack, PlayerType.Player2, height + 1, piece, new Point(validSpaces[i].X, validSpaces[i].Y));

                                        worth = _worst;
                                    }

                                    //If the move is worth more than the previously best move, store this as the best move.
                                    if (worth > _maxWorth)
                                    {
                                        //Gets the valid spaces
                                        //piece.GetValidSpaces(ref _board, true);


                                        //Checks to see if there is no error (it could not move, so it moves to the same place).
                                        if (prevX != validSpaces[i].X || prevY != validSpaces[i].Y)
                                        {
                                            //Stores the which piece and where it moved and the max worth as the worth of this move.
                                            _movedPiece = piece;
                                            _movedPoint = new Point(validSpaces[i].X, validSpaces[i].Y);
                                            _maxWorth = worth;
                                        }
                                    }
                                }

                                //Returns the movement information.
                                MoveNode moveInfo = moveStack.Pop();

                                //Restores the board to what it was like before the move.
                                _board.Pieces = moveInfo.Restore(_board);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Analyzes and returns the worth of the current board based on the AI's interests.
        /// </summary>
        /// <param name="board">Reference to the board being analyzed</param>
        /// <returns>Returns the worth as a positive or negative integer</returns>
        private int GetMoveWorth(BoardModel board)
        {
            //Stores the worth of the move.
            int score = 0;

            //If the difficulty is easy, just make a random move.
            if (_difficulty <= (int)Difficulty.Easy || _board.Form.IsCheating)
            {
                //Makes the score a random number between 0 and 100, so the highest random value is the best move.
                score += new Random().Next(100);
            }

            //If the difficulty is not easy, make an informed move.
            else
            {
                //Stores the value of capturing a piece.
                int[] pieceValue = new int[8];

                //Stores the value obtained for having a piece.
                pieceValue[(int)PieceType.Pawn] = 8;
                pieceValue[(int)PieceType.Knight] = 16;
                pieceValue[(int)PieceType.Rook] = 25;
                pieceValue[(int)PieceType.Bishop] = 25;
                pieceValue[(int)PieceType.Queen] = 65;
                pieceValue[(int)PieceType.King] = 1000;
                pieceValue[(int)PieceType.Vietnam] = 5;
                pieceValue[(int)PieceType.Hsiung] = 5;

                //The number of points obtained for checking the opponent.
                const int CHECKING_POINTS = 250;

                //Loops for all columns on the board.
                for (int i = 0; i < 8; i++)
                {
                    //Loops for all rows on the board.
                    for (int j = 0; j < 8; j++)
                    {
                        //If the space is not empty.
                        if (_board.Pieces[i, j] != null)
                        {
                            //Get it's valid spaces.
                            _board.Pieces[i, j].GetValidSpaces(ref board, true);

                            //If the piece is the AI's piece.
                            if (_board.Pieces[i, j].Owner == board.CurrentPlayer.ID)
                            {
                                //If the difficulty is hard, check to see the value of the piece.
                                if (_difficulty <= (int)Difficulty.Hard)
                                {
                                    //Increase the score by the value of the piece (since AI owns it).
                                    score += pieceValue[(int)_board.Pieces[i, j].Type];
                                }

                                //If the difficulty is grandmaster, check to see the value of the piece.
                                if (_difficulty <= (int)Difficulty.Grandmaster)
                                {
                                    //Increase the score by the number of spaces can move (since AI owns it).
                                    score += board.ValidSpaces.Count;
                                }
                            }

                            //If the piece is the other player's piece.
                            else
                            {
                                //If the difficulty is hard, check to see the value of the piece.
                                if (_difficulty <= (int)Difficulty.Hard)
                                {
                                    //Increase the score by the value of the piece (since AI doesn't owns it).
                                    score -= pieceValue[(int)_board.Pieces[i, j].Type];
                                }

                                //If the difficulty is grandmaster, check to see the value of the piece.
                                if (_difficulty <= (int)Difficulty.Grandmaster)
                                {
                                    //Increase the score by the number of spaces it can move (since AI doesn't owns it).
                                    score -= board.ValidSpaces.Count;
                                }
                            }
                        }
                    }
                }

                //Stores a reference to the pieces array from the board.
                Piece[,] thisPieces = _board.Pieces;

                //If the AI king is safe and the difficulty is at least normal, add the checking point to the store.
                if (!board.Player1.KingPiece.isSafe(ref thisPieces, new Point(KingPiece.X, KingPiece.Y)) && _difficulty <= (int)Difficulty.Normal)
                {
                    //Remove the points from the score since the AI is being checked.
                    score -= CHECKING_POINTS;
                }

                //If the player king is safe and the difficulty is at least normal, add the checking point to the store.
                if (!board.Player2.KingPiece.isSafe(ref thisPieces, new Point(KingPiece.X, KingPiece.Y)) && _difficulty <= (int)Difficulty.Normal)
                {
                    //Remove the points from the score since the player is being checked.
                    score += CHECKING_POINTS;
                }
            }

            //Returns the total score.
            return score;
        }

        /// <summary>
        /// Finds the best move and executes it on the board.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        public void ExecuteMoves(ref BoardModel board)
        {
            //Runs the minimax algorithmn to find the best piece.
            FindBestMove();

            //If the moved piece exists, move the piece.
            if (_movedPiece != null)
            {
                //Gets the piece's valid spaces.
                _movedPiece.GetValidSpaces(ref _board, true);

                //Makes the piece move on the board.
                board.Move(_movedPiece, _movedPoint.X, _movedPoint.Y, true);

                //Refreshes the server.
                board.Form.IsRefresh = true;
            }
        }
    }
}
