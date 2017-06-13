/*
 * Henry Gao.
 * Stores the information associated with
 * a server, that was found by a reciever.
 * Jan 24th, 2017.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ChessChallenge
{
    public class ServerInfo
    {
        //Stores the server's port.
        private int _port;

        //Public accessor to the server port.
        public int Port
        {
            get
            {
                //Returns the server port.
                return _port;
            }
        }

        //Stores the server's name.
        private string _name;

        //Public accessor to the server name.
        public string Name
        {
            get
            {
                //Returns the server name.
                return _name;
            }
        }

        //Stores the server's IP.
        private string _ip;

        //Public accessor to the server IP.
        public string Ip
        {
            get
            {
                //Returns the server IP.
                return _ip;
            }
        }

        /// <summary>
        /// Constructs a new server info object, with the given port, name and ip addresses of the server.
        /// </summary>
        /// <param name="form">The form that the informtion will be shown in</param>
        /// <param name="numFound">The number of servers found in total</param>
        /// <param name="port">The port number of the server</param>
        /// <param name="name">The name of the server</param>
        /// <param name="ip">The Ip address of the server</param>
        public ServerInfo(View form, int numFound, int port, string name, string ip)
        {
            //Stores the server port.
            _port = port;

            //Stores the server name.
            _name = name;

            //Stores the server IP.
            _ip = ip;

            //Displays the name of the server on the form, and refreshes it.
            form.NumFound = numFound;
            form.LblName = name;
            form.IsLblUpdate = true;
        }
    }
}
