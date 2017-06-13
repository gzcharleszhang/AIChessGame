using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessChallenge
{
    class Queen : Piece
    {
        public Queen(Color color) : base (PieceType.Queen, color)
        {
            //_image = Resources.;
        }

        public override void IsValid(MainForm frame, int x, int y)
        {

        }
    }
}
