/*
 * Henry Gao
 * The client that connects to the server
 * reads and writes to the server, to perform commands.
 * Jan 24th, 2017.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;

namespace ChessChallenge
{
    class Client : Network
    {
        //Stores the ip of the server (without searching).
        private string _directIp = null;

        //Accessor to the directIP of the server
        public string DirectIP
        {
            //Returns the directIP
            get
            {
                return _directIp;
            }
            //Sets the direct IP to the desired value
            set
            {
                _directIp = value;
            }
        }

        /// <summary>
        /// Constructs a client with a given port that searches for a suitable IP address.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="port">The port that the server is on</param>
        public Client(ref BoardModel board, int port) : base(ref board)
        {
            //Stores this network (since you can't do ref this).
            Network thisNetwork = this;

            //Stores the starting and ending ports.
            portStart = 5000;
            portEnd = 5005;

            //Stores the port of the server.
            this._port = port;

            //Stores that the client is now authenticating.
            _state = ServerState.Authenticating;

            //Stores the protocol that the client uses to communicate with the server.
            _protocol = new Protocol(ref thisNetwork);

            //Stores that this network is a client.
            _isServer = false;
        }

        /// <summary>
        /// Constructs a client that connects to a specific IP address.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="ip">The ip of the server</param>
        /// <param name="port">The port that the server is on</param>
        public Client(ref BoardModel board, string ip, int port) : this(ref board, port)
        {
            //Stores the ip of the server
            _directIp = ip;
        }

        /// <summary>
        /// Connects to a server on a specific port.
        /// </summary>
        /// <returns>Returns true if the server can connect</returns>
        public override bool Connect()
        {
            //Stores the IP address of the local network.
            IPAddress lanIp = GetIp();

            //If the user is not connected to the internet.
            if (lanIp == null)
            {
                //Console.WriteLine("NO LAN IP FOUND - MAKE SURE YOU ARE CONNECTED TO A NETWORK");
            }

            //If the user is connected to the internet.
            else
            {

                //If there is no direct IP, then use the lanIP.
                if (_directIp == null)
                {
                    //Creates a client using the lan IP and port.
                    _client = new TcpClient(lanIp.ToString(), _port);
                }

                //If there is a direct IP, then connect to the specified IP.
                else
                {
                    //Creates a client using the direct IP and port.
                    _client = new TcpClient(_directIp, _port);
                }


                //Obtains the network stream.
                _stream = _client.GetStream();

                //Stores that the client is connected.
                _board.isConnected = true;

                //Returns that the client is connected.
                return true;
            }

            //Returns that the client is not connected.
            return false;
        }

        /// <summary>
        /// Connects to the client and repeatedly alternating between sending and waiting for commands.
        /// </summary>
        public override void Run()
        {
            //Connects to the server.
            Connect();

            //Processes the coin flip.
            _protocol.ProcessInput(ref _board, Listen());

            //If an error occurs, then the opponent disconnected.
            try
            {
                //While the client is open, alternate between sending and receiving inputs.
                while (IsOpen)
                {

                    //Waits for a message from the server.
                    byte[] msg = Listen();


                    //Processes the input and does the correct action.
                    _protocol.ProcessInput(ref _board, msg);

                }
            }
            //If an error occurs, then the opponent disconnected, so display a win screen.
            catch (Exception e)
            {
                //Disconnects the client from the server.
                Disconnect(false);
            }
        }
    }
}
