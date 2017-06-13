using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class Knight : Piece
    {
        public Knight(Color color) : base (PieceType.Knight, color)
        {
            //_image = Resources.;
        }

        public override void IsValid(MainFrame frame, int x, int y)
        {

        }
    }
}
