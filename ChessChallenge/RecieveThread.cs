/*
 * Henry Gao
 * Goes though a specific IP address, and searches all the possible ports
 * on that IP address for a broadcaster. Does not inherit network, because 
 * it CREATES clients and servers, it is NOT a client or server.
 * Jan 24th, 2017.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class RecieveThread
    {
        //Stores the thread that searches the possible port range on the IP for a broadcaster.
        private Thread _thread;

        /// <summary>
        /// Allows thread to be visible to other classes
        /// </summary>
        public Thread Thread
        {
            get
            {
                return _thread;
            }
            // can be changed externally
            set
            {
                _thread = value;
            }
        }

        //Reference to the list of server info from the reciever, stores the found servers.
        private List<ServerInfo> _list;

        /// <summary>
        /// Allows list to be visible to other classes
        /// </summary>
        public List<ServerInfo> List
        {
            get
            {
                return _list;
            }
            // can be changed externally
            set
            {
                _list = value;
            }
        }

        //Stores the buffer size.
        private const int _BUFFER_SIZE = 1024;

        //Stores the port start, end and subnet start and end ranges.
        private int _portStart;
        private int _portEnd;
        private int _subnetStart;
        private int _subnetEnd;

        //Stores the IP address.
        private string _ipAddress;

        //Stores a reference to the local board model.
        private BoardModel _board;

        /// <summary>
        /// Constructs a thread that searches for a broadcaster
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        /// <param name="list">Reference to the list of server info</param>
        /// <param name="address">The IP address of that will be searched</param>
        /// <param name="subnetStart">The starting number of the subnet</param>
        /// <param name="subnetEnd">The ending number of the subnet</param>
        /// <param name="portStart">The starting port</param>
        /// <param name="portEnd">The ending port</param>
        public RecieveThread(ref BoardModel board, ref List<ServerInfo> list, string address, int subnetStart, int subnetEnd, int portStart, int portEnd)
        {
            //Stores the reference of the local board odel.
            _board = board;

            //Stores the desired IP address.
            _ipAddress = address;

            //Stores the range of subnet numbers to be searched.
            this._subnetStart = subnetStart;
            this._subnetEnd = subnetEnd;

            //Stores the range of ports to be searched.
            this._portStart = portStart;
            this._portEnd = portEnd;

            //Stores the list to put found servers into.
            this.List = list;

            //Creates a new thread and starts it.
            _thread = new Thread(new ThreadStart(Run));
            _thread.Start();
        }

        /// <summary>
        /// Searches through the specified IP address, and all ports.
        /// </summary>
        public void Run()
        {
            //Loops from the beginning of the subnet to the end of the subnet, stores the current IP at each subnet.
            for (int currIp = _subnetStart; currIp <= _subnetEnd; ++currIp)
            {
                //The complete LAN ip address that will be searched.
                string lanIp = _ipAddress.Substring(0, _ipAddress.LastIndexOf('.')) + "." + currIp;

                //Loops through the starting to ending ports, and temporarily stores the port with each loop as currPort.
                for (int currPort = _portStart; currPort <= _portEnd; ++currPort)
                {
                    //If there is no exception, the client found a reply within the alloted time.
                    try
                    {
                        //If the server list is full, then stop searching.
                        if (List.Count >= 5)
                        {
                            return;
                        }

                        //Creates a temporary client at the given IP and port.
                        TcpClient tempClient = new TcpClient(lanIp, currPort);

                        //Set the client to disconnect at 2 seconds without a reply.
                        tempClient.ReceiveTimeout = 2000;

                        //Stores the network stream between the broadcaster and recieveThread.
                        NetworkStream stream = tempClient.GetStream();

                        //Stores the message containing server information by the broadcaster.
                        byte[] msg = new byte[_BUFFER_SIZE];

                        //Reads a message from the broadcaster.
                        stream.Read(msg, 0, _BUFFER_SIZE);

                        //Converts the character array to a readable string.
                        String str = System.Text.Encoding.UTF8.GetString(msg).TrimEnd('\0');

                        //Splits the string by '~', the separating character. This gives the server info.
                        string[] args = str.Split(new char[] { '~' });

                        //Stores the name of the server.
                        string name = args[1];

                        //Stores the port of the server.
                        int port = -1;

                        //Converts the port from the server message into an integer.
                        int.TryParse(args[2], out port);

                        //If the port is within the valid range for a server, send it.
                        if (port >= 2000 && port <= 2005)
                        {
                            //If the list is not full, add the information to the form.
                            if (List.Count < 5)
                            {
                                //Adds the name and port to the form.
                                List.Add(new ServerInfo(_board.Form, List.Count, port, name, lanIp));
                            }
                        }

                        //Closes the network stream.
                        stream.Close();

                        //Closes the client.
                        tempClient.Close();
                    }
                    //If there is an exception, then it could not connect to the broadcaster.
                    catch (Exception e)
                    {
                        //Console.WriteLine("No valid response at " + lanIp  + " " + currPort);
                    }
                }
            }
        }
    }
}
