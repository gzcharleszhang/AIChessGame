using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ChessChallenge
{
    class Rook : Piece
    {
        public Rook(Color color) : base (PieceType.Rook, color)
        {
            //_image = Resources.;
        }

        public override void IsValid(MainFrame frame, int x, int y)
        {

        }
    }
}
