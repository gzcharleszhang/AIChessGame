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
        private string _name;
        ArrayList pieces;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Player(string name)
        {
            pieces = new ArrayList();
        }
    }
}
