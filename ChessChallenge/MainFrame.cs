using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace ChessChallenge
{
    public partial class MainFrame : Form
    {
        const bool PLAYER_1 = true, PLAYER_2 = false;

        bool currentTurn = PLAYER_1;

        public const int MAX_LENGTH = 8;

        Player p1, p2;

        RectangleF[,] boardGrids = new RectangleF[MAX_LENGTH, MAX_LENGTH];
        SizeF gridSize = new SizeF();
        PointF[,] gridPoints = new PointF[MAX_LENGTH, MAX_LENGTH];

        public MainFrame()
        {
            InitializeComponent();
            SetupDrawing();

            p1 = new Player("Comp Sci Club");
            p2 = new Player("Chess Club");
        }

        public Player CurrentPlayer
        {
            get
            {
                if (currentTurn == PLAYER_1)
                {
                    return p1;
                }
                else
                {
                    return p2;
                }
            }
        }

        public void SetupDrawing()
        {
            gridSize.Height = ClientSize.Height / 8;
            gridSize.Width = gridSize.Height;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    gridPoints[i, j] = new PointF(i * gridSize.Height, j * gridSize.Width);
                    boardGrids[i, j] = new RectangleF(gridPoints[i, j], gridSize);
                }
            }
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Set up the board grids
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i+j)%2 == 0)
                    {
                        e.Graphics.FillRectangle(Brushes.Black, boardGrids[i, j]);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.Gray, boardGrids[i, j]);
                    }
                }
            }
        }
    }
}
