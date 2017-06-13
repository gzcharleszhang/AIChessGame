using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class King : Piece
    {
        public King(Color color) : base (PieceType.King, color)
        {
            //_image = Resources.;
        }

        public override void IsValid(MainForm frame, int x, int y)
        {

        }
    }
}
