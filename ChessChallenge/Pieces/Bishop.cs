using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class Bishop : Piece
    {
        public Bishop(Color color) : base (PieceType.Bishop, color)
        {
            //_image = Resources.;
        }

        public override void IsValid(MainFrame frame, int x, int y)
        {

        }
    }
}
