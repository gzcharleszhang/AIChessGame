using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class Pawn : Piece
    {
        public Pawn(Color color) : base (PieceType.Pawn, color)
        {
            //_image = Resources.;
        }

        public override void IsValid(MainFrame frame, int x, int y)
        {

        }
    }
}
