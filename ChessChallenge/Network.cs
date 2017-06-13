/*
 * Henry Gao
 * The basic network that the server, client and broadcaster is based on.
 * Provides the basic functions that allows the server and client to communicate.
 * Jan 24th, 2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace ChessChallenge
{
    abstract class Network
    {
        //Stores the fixed size of each message, default set to 1kb.
        public const int BUFFER_SIZE = 1024;

        //Stores the port that the server is run on.
        protected int _port;

        //Stores whether the server is open or not.
        protected bool _isOpen;

        //Stores whether this network is a server or client.
        protected bool _isServer;

        //Stores the thread that the server runs on.
        protected Thread _thread;

        //Stores the reference to the board model.
        protected BoardModel _board;

        //Stores whether the server is autenticating, sending or recieving data.
        protected ServerState _state;

        //Stores the client that connects to the server
        protected TcpClient _client;

        //Stores the stream that data is transferred.
        protected NetworkStream _stream;

        //Stores the rules for sending and processing data.
        protected Protocol _protocol;

        //Stores the range of ports the server can be hosted on.
        protected int portStart;
        protected int portEnd;

        //Accessor and mutator to the port of the network
        public int Port
        {
            //Returns what the current port is
            get
            {
                return _port;
            }
            //Sets the current port to the desired value
            set
            {
                _port = value;
            }
        }

        //Accessor to is open, as work may be split.
        protected bool IsOpen
        {
            get
            { 
                //Returns whether the server is open or not.
                return _isOpen;
            }
        }

        //Accessor to is server, as work may be split.
        protected bool IsServer
        {
            get
            {
                //Returns whether the network is a server or client.
                return _isServer;
            }
        }

        //Accessor and mutator to the server state, allows it to be updated when player moves.
        public ServerState State
        {
            get
            {
                //Returns the server state
                return _state;
            }
            set
            {
                //Sets the server state to the desired value
                _state = value;
            }
        }

        //Accessor to the client
        protected TcpClient Client
        {
            get
            {
                //Returns the reference to the client.
                return _client;
            }
        }

        //Accessor to the network stream.
        protected NetworkStream Stream
        {
            get
            {
                //Returns the network stream.
                return _stream;
            }
        }

        //Accessor to the protocol
        protected Protocol Protocol
        {
            get
            {
                //Returns the protocol
                return _protocol;
            }
        }

        /// <summary>
        /// Constructs a network that runs on a separate thread.
        /// </summary>
        /// <param name="board">The local board model</param>
        public Network(ref BoardModel board)
        {
            //Stores the board that stores the local data.
            _board = board;

            //Stores that the server is open
            _isOpen = true;

            //Creates a new thread that runs the method "Run"
            _thread = new Thread(new ThreadStart(Run));

            //Starts running the thread.
            _thread.Start();
        }

        /// <summary>
        /// Disconnects from the threads and all networking classes.
        /// </summary>
        /// <param name="retry">Stores whether the network should try to reconnect</param>
        public virtual void Disconnect(bool retry)
        {
            //If the thread is set, disconnect from it.
            if (_thread != null)
            {
                //Stops the thread.
                _thread.Abort();
            }

            //If the stream is not resetting and is set, close it.
            if (!retry && _stream != null)
            {
                //Closes the stream.
                _stream.Close();
            }

            //If the client is set, disconnect it from the server.
            if (_client != null)
            {
                //Disconnects the client.
                _client.Close();
            }
        }

        /// <summary>
        /// The method that is run on the thread.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Connects to another valid network
        /// </summary>
        /// <returns>Returns true if the connection was successful</returns>
        public abstract bool Connect();

        /// <summary>
        /// Reads one single "packet" of unprocessed data from the other network~
        /// </summary>
        /// <returns>Returns the bytes of data that is send.</returns>
        public byte[] Listen()
        {
            //The buffer that stores the raw data that is recieved.
            byte[] buffer = new byte[BUFFER_SIZE];

            //Reads the raw data and stores it in the buffer.
            Stream.Read(buffer, 0, BUFFER_SIZE);

            //Returns the bytes of raw data.
            return buffer;
        }

        /// <summary>
        /// Writes one single "packet" of unprocessed data from the other network~
        /// </summary>
        /// <param name="message">Bytes of data containing the command to be send to the other network</param>
        public void Send(byte[] message)
        {
            //Writes the bytes of data to the other network.
            Stream.Write(message, 0, BUFFER_SIZE);
        }

        /// <summary>
        /// Sends the command to forfeit the match to the other player.
        /// </summary>
        public void SendForfeit()
        {
            //Sends the forfeit command.
            Send(Protocol.SendForfeit(_board));
        }

        /// <summary>
        /// Sends which player should go first to the other player to the other network.
        /// </summary>
        public void SendCoinFlip()
        {
            //0 or 1 that stores which player should go first.
            int coinValue = _board.CoinFlip();

            //If the value is 0, then this network should go first.
            if (coinValue == 0)
            {
                _state = ServerState.Active;
            }

            //If the value is 1, then this network should go second.
            else
            {
                _state = ServerState.Waiting;
            }

            //Sends the results of the coin flip to the other network.
            Send(Protocol.SendCoinFlip(_board, coinValue));
        }

        /// <summary>
        /// Sends which piece a pawn is being promoted to to the other network.
        /// </summary>
        /// <param name="piece">The piece that the pawn becomes.</param>
        public void SendPawnPromote(Piece piece)
        {
            //Sends the type of piece to the other server.
            Send(Protocol.SendPawnPromote(_board, piece.X, piece.Y, (int)piece.Type));
        }

        /// <summary>
        /// Sends the a chess piece's move to the other network.
        /// </summary>
        /// <param name="startX">The starting X position of the chess piece</param>
        /// <param name="startY">The starting Y position of the chess piece</param>
        /// <param name="endX">The starting X position of the chess piece</param>
        /// <param name="endY">The starting Y position of the chess piece></param>
        /// <param name="hasMoved">Whether the piece has moved before or not</param>
        public void SendMove(int startX, int startY, int endX, int endY, bool hasMoved)
        {
            //Sends the movement of a piece to the other network.
            Send(Protocol.SendMove(_board, startX, startY, endX, endY, hasMoved));
        }

        /// <summary>
        /// Sends that the player is castling to the other network.
        /// </summary>
        /// <param name="startX">The starting X position of the chess piece</param>
        /// <param name="startY">The starting Y position of the chess piece</param>
        /// <param name="endX">The starting X position of the chess piece</param>
        /// <param name="endY">The starting Y position of the chess piece></param>
        /// <param name="hasMoved">Whether the piece has moved before or not</param>
        public void SendCastle(int startX, int startY, int endX, int endY, bool hasMoved)
        {
            //Sends that the player is castling to the other network.
            Send(Protocol.SendCastle(_board, startX, startY, endX, endY, hasMoved));
        }

        /// <summary>
        /// Gets the IP address that the network is hosted on.
        /// </summary>
        /// <returns>Returns the ip address of the host network.</returns>
        protected IPAddress GetIp()
        {
            //The list of local IP addresses.
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            //Loops through the local Ip addresses, until it finds the IPv4 address.
            for (int i = 0; i < localIPs.Length; ++i)
            {
                //If the ip address at index i is an IPv4 address, return it.
                if (localIPs[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    //Returns the local IP address.
                    return localIPs[i];
                }
            }

            //Returns null, if none are suitable.
            return null;
        }
    }
}
