/*
 * Henry Gao
 * The server that sends and recieves data.
 * The server hosts the client, and uses 
 * modified functions from the base network.
 * Jan 24th, 2017
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace ChessChallenge
{
    class Server : Network
    {
        //Stores the server name, by default it is default server name.
        private string _serverName = "Default Server Name";

        //Stores the server that the client connects to.
        protected TcpListener _server;

        //Stores the broadcaster that informs other users the port and name of the server.
        private Broadcast _broadcaster;

        /// <summary>
        /// Constructs a server of a given name, on the first avaliable port.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="name">The name of the server</param>
        public Server(ref BoardModel board, string name) : base(ref board)
        {
            //Temporarily stores the reference to this network (can't call ref this).
            Network thisNetwork = this;
            _serverName = name;

            //Stores the range of ports the server can be run on.
            portStart = 2000;
            portEnd = 2005;

            //Stores that the server is authenicating (waiting for a client).
            _state = ServerState.Authenticating;

            //Creates a new protocol, to read and write information to the client.
            _protocol = new Protocol(ref thisNetwork);
            
            //Stores that this network is a server.
            _isServer = true;
        }

        /// <summary>
        /// Disconnects from the broadcaster, thread and networking objects.
        /// </summary>
        /// <param name="retry">Whether the server should try to reconnect</param>
        public override void Disconnect(bool retry)
        {
            //If the broadcaster is set, disconnect it.
            if (_broadcaster != null)
            {
                //Sets that it is not open.
                _broadcaster._isOpen = false;

                //Disconnects the broadcaster.
                _broadcaster.Disconnect(false);
            }

            //If this is not retrying, and the thread is set, then close it.
            if (!retry && _thread != null)
            {
                //Closes the thread.
                _thread.Abort();
            }

            //If the network stream is set, close it.
            if (_stream != null)
            {
                //Closes the stream.
                _stream.Close();
            }

            //If the client is set, disconnect it.
            if (_client != null)
            {
                //Disconnects the client from the server.
                _client.Close();
            }

            //If the server is set, stop it.
            if (_server != null)
            {
                //Stops the server.
                _server.Stop();
            }
        }

        /// <summary>
        /// Waits to connect to a client.
        /// </summary>
        /// <returns>Returns true if the server is connected</returns>
        public override bool Connect()
        {
            //Stores the host network's IP address.
            IPAddress lanIp = GetIp();

            //If the there is no found IP address, print a message for developers.
            if (lanIp == null)
            {
                //Prints the message.
                //Console.WriteLine("NO LAN IP FOUND - MAKE SURE YOU ARE CONNECTED TO A NETWORK");
            }

            //If there is a valid Ip address, try to find a port to connect to.
            else
            {
                //Loops from the starting port to the ending port, to see if they can be used.
                for (_port = portStart; _port <= portEnd; ++_port)
                {
                    //Catches socket exceptions, when the port is not avaliable.
                    try
                    {

                        //Creates and starts a new server on the ip address and port.
                        _server = new TcpListener(lanIp, _port);
                        _server.Start();

                        //Makes the form display the server ip and port, so others can directly connect to them.
                        _board.Form.IsDisplayServer = true;

                        //Displays the ip address.
                        _board.Form.Name = lanIp.ToString();

                        //Displays the port (the broadcasting port - 3000 is the server port).
                        _board.Form.Port = _port - 3000;

                        //If this is not a broadcaster, then create one.
                        if (!(this is Broadcast))
                        {
                            //If this server has a broadcaster, disconnect it.
                            if (_broadcaster != null)
                            {
                                //Disconnects the broadcaster and discards the reference.
                                _broadcaster.Disconnect(false);
                                _broadcaster = null;
                            }

                            //Creates a new broadcaster with the specified port and name.
                            _broadcaster = new Broadcast(_serverName, _port, ref _board);

                        }

                        //When the client connects to it, it will store the client as the connected client.
                        _client = _server.AcceptTcpClient();


                        //Stores the stream that allows the server to communicate with the client.
                        _stream = _client.GetStream();


                        //Clears the prompt in the form, since it doesn't need the server info anymore.
                        _board.Form.IsErasePrompt = true;

                        //Stores that the board's network is already connected.
                        _board.isConnected = true;

                        //If the server is not a broadcaster, flip a coin and send the result to the client.
                        if (!(this is Broadcast))
                        {
                            //Sends the coin flip command to the client.
                            SendCoinFlip();
                        }

                        //Returns that the server is connected.
                        return true;
                    }
                    //Catches an exception, meaning that the port is unavaliable.
                    catch (Exception e)
                    {
                        //Prints a message for the developer that the port cannot be used.
                        //Console.WriteLine(port + " is unavaliable...");
                    }
                }
            }

            //Returns that the server is not connected.
            return false;
        }

        /// <summary>
        /// Connects to the client and repeatedly alternating between sending and waiting for commands.
        /// </summary>
        public override void Run()
        {
            //Connects to a client.
            Connect();

            //Sees if the client has disconnected.
            try
            {
                while (IsOpen)
                {

                    //Waits for a message.
                    byte[] msg = Listen();


                    //Processes the message and performs the correct action.
                    _protocol.ProcessInput(ref _board, msg);

                }
            }

            //If an error occurs, the other player disconnected so this player wins.
            catch (Exception e)
            {
                //Disconnects the server.
                Disconnect(false);
            }
        }
    }
}
