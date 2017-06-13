/*
 * Henry Gao
 * Connects to broadcaster and updates the server list in the form.
 * Does NOT inherit from network, because it's very different and this would require
 * a lot of overhead.
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
using System.IO;

namespace ChessChallenge
{
    class Reciever
    {
        //Stores the range of ports that needs to be searched for broadcasters.
        protected int portStart;
        protected int portEnd;

        //Stores the buffer size.
        private const int _BUFFER_SIZE = 1024;
        // Allows the buffer size to be visible to other classes
        public int BufferSize
        {
            get
            {
                return _BUFFER_SIZE;
            }
        }

        //Stores the ip of the LAN network.
        private IPAddress _lanIp = null;
        /// <summary>
        /// Allows LAN IP to be visible to other clases
        /// </summary>
        public IPAddress LanIp
        {
            get
            {
                return LanIp;
            }
        }

        //Stores a reference to the local board model.
        private BoardModel _board;
        /// <summary>
        /// Allows board to be visible to other classes
        /// </summary>
        public BoardModel Board
        {
            get
            {
                return _board;
            }
        }

        /// <summary>
        /// Constructs a reciever that searches the default port ranges for broadcasters.
        /// </summary>
        /// <param name="board">Reference to the local board model</param>
        public Reciever(BoardModel board)
        {
            //Stores the local IP addresses.
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            //Stores the defaul port ranges to find broadcasters.
            portStart = 5000;
            portEnd = 5005;

            //Loops through the local IP addresses, until it finds an IPv4 address.
            for (int i = 0; i < localIPs.Length; ++i)
            {
                //If the IPv4 address matches a local IP, then it is the LAN ip.
                if (localIPs[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    //Stores the lanIP.
                    _lanIp = localIPs[i];
                    break;
                }
            }

            //Stores the reference to the board model.
            _board = board;
        }

        /// <summary>
        /// Searches for open servers on a network on the same subnet.
        /// </summary>
        /// <param name="numPorts">The number of ports that is being searched</param>
        /// <returns>A list of server information of open servers</returns>
        public List<ServerInfo> SearchServers(int numPorts)
        {
            //Creates an empty list of server information.
            List<ServerInfo> list = new List<ServerInfo>();

            /*Creates an element in an array for reach recieve threads, so the information 
              they provide can be accessed after they are done processing.*/
            RecieveThread[] recieveThreads = new RecieveThread[numPorts];

            //If the user is searching for 1 port, make them search for the local port.
            if (numPorts == 1)
            {
                //Stores the lanIP as a string.
                string str = _lanIp.ToString();

                //The device number on the network.
                int value = -1;

                //Converts the IP address's last 'section' into an integer.
                int.TryParse(str.Substring(str.LastIndexOf('.') + 1, str.Length - 1 - str.LastIndexOf('.')), out value);


                //Creates a recieve thread to find the ports on the local IP address.
                recieveThreads[0] = new RecieveThread(ref _board, ref list, _lanIp.ToString(), value, value, portStart, portEnd);
            }
            //If the user is searching for a range of ports, they have multiple threads.
            else
            {
                //Loops once for each recieve thread, and creates said recieve thread. I is the device number.
                for (int i = 0; i < recieveThreads.Length; ++i)
                {
                    //Creates a recieve thread that searches if there is a server on specific device number in the network.
                    recieveThreads[i] = new RecieveThread(ref _board, ref list, _lanIp.ToString(), i, i, portStart, portEnd);
                }
            }

            //Loops once for each recieve thread created, and checks to see if it's still active.
            for (int i = 0; i < numPorts; ++i)
            {
                //If the thread is active, wait and check if it's active 10 ms later.
                while (recieveThreads[i].Thread.IsAlive)
                {
                    //The thread waits for 10 ms.
                    System.Threading.Thread.Sleep(10);
                }
            }

            //Returns the list of server info.
            return list;
        }
    }
}
