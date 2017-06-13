using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;

namespace ChessChallenge
{
    abstract class Piece
    {
        protected Image _image;

        protected PieceType _type;
        protected Color _color;

        ArrayList validSpaces;

        protected int _x, _y;

        public Piece(PieceType type, Color color)
        {
            validSpaces = new ArrayList();

            _type = type;
            _color = color;
        }

        public void GetValidSpaces(MainForm frame)
        {
            for (int y = 0;y < MainForm.MAX_LENGTH;++y)
            {
                for (int x = 0; x < MainForm.MAX_LENGTH; ++x)
                {
                    
                }
            }
        }

        public void Move(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public abstract void IsValid(MainForm frame, int x, int y);
    }
}
