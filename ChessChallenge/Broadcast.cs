/*
 * Henry Gao
 * Broadcasts the server name and port
 * to other user's clients, so they
 * can connect to them.
 * Jan 24th, 2017.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ChessChallenge
{
    class Broadcast : Server
    {
        //Stores the name of the server.
        private string _storedName;
        
        //Stores the port of the server.
        private int _storedPort;

        /// <summary>
        /// Constructs a broadcaster given a name and port that it should store.
        /// </summary>
        /// <param name="storedName">The name of the server</param>
        /// <param name="storedPort">he port of the server</param>
        /// <param name="board">Reference to the local board</param>
        public Broadcast(string storedName, int storedPort, ref BoardModel board) : base(ref board, "Broadcaster")
        {
            //Stores the name of the server.
            _storedName = storedName;

            //Stores the port of the server.
            _storedPort = storedPort;
            
            //Stores the start and ending ranges for ports.
            portStart = 5000;
            portEnd = 5005;
        }

        /// <summary>
        /// Run by thread to repeatedly sends the server information to recievers and restarts itself.
        /// </summary>
        public override void Run()
        {
            //It keeps resetting itself, if the broadcaster is open.
            while (IsOpen)
            {
                //Connects to the reciever.
                Connect();

                //Sends the server information.
                SendServerName();

                //Disconnects the broadcaster, so it can reset itself after the loop restarts.
                Disconnect(true);
            }
        }

        /// <summary>
        /// Sends the server name to any reciever that connects to the broadcaster.
        /// </summary>
        public void SendServerName()
        {
            //Sends the server information to the reciever.
            Send(Protocol.SendServerName(_board, _storedName, _storedPort));
        }
    }
}
