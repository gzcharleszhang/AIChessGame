/*
 * Henry Gao
 * The protocol that the server and client follows to
 * send and recieve messages from each other.
 * Jan 24th, 2017
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class Protocol
    {
        //Stores the network that is using the protocol.
        private Network _network;

        /// <summary>
        /// Creates a protocol with the network that is using the protocol.
        /// </summary>
        /// <param name="network">The network that is using the protocol</param>
        public Protocol(ref Network network)
        {
            //Stores the network.
            _network = network;
        }

        /// <summary>
        /// Processes the raw input from the other server, and performs the cooresponding action.
        /// </summary>
        /// <param name="board">The local board model</param>
        /// <param name="data">The byte of data that is being processed</param>
        public void ProcessInput(ref BoardModel board, byte[] data)
        {

            //If the the packet contains information, begin processing it.
            if (data.Length > 0)
            {
                //Stores the type of command from the message, from the first byte.
                char command = (char)data[0];

                //If the other server is sending a coin flip.
                if (command == 'c')
                {
                    /*Stores the results of the coin flip, by subtracting characters '0' from '1' or '0.' 
                    This makes the integer 0 or 1, respectively.*/
                    int value = (int)data[1] - '0';

                    //If the value is 0, this network is going first.
                    if (value == 0)
                    {
                        //The network can send data.
                        _network.State = ServerState.Active;
                    }

                    //If the value is 1, this network is going second.
                    else
                    {
                        //This network will wait for data.
                        _network.State = ServerState.Waiting;
                    }

                    //Visually shows a coin flip on the form.
                    board.Form.FlipCoin(value);
                }

                //If the command is move or castle.
                else if (command == 'm' || command == 'k')
                {

                    //Stores the starting and ending x, y positions of the piece that moved.
                    int startX = data[1], startY = data[2], endX = data[3], endY = data[4];

                    //Stores whether the piece moved or not before.
                    bool hasMoved = Convert.ToBoolean(data[5]);

                    //If the data has been sent to the wrong server (the piece already moved locally), resend it.
                    if (board.Pieces[startX, startY] == null)
                    {
                        //If it is a move command, send a move message.
                        if (command == 'm')
                        {
                            //Sends the same move message to the other server.
                            SendMove(board, startX, startY, endX, endY, hasMoved);
                        }

                        //If it is a castle command, send a castle message.
                        else
                        {
                            //Sends the same castle message to the other server.
                            SendCastle(board, startX, startY, endX, endY, hasMoved);
                        }

                        //Exits this method.
                        return;
                    }

                    //Stores the selected piece, and obtain a list of valid spaces for it to move.
                    board.SelectedPiece = board.Pieces[startX, startY];
                    board.Pieces[startX, startY].GetValidSpaces(ref board, true);

                    //If it's a move command, move the piece.
                    if (command == 'm')
                    {
                        //Move the piece to the desired location.
                        board.Move(board.Pieces[startX, startY], endX, endY, true);
                    }
                    
                    //If it's a castle command, simulate the castling.
                    else
                    {
                        //Moves the pieces to castle to the desired location.
                        board.DoCastle(board.Pieces[startX, startY], board.Pieces[endX, endY]);
                    }

                    //Deselects the piece, now that it has been moved.
                    board.SelectedPiece = null;

                    //Stores that this network can sending data.
                    _network.State = ServerState.Active;
                }
                else if (command == 'p')
                {
                    //Stores the x and y location of the piece to be promoted, and the type of chess piece.
                    int x = data[1], y = data[2], type = data[3];

                    //If the piece at that position is empty, then send the promote command again (sent to wrong server).
                    if (board.Pieces[x, y] == null)
                    {
                        //Sends the same command to the other network.
                        SendPawnPromote(board, x, y, type);

                        //Exits the method.
                        return;
                    }

                    //By default, promote the piece to a queen.
                    Piece piece = new Queen(board.Pieces[x, y].Owner, x, y);

                    //If the user wants to make a queen, store that the piece is a queen.
                    if (type == (int)PieceType.Queen)
                    {
                        //stores a queen piece.
                        piece = new Queen(board.Pieces[x, y].Owner, x, y);
                    }

                    //If the user wants to make a queen, store that the piece is a rook.
                    else if (type == (int)PieceType.Rook)
                    {
                        //stores a rook piece.
                        piece = new Rook(board.Pieces[x, y].Owner, x, y);
                    }

                    //If the user wants to make a queen, store that the piece is a bishop.
                    else if (type == (int)PieceType.Bishop)
                    {
                        //stores a bishop piece.
                        piece = new Bishop(board.Pieces[x, y].Owner, x, y);
                    }

                    //If the user wants to make a queen, store that the piece is a knight.
                    else if (type == (int)PieceType.Knight)
                    {
                        //stores a knight piece.
                        piece = new Knight(board.Pieces[x, y].Owner, x, y);
                    }

                    //Promotes the pawn to the desired piece.
                    board.PromotePawn(piece);

                    //Stores that this network can send data.
                    _network.State = ServerState.Active;
                }

                //If it is a forfeit command, this player wins.
                else if (command == 'f')
                {
                    //If the forfeit command is from the same player, resend the command.
                    if (data[1] == (byte)(board._player))
                    {
                        //Sends the same command to the other network.
                        SendForfeit(board);
                    }

                    //Displays the win screen (can't access directly from another thread).
                    board.Form.IsDisplayWin = true;
                }
            }

        }

        /// <summary>
        /// Sends the data containing the server name.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="name">The name of the server</param>
        /// <param name="port">The port that the server is located on</param>
        /// <returns></returns>
        public byte[] SendServerName(BoardModel board, string name, int port)
        {
            //The string containing the name and port seperated by a separator character.
            String str = "n~" + name + "~" + port;

            //The byte array containing the message to be sent.
            byte[] msg = new byte[Network.BUFFER_SIZE];

            //The temporary character array of the string, to be converted to bytes.
            char[] temp = str.ToCharArray();

            //Loops from 0 to the buffer size, to convert the characters to bytes.
            for (int i = 0;i < Network.BUFFER_SIZE;++i)
            {
                //If there is a character at index i.
                if (i < temp.Length)
                {
                    //Stores the character as a byte.
                    msg[i] = (byte)temp[i];
                }
            }

            //Sends the message to the other network.
            _network.Send(msg);

            //Returns the message
            return msg;
        }

        /// <summary>
        /// Sends a forfeit command to the other network (this player loses).
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <returns>Returns the message as a byte array</returns>
        public byte[] SendForfeit(BoardModel board)
        {
            //Stores the message as a byte array.
            byte[] msg = new byte[Network.BUFFER_SIZE];

            //Stores the command in the first byte, and 0 or 1 to represent which player forfeits.
            msg[0] = (byte)'f';
            msg[1] = (byte)board._player;

            //Returns the message
            return msg;
        }

        /// <summary>
        /// Sends a move command to the other network, moving the desired piece to a desired location.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="startX">The starting x location</param>
        /// <param name="startY">The starting y location</param>
        /// <param name="endX">The ending x location</param>
        /// <param name="endY">The ending y location</param>
        /// <param name="hasMoved">Whether the piece has moved before or not</param>
        /// <returns>Returns the byte array containing the command</returns>
        public byte[] SendMove(BoardModel board, int startX, int startY, int endX, int endY, bool hasMoved)
        {
            //Stores the message as a byte array.
            byte[] msg = new byte[Network.BUFFER_SIZE];

            //Stores the command name, startX, startY, endX, endY and hasMoved in the respective order of bytes.
            msg[0] = (byte)'m';
            msg[1] = (byte)startX;
            msg[2] = (byte)startY;
            msg[3] = (byte)endX;
            msg[4] = (byte)endY;
            msg[5] = Convert.ToByte(hasMoved);

            //Returns the message.
            return msg;
        }

        /// <summary>
        /// Sends the castle command to the other network.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="startX">The starting x location</param>
        /// <param name="startY">The starting y location</param>
        /// <param name="endX">The ending x location</param>
        /// <param name="endY">The ending y location</param>
        /// <param name="hasMoved">Whether the piece has moved before or not</param>
        /// <returns>Returns the byte array containing the command</returns>
        public byte[] SendCastle(BoardModel board, int startX, int startY, int endX, int endY, bool hasMoved)
        {
            //Stores the message as a byte array.
            byte[] msg = new byte[Network.BUFFER_SIZE];

            //Stores the command name, startX, startY, endX, endY and hasMoved in the respective order of bytes.
            msg[0] = (byte)'k';
            msg[1] = (byte)startX;
            msg[2] = (byte)startY;
            msg[3] = (byte)endX;
            msg[4] = (byte)endY;
            msg[5] = Convert.ToByte(hasMoved);

            //Returns the message.
            return msg;
        }

        /// <summary>
        /// Sends the pawn promotion command to the other network.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="startX">The starting x location</param>
        /// <param name="startY">The starting y location</param>
        /// <param name="type">Stores the type of the piece</param>
        /// <returns>Returns the byte array containing the command</returns>
        public byte[] SendPawnPromote(BoardModel board, int startX, int startY, int type)
        {
            //Stores the message as a byte array.
            byte[] msg = new byte[Network.BUFFER_SIZE];

            //Stores the command name, startX, startY and type of piece in the respective order of bytes.
            msg[0] = (byte)'p';
            msg[1] = (byte)startX;
            msg[2] = (byte)startY;
            msg[3] = (byte)type;

            //Returns the message.
            return msg;
        }

        /// <summary>
        /// Sends the results of the coin flip
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="result">The results of the coin flip</param>
        /// <returns>Returns the message as a byte array</returns>
        public byte[] SendCoinFlip(BoardModel board, int result)
        {
            //Stores the message as a byte array.
            byte[] msg = new byte[Network.BUFFER_SIZE];

            //Stores the command name in the first byte.
            msg[0] = (byte)'c';
            
            //Stores the opposite result of the coin flip (because they are the other player). 0 becomes 1, 1 becomes 0.
            msg[1] = (byte)(((result + 1) % 2) + '0');

            //Returns the message.
            return msg;
        }
    }
}
