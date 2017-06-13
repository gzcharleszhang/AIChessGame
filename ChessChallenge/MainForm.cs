/* Nemo Liu, Charles Zhang, and Henry Gao
 * Competitive Chess Challenge Culminating Program
 * The main form where all the graphics are drawn and all the functions are performed
 * Due: January 25, 2017
 */
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
using System.Media;
using System.IO;

namespace ChessChallenge
{
    public partial class View : Form
    {
        //Stores the display text to show that no server is found for multiplayer
        public const string SERVER_NOT_FOUND = "----------------------------------------No server found----------------------------------------";
        //Stores the size for the chess board, 8x8 chess board
        public const int GLOBAL_SIZE = 8;
        //Stores the flag to see if the player is cheating or not, this data is required for AI so it has a public accessor
        private bool _isCheating = false;
        public bool IsCheating
        {
            //Returns if the user is cheating or not
            get
            {
                return _isCheating;
            }
        }
        //Stores the flag to see if the user decides to restart
        private bool _isRestarting = false;
        //Creates a soundplayer to play music
        private SoundPlayer _bgMusic = new SoundPlayer();
        //Creates a board to play chess
        private BoardModel _board;
        //Sets a flag to stop game from starting
        private bool _isGameStarted = false;
        //Sets a flag that music has been started
        private bool _isMusicStarted = true;
        //Stores the pawn for a pawn promotion
        private Piece _pawn;
        //Stores the promoted piece after pawn promotion
        private Piece _promotedPiece;
        //Creates an actual 8x8 chessboard
        private RectangleF[,] _gameBoard = new RectangleF[GLOBAL_SIZE, GLOBAL_SIZE];
        //Creates points for the said chessboard
        private PointF[,] _gridPoints = new PointF[GLOBAL_SIZE, GLOBAL_SIZE];
        //Creates boundaries for pieces
        private RectangleF[,] _pieceBounds = new RectangleF[GLOBAL_SIZE, GLOBAL_SIZE];
        //Creates a new size for the board
        private SizeF _boardSize = new SizeF();
        //Creates 5 lables for the server names
        private Label[] _lblServerName = new Label[5];
        //Accessor to the server names
        public Label[] LblServerName
        {
            get
            {
                return _lblServerName;
            }
        }

        //Creates 5 buttons to join servers
        private Button[] _btnServerJoin = new Button[5];
        //Accessor to the button of joining a server
        public Button[] BtnServerJoin
        {
            get
            {
                return _btnServerJoin;
            }
        }
        //Stores to flag to see if the coin has been flipped
        private bool _isCoinFlipping = false;

        //flag to see if the victory screen has been shown, this data is required for server communications along with its public accessors and mutators
        private bool _isDisplayWin = false;
        public bool IsDisplayWin
        {
            //Returns if the victory screen is displayed
            get
            {
                return _isDisplayWin;
            }
            //Set the flag depending on if the victory screen is displayed or not
            set
            {
                _isDisplayWin = value;
            }
        }
        //if the prompt needs to be erased, this data is required for server communications along with its public accessors and mutators
        private bool _isErasePrompt = false;
        public bool IsErasePrompt
        {
            //Returns if the prompt is being erased or not
            get
            {
                return _isErasePrompt;
            }
            //Sets if the prompt is being erased
            set
            {
                _isErasePrompt = value;
            }
        }
        //If the server info needs to be displayed, this data is required for server communications along with its public accessors and mutators
        private bool _isDisplayServer = false;
        public bool IsDisplayServer
        {
            //Returns if the serverinfo needs to be displayed
            get
            {
                return _isDisplayServer;
            }
            //Sets if the server info needs to be displayed or not
            set
            {
                _isDisplayServer = value;
            }
        }

        //The default name for a server, this data is required for server communications along with its public accessors and mutators
        private string _name = "";
        public string Name
        {
            //Returns the name for the server
            get
            {
                return _name;
            }
            //Sets the name for the server to be a desired name
            set
            {
                _name = value;
            }
        }

        //The starting port for the server, this data is required for server communications along with its public accessors and mutators
        private int _port = 0;
        public int Port
        {
            //Returns the current port
            get
            {
                return _port;
            }
            //Sets the current port
            set
            {
                _port = value;
            }
        }

        //Sets the flag to see if the player has been defeated
        private bool _isDefeated = false;
        //flags to see if the player has extra time on their turn
        private bool _hasExtraTime = false;
        //If the pawn is being promoted, initially it is not so set it to false, this data is required for server communications along with its public accessors and mutators
        private bool _isPromoting = false;
        public bool IsPromoting
        {
            //Returns if the user is currently promoting a pawn
            get
            {
                return _isPromoting;
            }
            //Sets whether the user is promoting a pawn or not
            set
            {
                _isPromoting = value;
            }
        }

        //The flip value of the coin
        private int _flipValue;
        //If the board is redrawn/needed to be redrawn, this data is required for server communications along with its public accessors and mutators
        private bool _isRefresh = false;
        public bool IsRefresh
        {
            //Returns if the user is refreshing or not
            get
            {
                return _isRefresh;
            }
            //Sets if the user is refreshing or not
            set
            {
                _isRefresh = value;
            }
        }
        //Stores the lable of the name of server, this data is required for server communications along with its public accessors and mutators
        private string _lblName;
        public string LblName
        {
            //Returns the name of server lable
            get
            {
                return _lblName;
            }
            //Sets the name of server lable
            set
            {
                _lblName = value;
            }
        }

        //How many servers are found, this data is required for server communications along with its public accessors and mutators
        private int _numFound;
        public int NumFound
        {
            //Returns the number found
            get
            {
                return _numFound;
            }
            //Sets the number found
            set
            {
                _numFound = value;
            }
        }

        //If the server name needs to be updated, this data is required for server communications along with its public accessors and mutators
        private bool _isLblUpdate = false;
        public bool IsLblUpdate
        {
            //Returns the lable update
            get
            {
                return _isLblUpdate;
            }
            //Sets the lable to the desired value
            set
            {
                _isLblUpdate = value;
            }
        }

        public View()
        {
            //Creates a new board
            _board = new BoardModel(this);

            InitializeComponent();
            //Initializes board height and width as well as piece size and hitboxes
            SetupDrawing();
            //sets up the game with specific properties
            SetupForm();
            //Creates the multiplayer panel and fills it in with lables and buttons
            SetupMultiplayerPanel();
        }

        /// <summary>
        /// Subprogram to set up the multiplayer panel
        /// </summary>
        public void SetupMultiplayerPanel()
        {
            //Loops 5 times because there is 5 lables and 5 corresponding buttons
            for (int i = 0; i < 5; ++i)
            {
                //Creates a new white lable at a specific point and says the initial: server not found
                _lblServerName[i] = new Label();
                //The background colour of the lable is white
                _lblServerName[i].BackColor = System.Drawing.Color.White;
                //The lable is autosized
                _lblServerName[i].AutoSize = true;
                //Sets a new location of the lable, one after each other
                _lblServerName[i].Location = new Point(240, i * 50 + 124);
                //Sets the name of the lable
                _lblServerName[i].Name = "lblServerName";
                //Initializes a size for the lable
                _lblServerName[i].Size = new Size(350, 40);
                //Sets the tab index of the server
                _lblServerName[i].TabIndex = 8;
                //The initial display text of the lable prommpt
                _lblServerName[i].Text = SERVER_NOT_FOUND;
                //Add a new server with the server name
                pnlMultiplayer.Controls.Add(_lblServerName[i]);
                //Bring the server name lable to the front
                _lblServerName[i].BringToFront();
                //Creates a new button at a specific point with a specific size that allows one to join a server
                _btnServerJoin[i] = new Button();
                //Autosize the button
                _btnServerJoin[i].AutoSize = true;
                //Sets the location of the button to be beside the lable
                _btnServerJoin[i].Location = new Point(240 + 350, i * 50 + 120);
                //Reinitialize a name for the button
                _btnServerJoin[i].Name = "btnServerJoin" + i;
                //If the button is clicked, add the button
                _btnServerJoin[i].Click += btnJoin_Click;
                //Initialize the size of the button
                _btnServerJoin[i].Size = new Size(100, 14);
                //Initializes the tab index of the button to be 8
                _btnServerJoin[i].TabIndex = 8;
                //The button will display "Join"
                _btnServerJoin[i].Text = "Join";
                //Another button is added to the multiplayer panel
                pnlMultiplayer.Controls.Add(_btnServerJoin[i]);
                //brings the button to the front of the screen
                _btnServerJoin[i].BringToFront();
            }
        }

        /// <summary>
        /// Start a specific track
        /// </summary>
        /// <param name="theme">the location of a specific track</param>
        public void StartMusic(Stream theme)
        {
            //Stops the music
            _bgMusic.Stop();
            //Finds the source of the music
            _bgMusic.Stream = theme;
            //Plays music on loop
            _bgMusic.PlayLooping();
        }

        /// <summary>
        /// Sets up the main form for the game initially
        /// </summary>
        public void SetupForm()
        {
            MaximizeBox = false;

            //Brings the start panel to the front
            pnlStart.BringToFront();
            //Plays the classic theme song
            StartMusic((Stream)Properties.Resources.ResourceManager.GetObject("_" + (int)Theme.Classic));
            //Sets the board's theme to classic
            _board.Settings.Theme = Theme.Classic;
            //No prompt is written for the actual game
            lblPrompts.Text = null;
            //Hides the pawn promotion and transparent panels
            pnlPawnPromote.Hide();
            pnlTransparent.Hide();
            //Checks to see if there's a file called "savedNames" of previous saved games
            if(File.Exists("_savedNames.txt"))
            {
                //Reads the file with all of the saved names
                using (StreamReader savedGames = new StreamReader("_savedNames.txt"))
                {
                    //While there is something to read, add the saved name to the combo box
                    while (savedGames.Peek() > 0)
                    {
                        cboLoadList.Items.Add(savedGames.ReadLine());
                    }
                }
            }
            //Otherwise there are no save files existing, disable the combobox
            else
            {
                cboLoadList.Enabled = false;
            }
        }

        /// <summary>
        /// Method to draw on the main form
        /// </summary>
        /// <param name="e">The active event to do the actual drawing</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //Draws the chess board
            DrawBoard(e);
            //Draws the pieces
            DrawPieces(e);
            //Draws the captured pieces
            DrawCapturedPieces(e);
        }

        /// <summary>
        /// Method to set up the board and pieces to be drawn
        /// </summary>
        public void SetupDrawing()
        {
            //Initializes the board's size and height to start drawing on the upper left corner of the form
            _boardSize.Height = ClientSize.Height / GLOBAL_SIZE;
            //Since it's a square, width = height
            _boardSize.Width = _boardSize.Height;
            //Loops through the board 8 by 8 times as it's stored as a table
            for (int i = 0; i < GLOBAL_SIZE; i++)
            {
                for (int j = 0; j < GLOBAL_SIZE; j++)
                {
                    //Make a new point for each piece on the board
                    _gridPoints[i, j] = new PointF(i * _boardSize.Height, j * _boardSize.Width);
                    //Make boundaries for each piece
                    _pieceBounds[i, j] = new RectangleF(new PointF(i * _boardSize.Height + 13, j * _boardSize.Width + 13), new SizeF(50, 50));
                    //Makes a new rectangle for the board
                    _gameBoard[i, j] = new RectangleF(_gridPoints[i, j], _boardSize);
                }
            }
        }

        /// <summary>
        /// Method to draw the pieces only
        /// </summary>
        /// <param name="e">Active event to do the drawing</param>
        public void DrawPieces(PaintEventArgs e)
        {
            //Loops through the 8x8 board
            for(int i = 0; i < GLOBAL_SIZE; i++)
            {
                for(int j = 0; j <GLOBAL_SIZE; j++)
                {
                    //Only draw if there is something to draw
                    if(_board.Pieces[i,j] != null)
                    {
                        //If the player is cheating, draw the altered pieces
                        if(_isCheating == true)
                        {
                            //Draw specific pieces for players 1 and 2
                            if (_board.Pieces[i, j].Owner == PlayerType.Player1)
                            {
                                //Draws the piece image for player 1
                                e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Pieces[i, j].Type), _pieceBounds[i, j]);
                            }
                            else
                            {
                                //Draws the piece image for player 2
                                e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Pieces[i, j].Type), _pieceBounds[i, j]);
                            }
                        }
                        //Otherwise, draw regular pieces
                        else
                        {
                            //Draw specifc pieces for players 1 and 2 with regards to the current theme
                            if (_board.Pieces[i, j].Owner == PlayerType.Player1)
                            {
                                //Draws the piece image for player 1
                                e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Settings.Theme + "a" + (int)_board.Pieces[i, j].Type), _pieceBounds[i, j]);
                            }
                            else
                            {
                                //Draws the piece image for player 2
                                e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Settings.Theme + "b" + (int)_board.Pieces[i, j].Type), _pieceBounds[i, j]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to draw the captured pieces of player 1 and 2
        /// </summary>
        /// <param name="e">The activator event to do the drawing</param>
        public void DrawCapturedPieces(PaintEventArgs e)
        {
            //Makes a new size for the captured piece
            SizeF capturedSize = new SizeF(25, 25);
            //Makes a point for the piece to be drawn on
            PointF drawPoint;
            //Makes a rectangle boundary for the piece
            RectangleF capturedPiece;
            //Stores how many piece player 1 has captured
            int captured = _board.Player1.PiecesCaptured.Count;
            //Loops thorugh all of the pieces that player 1 has captured
            for (int i = 0; i < captured; i++)
            {
                //Initialize a new point and boundary for the captured piece
                drawPoint = new PointF((_boardSize.Width) * GLOBAL_SIZE, capturedSize.Height * i);
                capturedPiece = new RectangleF(drawPoint, capturedSize);
                //If the player is not cheating, draw their captured pieced, otherwise, draw a special piece
                if (_isCheating == false)
                {
                    //Draw the captured piece (player 2's piece) for player 1
                    e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Settings.Theme + "b" + (int)_board.Player1.PiecesCaptured[i].Type), capturedPiece);
                }
                else
                {
                    //Draws a special piece that has been captured
                    e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Player1.PiecesCaptured[i].Type), capturedPiece);
                }
            }
            //reinitialize the number of captured pieces for player 2
            captured = _board.Player2.PiecesCaptured.Count;
            //Loops thorugh all of the pieces that player 2 has captured
            for (int i = 0; i < captured; i++)
            {
                //Initialize a new point and boundary for the captured piece
                drawPoint = new PointF((_boardSize.Width) * GLOBAL_SIZE + 25, capturedSize.Height * i);
                capturedPiece = new RectangleF(drawPoint, capturedSize);
                //If the player is not cheating, draw their captured pieced, otherwise, draw a special piece
                if (_isCheating == false)
                {
                    //Draw the captured piece (player 1's piece) for player 2
                    e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Settings.Theme + "a" + (int)_board.Player2.PiecesCaptured[i].Type), capturedPiece);
                }
                else
                {
                    //Draws a special piece that has been captured
                    e.Graphics.DrawImage((Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Player2.PiecesCaptured[i].Type), capturedPiece);
                }
            }
        }

        /// <summary>
        /// Draws the chess board
        /// </summary>
        /// <param name="e">Activator event to actually draw the board</param>
        public void DrawBoard(PaintEventArgs e)
        {
            //Stores all of the points for the valid spaces of a piece
            List<Point> validSpaces = _board.ValidSpaces;
            //Loops through the 8x8 board
            for (int i = 0; i < GLOBAL_SIZE; i++)
            {
                for (int j = 0; j < GLOBAL_SIZE; j++)
                {
                    //If there is no modulous of the point, an odd point, colour it dark gray
                    if ((i + j) % 2 == 0)
                    {
                        //Fills in the board square as dark gray
                        e.Graphics.FillRectangle(Brushes.DimGray, _gameBoard[i, j]);
                    }
                    else
                    {
                        //Fills in the board square as light gray
                        e.Graphics.FillRectangle(Brushes.LightGray, _gameBoard[i, j]);
                    }
                }
            }
            //Makes a new brush that's transparent green
            Brush transparentGreen = new SolidBrush(Color.FromArgb(100, 0, 255, 0));
            //Makes a new brugh that's solid gold
            Brush gold = new SolidBrush(Color.Gold);
            //If there are valid spaces for the piece
            if (validSpaces.Count != 0)
            {
                //Loops through all the valid space points
                for (int i = 0; i < validSpaces.Count; i++)
                {
                    //Fills in all the valid spaces that a piece can move to as transparent green
                    e.Graphics.FillRectangle(transparentGreen, _gameBoard[validSpaces[i].X, validSpaces[i].Y]);
                }
                //Fills in the current selected piece's position as gold
                e.Graphics.FillRectangle(gold, _gameBoard[_board.SelectedPiece.X, _board.SelectedPiece.Y]);
            }
        }

        /// <summary>
        /// Hides all of the panels
        /// </summary>
        public void HideAll()
        {
            //Hides the defeat panel
            pnlDefeat.Hide();
            //Hides the victory panel
            pnlVictory.Hide();
            //Hides the start panel
            pnlStart.Hide();
            //Hides the singe player panel
            pnlSinglePlayer.Hide();
            //Hides the settings panel
            pnlSettings.Hide();
            //Hides the multiplayer panel
            pnlMultiplayer.Hide();
            //Hides the coin flip panel
            pnlCoinFlip.Hide();
        }

        /// <summary>
        /// Displays all of the settings available
        /// </summary>
        public void ShowSettings()
        {
            //Hides all of the panels
            HideAll();
            //Show only the settings panel
            pnlSettings.Show();
        }

        /// <summary>
        ///Show the victory panel 
        /// </summary>
        public void ShowVictory()
        {
            //If the player has not been defeated
            if (!_isDefeated)
            {
                //Hides all of the panels
                HideAll();
                //Hows only the victory panel
                pnlVictory.Show();
                //Stops the background music
                _bgMusic.Stop();
                //Stops the game
                _isGameStarted = false;
                //Stops the timer
                tmrGameTimer.Enabled = false;
            }
        }

        /// <summary>
        /// Shows the defeat panel
        /// </summary>
        public void ShowDefeat()
        {
            //Hides all the panels
            HideAll();
            //Shows only the defeat screen
            pnlDefeat.Show();
            //Stops the background music
            _bgMusic.Stop();
            //Stops the game
            _isGameStarted = false;
            //Stops the timer
            tmrGameTimer.Enabled = false;
            _isDefeated = true;
        }

        /// <summary>
        /// Display the coin filp
        /// </summary>
        public void ShowCoinFlip(int value)
        {
            //hides all of the panels
            HideAll();
            //Shows only the coin flip screen
            pnlCoinFlip.Show();

            if (value == 0)
            {
                //Prompts the user that they're player 1
                lblCoinFlip.Text = "You are Player 1!";
                //Sets the current player to be player 1
                _board._player = PlayerType.Player1;
                //Set's the next player to be player 2
                _board.Turn = PlayerType.Player2;
                //Start their turn
                _board.StartTurn();
            }
            //Otherwise the random number generated is 1 so it's the other player's turn
            else
            {
                //Prompts the user that they're player 2
                lblCoinFlip.Text = "You are Player 2!";
                //Sets the current player to be player 2
                _board._player = PlayerType.Player2;
                //Sets the next player to be player 1
                _board.Turn = PlayerType.Player1;

                //Only start the match if it is a local match
                if (_board.BattleType == BattleType.LocalMatch)
                {
                    //Start player 1's turn
                    _board.StartTurn();
                }
            }
            //Disables saving for local match
            btnSave.Enabled = false;
            txtSaveName.Enabled = false;
            //Redraws everything
            Refresh();
        }

        /// <summary>
        /// If the user decides to forfeit
        /// </summary>
        public void Forfeit()
        {
            //Prompts the user if they're sure they want to forfeit
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to forfeit?", "You will lose", MessageBoxButtons.YesNo);
            //If they select they want to forfeit
            if (dialogResult == DialogResult.Yes)
            {
                //If it is a multiplayer match
                if (_board.BattleType == BattleType.LANMatch)
                {
                    //tell the server that the user has decided to forfeit
                    _board.SendForfeit();
                }
                //Stop the game
                _isGameStarted = false;
                //Hide all screens
                HideAll();
                //Show the defeat screen
                pnlDefeat.Show();
            }
        }

        /// <summary>
        /// If the user decides to quit the game
        /// </summary>
        public void QuitGame()
        {
            //Closes the application
            Application.Exit();
            //disconnects from everything (servers mainly)
            System.Environment.Exit(0);
        }

        /// <summary>
        /// If the user decides to change a setting
        /// </summary>
        private void btnSettings_Click(object sender, EventArgs e)
        {
            //Calls the show settings method
            ShowSettings();
        }

        /// <summary>
        /// If the user clicks the single player button
        /// </summary>
        private void btnSingle_Click(object sender, EventArgs e)
        {
            //Hides all of the screens
            HideAll();
            //shows the single player screen (difficulty selection)
            pnlSinglePlayer.Show();
        }

        /// <summary>
        /// If the user clicks the multi-player button
        /// </summary>
        private void btnMultiplayer_Click(object sender, EventArgs e)
        {
            //Hides all of the panels
            HideAll();
            //Shows only the multiplayer screen
            pnlMultiplayer.Show();
        }


        /// <summary>
        /// If the user clicks the main menu button on the defeat screen
        /// </summary>
        private void btnMainMenuD_Click(object sender, EventArgs e)
        {
            _isRestarting = true;

            //Restarts the application
            Application.Restart();
        }

        /// <summary>
        /// If the user clicks the exit button on the defeat screen
        /// </summary>
        private void btnExitD_Click(object sender, EventArgs e)
        {
            //Exits the game
            QuitGame();
        }

        /// <summary>
        /// If the user clicks the "Default Return to Main menu" button
        /// </summary>
        private void btnDefaultReturn_Click(object sender, EventArgs e)
        {
            //Reinitialize the settings theme
            _board.Settings.Theme = Theme.Classic;
            //Restart the music for default theme
            StartMusic((Stream)Properties.Resources.ResourceManager.GetObject("_" + (int)Theme.Classic));
            //Hides all of the screens
            HideAll();
            //Shows the start screen
            pnlStart.Show();
        }

        /// <summary>
        /// If the user clicks the "Save and return" button
        /// </summary>
        private void btnSaveReturn_Click(object sender, EventArgs e)
        {
            //Hides all of the screens
            HideAll();
            //Show the start screen
            pnlStart.Show();
        }

        /// <summary>
        /// If the user clicks the chilly theme button
        /// </summary>
        private void btnChilly_Click(object sender, EventArgs e)
        {
            //Starts the music to the respected theme
            StartMusic((Stream)Properties.Resources.ResourceManager.GetObject("_" + (int)Theme.ChillyChess));
            //Initializes the setting to chilly theme
            _board.Settings.Theme = Theme.ChillyChess;
        }

        /// <summary>
        /// If the user clicks the classic theme button
        /// </summary>
        private void btnClassic_Click(object sender, EventArgs e)
        {
            //Starts the music to the respected theme
            StartMusic((Stream)Properties.Resources.ResourceManager.GetObject("_" + (int)Theme.Classic));
            //Initializes the setting to classic default theme
            _board.Settings.Theme = Theme.Classic;
        }

        /// <summary>
        /// If the user clicks the dusty theme button
        /// </summary>
        private void btnDusty_Click(object sender, EventArgs e)
        {
            //Starts the music to the respected theme
            StartMusic((Stream)Properties.Resources.ResourceManager.GetObject("_" + (int)Theme.Dusty));
            //intitializes the setting to dusty theme
            _board.Settings.Theme = Theme.Dusty;
        }

        /// <summary>
        /// If the user clicks the rainbow theme setting
        /// </summary>
        private void btnRainbow_Click(object sender, EventArgs e)
        {
            //Starts the music to the respected theme
            StartMusic((Stream)Properties.Resources.ResourceManager.GetObject("_" + (int)Theme.Rainbow));
            //initializes the setting to rainbow theme
            _board.Settings.Theme = Theme.Rainbow;
        }

        /// <summary>
        /// If the user clicks the main menu button on the victory screen
        /// </summary>
        private void btnMainMenuV_Click(object sender, EventArgs e)
        {
            //Set the restart to true
            _isRestarting = true;
            //Restarts the application to go to main menu
            Application.Restart();
        }

        /// <summary>
        /// If the user clicks the exit button on the victory screen
        /// </summary>
        private void btnExitV_Click(object sender, EventArgs e)
        {
            //Closes the game
            QuitGame();
        }

        /// <summary>
        /// If the user clicks the main menu button
        /// </summary>
        private void btnMainMenu_Click(object sender, EventArgs e)
        {
            //Hides all of the screens
            HideAll();
            //Shows the start screen
            pnlStart.Show();
        }


        private void btnAddServer_Click(object sender, EventArgs e)
        {
            //Saves that the user wants their server name to be
            string name = txtServerName.Text.Trim();
            //If what they entered is nothing
            if (name == null)
            {
                //Prompt the user to enter something
                MessageBox.Show("Error: You need a name to create a new server");
            }
            //Otherwise, they've entered something so create the new server
            else
            {
                //Set the type of battle to be a local area network match
                _board.BattleType = BattleType.LANMatch;
                //Create the server with the desired name
                _board.CreateServer(name);
                //Hide all of the screens
                HideAll();
            }
        }

        /// <summary>
        /// If the user clicks the refresh button on multiplayer screen
        /// </summary>
        private void btnRfrsh_Click(object sender, EventArgs e)
        {
            //Loops through the number of servers
            for (int i = 0; i < 5; ++i)
            {
                //Prinpt out the server not found text
                LblServerName[i].Text = SERVER_NOT_FOUND;
            }
            //Refresh the server up to 255 times
            _board.RefreshServer(255);
            //Redraws everything
            Refresh();
        }

        /// <summary>
        /// If the user clicks the join server button on multiplayer screen
        /// </summary>
        private void btnJoin_Click(object sender, EventArgs e)
        {
            //Saves the name of what server they chose
            string name = ((Button)sender).Name;
            //Start at a position of 0
            int index = 0;
            //Converts what the user has selected to a position
            int.TryParse(name.Remove(0, 13), out index);
            //If the name is the same as the one saying "no server"
            if (_lblServerName[index].Text == SERVER_NOT_FOUND)
            {
                //Tell the user to select an active server
                MessageBox.Show("Error: You need to select an active server.");
            }
            //Otherwise, join the selected server
            else
            {
                //Joins the server from the board
                _board.JoinServer(((Button)sender).Name);
                _board.BattleType = BattleType.LANMatch;
                //Hides all the screens
                HideAll();
            }
            //Disable saving
            btnSave.Enabled = false;
            txtSaveName.Enabled = false;
        }


        /// <summary>
        /// If the user clicks the player local multiplayer button
        /// </summary>
        private void btnPlayLocal_Click(object sender, EventArgs e)
        {
            //Prompts the user to make a decision
            MessageBox.Show("Hint: Please decide on who will be doing the coin toss. They will be given their player number.");
            //Sets the battle type to be a local match
            _board.BattleType = BattleType.LocalMatch;
            //Flips a goin
            _board.CoinFlip();
        }

        /// <summary>
        /// Timer for the game to display how much time is left
        /// </summary>
        private void tmrGameTimer_Tick(object sender, EventArgs e)
        {
            //initiate the start time to however many seconds the player has
            int turnStartTime = _board.CurrentPlayer.TurnStartTime;
            //If it's a local match and it's the player's turn
            if (_board.BattleType == BattleType.LocalMatch || _board.Turn == _board._player)
            {
                //If the game has started and there is more than 0s left
                if (_isGameStarted == true && turnStartTime > 0)
                {
                    //the timer goes down
                    _board.CurrentPlayer.TurnStartTime--;
                    //initializes how many mins there are
                    string minute = "" + turnStartTime / 60, second = "" + turnStartTime % 60;
                    //If there is only 1 digit in the minuites
                    if (minute.Length == 1)
                    {
                        //Add another digit
                        minute = "0" + minute;
                    }
                    //If there is only 1 digit in the seconds
                    if (second.Length == 1)
                    {
                        //Add another digit
                        second = "0" + second;
                    }
                    //print out the time left
                    lblTimer.Text = minute + " : " + second + "s";
                }
                //Otherwise, the player does not have extra time and they have ran out of initial time
                else if (_isGameStarted == true && !_hasExtraTime && (turnStartTime == 0 || _board.CurrentPlayer.NumMoves < 0))
                {
                    //Re-initializes 30mins for the player
                    _board.CurrentPlayer.TurnStartTime = 30 * 60;
                    //Both players are running on extra time now
                    _hasExtraTime = true;
                }
                //If it's the player 1's turn
                else if (_board.CurrentPlayer == _board.Player1)
                {
                    //They lose
                    ShowDefeat();
                    //The server has a forfeit for player 1
                    _board.SendForfeit();
                }
            }
            //Otherwise, it's the other player's turn so print it out
            else
            {
                lblTimer.Text = "Wait...";
            }
        }


        /// <summary>
        /// If the user clicks on the easy difficulty
        /// </summary>
        private void btnEasy_Click(object sender, EventArgs e)
        {
            //Set the difficulty to be easy
            _board.GameDifficulty = Difficulty.Easy;
            //hide the panel that's covering the difficulty image
            pnlHideDifficulty.Dispose();
            //Bring the easy difficulty image to the front
            picEasy.BringToFront();
        }

        /// <summary>
        /// If the user clicks the normal difficulty button
        /// </summary>
        private void btnNormal_Click(object sender, EventArgs e)
        {
            //Set the difficulty to be Normal
            _board.GameDifficulty = Difficulty.Normal;
            //hide the panel that's covering the difficulty image
            pnlHideDifficulty.Dispose();
            //Bring the Normal difficulty image to the front
            picNormal.BringToFront();
        }

        /// <summary>
        /// If the user clicks the Hard difficulty button
        /// </summary>
        private void btnHard_Click(object sender, EventArgs e)
        {
            //Set the difficulty to be Hard
            _board.GameDifficulty = Difficulty.Hard;
            //hide the panel that's covering the difficulty image
            pnlHideDifficulty.Dispose();
            //Bring the Hard difficulty image to the front
            picHard.BringToFront();
        }

        /// <summary>
        /// If the user clicks the Grandmaster difficulty button
        /// </summary>
        private void btnGrandMaster_Click(object sender, EventArgs e)
        {
            //Set the difficulty to be Grandmaster
            _board.GameDifficulty = Difficulty.Grandmaster;
            //hide the panel that's covering the difficulty image
            pnlHideDifficulty.Dispose();
            //Bring the Grandmaster difficulty image to the front
            picGrandmaster.BringToFront();
        }

        /// <summary>
        /// If the user clicks the start game button
        /// </summary>
        private void btnStartGame_Click(object sender, EventArgs e)
        {
            //There is a difficulty chosen, start the game
            if (_board.GameDifficulty != Difficulty.None)
            {
                //Set the game type to be an AI match
                _board.BattleType = BattleType.AIMatch;
                _board.MakeAI();

                //Hide all screens
                HideAll();
                //Toggle game start and timer to start
                _isGameStarted = true;
                tmrGameTimer.Enabled = true;
            }
            //Otherwise, prompt the user to select a difficulty
            else
            {
                MessageBox.Show("Error: A difficulty requires to be selected.");
            }
        }

        /// <summary>
        /// If the user decides to forfeit
        /// </summary>
        private void btnForfeit_Click(object sender, EventArgs e)
        {
            //Prompts the user if they're sure they want to forfeit
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to forfeit?", "You will lose", MessageBoxButtons.YesNo);
            //If they select they want to forfeit
            if (dialogResult == DialogResult.Yes)
            {
                //Shut down the game
                _isGameStarted = false;
                //Hide all the panels
                HideAll();
                //Show the defeat screen
                pnlDefeat.Show();
            }
        }

        /// <summary>
        /// If the user decides to view all of the movement logs
        /// </summary>
        private void btnViewLog_Click(object sender, EventArgs e)
        {
            //Show all of the movement logs stored in the board
            DialogResult dialogResult = MessageBox.Show(_board.MoveLog, "Move logs", MessageBoxButtons.OK);
        }

        /// <summary>
        /// If the user decides to save the game
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //If the user has not typed in a save name
            string name = txtSaveName.Text.Trim();
            if (name == "")
            {
                //Prompt the user to type in a saved name
                MessageBox.Show("Error: You need a save name to save the game.");
            }
            //Otherwise they typed something so save the game
            else
            {
                //Calls the save game method with the board and the name of the save file
                _board.SaveGame(name);
                //Clears the save name textbox
                txtSaveName.Text = "";
            }
        }

        /// <summary>
        /// If the user decides to begin the game from local 2 player
        /// </summary>
        private void btnBegin_Click(object sender, EventArgs e)
        {
            //Hide all screens
            HideAll();
            //Enable game and timer
            _isGameStarted = true;
            tmrGameTimer.Enabled = true;
        }

        /// <summary>
        /// If the user decides to load a saved game
        /// </summary>
        private void btnLoadSaved_Click(object sender, EventArgs e)
        {
            //Gets the name of the saved game from a drop-down box
            string name = cboLoadList.Text;
            if(File.Exists(name + ".chess"))
            {
                _board.MakeAI();
                _board.LoadGame(name);
                //Hides all the panels
                HideAll();
                //Redraws everything
                Refresh();
                //Start the music based on the board theme
                StartMusic(Properties.Resources.ResourceManager.GetStream("_" + (int)_board.Settings.Theme));
                //Start the game
                _isGameStarted = true;
                tmrGameTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Displays the pawn promotion
        /// </summary>
        public void ShowPawnPromote()
        {
            //Shows the transparent panel so the player can't move any pieces
            pnlTransparent.Show();
            //Show the pawn promotion panel
            pnlPawnPromote.Show();
            //Bring that panel to the front
            pnlPawnPromote.BringToFront();
            //Clear the image for promotion
            picPromote.Image = null;
            //Initialize the selected piece
            _pawn = _board.SelectedPiece;
            //Redraws everything
            Refresh();
        }

        //Event occurs when the user presses something
        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            //Shuts off event if it's a multiplayer lan match and the board isn't connected
            if (_board.BattleType == BattleType.LANMatch && !_board.isConnected)
            {
                return;
            }

            //Gets the user's mouse position and use math to determine which tile they're on
            int x = 7 - (608 - e.X) / 76, y = 7 - (608 - e.Y) / 76;

            //If the current battletype is multiplayer and it's not the player's turn or they are promoting, disable mouse click
            if ((_board.BattleType == BattleType.LANMatch && _board.Turn != _board._player) || _isPromoting)
            {
                return;
            }
            //If the mouse clicks somewhere other than the board, disable mouse click
            if (e.X > 608)
            {
                return;
            }

            //If the user is within the chess board
            if (x <= 7 && y <= 7)
            {
                // if a piece is selected
                if (_board.SelectedPiece != null)
                {
                    // move the piece if the user does not select the same piece
                    if (_board.Pieces[x, y] != _board.SelectedPiece)
                    {
                        //Move the piece by updating its x and y
                        _board.Move(_board.SelectedPiece, x, y, true);
                    }
                    _board.SelectedPiece = null;
                    //Clear all valid spaces
                    _board.ValidSpaces.Clear();
                    //Redraws everything
                    Refresh();
                }
                //Otherwise, the player had decided to click on a piece
                else if (_board.Pieces[x, y] != null && _board.SelectedPiece == null)
                {
                    if (_board.Pieces[x, y].Owner == _board.Turn)
                    {
                        //The player has made no mistakes
                        lblPrompts.Text = "";
                        //Select the current piece
                        _board.SelectedPiece = _board.Pieces[x, y];
                        //Get the valid spaces for that piece on the board
                        _board.SelectedPiece.GetValidSpaces(ref _board, true);
                        //Redraws everything
                        Refresh();
                    }
                    //Otherwise, it's not the player's turn so prompt the user
                    else
                    {
                        lblPrompts.Text = "It's not your turn.";
                    }
                }
            }
            //Otherwise, the selection that the user has made in invalid
            else
            {
                //Tell the user that their selection is invalid
                lblPrompts.Text = "Selection unrecognized.";
            }
        }


        /// <summary>
        /// If the user decides to return to main menu
        /// </summary>
        private void btnBackToMain_Click(object sender, EventArgs e)
        {
            //Hides all screens
            HideAll();
            //Shows the start screen only
            pnlStart.Show();
        }

        /// <summary>
        /// If the user decides to promote their pawn into a queen
        /// </summary>
        private void btnQueen_Click(object sender, EventArgs e)
        {
            //Makes a new queen piece with the same owner and coordinates
            Queen queen = new Queen(_pawn.Owner, _pawn.X, _pawn.Y);

            _promotedPiece = queen;

            //Show the image of the queen as the promoted piece
            DisplayPromotionImage(queen);
        }

        /// <summary>
        /// If the user decides to promote their pawn into a knight
        /// </summary>
        private void btnKnight_Click(object sender, EventArgs e)
        {
            //Makes a new knight piece with the same owner and coordinates
            Knight knight = new Knight(_pawn.Owner, _pawn.X, _pawn.Y);

            _promotedPiece = knight;

            //Show the image of the knight as the promoted piece
            DisplayPromotionImage(knight);
        }

        /// <summary>
        /// If the user decides to promote their pawn into a bishop
        /// </summary>
        private void btnBishop_Click(object sender, EventArgs e)
        {
            //Makes a new bishop piece with the same owner and coordinates
            Bishop bishop = new Bishop(_pawn.Owner, _pawn.X, _pawn.Y);

            _promotedPiece = bishop;

            //Show the image of the bishop as the promoted piece
            DisplayPromotionImage(bishop);
        }

        /// <summary>
        /// If the user decides to promote their pawn into a rook
        /// </summary>
        private void btnRook_Click(object sender, EventArgs e)
        {
            //Makes a new rook piece with the same owner and coordinates
            Rook rook = new Rook(_pawn.Owner, _pawn.X, _pawn.Y);
            //Set the promoted piece as the new rook
            _promotedPiece = rook;
            //Show the image of the rook as the promoted piece
            DisplayPromotionImage(rook);
        }

        /// <summary>
        /// Method to show the future of the pawn promotion
        /// </summary>
        /// <param name="piece">The piece that's new</param>
        private void DisplayPromotionImage(Piece piece)
        {
            //If the owner is player one, show the respected piece for player one
            if (_pawn.Owner == PlayerType.Player1)
            {
                picPromote.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Settings.Theme + "a" + (int)piece.Type);
            }
            //Otherwise the owner is player 2, show the respected piece for player two
            else
            {
                picPromote.Image = picPromote.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Settings.Theme + "b" + (int)piece.Type);
            }
        }

        /// <summary>
        /// If the user confirms their piece choice
        /// </summary>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            //If it is a local area match
            if (_board.BattleType == BattleType.LANMatch)
            {
                //sends the pawn being promoted
                _board.SendPromote(_pawn.X, _pawn.Y);
            }

            //Promote the pawn into the promoted piece.
            _board.PromotePawn(_promotedPiece);

            //Hides the pawn promotion panel
            pnlPawnPromote.Hide();
            //Hides the transparent panel
            pnlTransparent.Hide();
            //Redraws board
            Refresh();
        }

        /// <summary>
        /// If the user decides to stop/play music
        /// </summary>
        private void btnToggleMusic_Click(object sender, EventArgs e)
        {
            //If music has been started already
            if (_isMusicStarted == true)
            {
                //stop the music
                _bgMusic.Stop();
                //toggle music start to false
                _isMusicStarted = false;
                //Display on the button that they can start the music again
                btnToggleMusic.Text = "Start ♫";
            }
            //Otherwise, start the music
            else
            {
                //toggle the music to be started
                _isMusicStarted = true;
                //Start the music based on the theme of the game
                StartMusic((Stream)Properties.Resources.ResourceManager.GetObject("_" + (int)_board.Settings.Theme));
                //Display on the button that they can start the Stop again
                btnToggleMusic.Text = "Stop ♫";
            }
        }

        /// <summary>
        /// If the user decides to cheat
        /// </summary>
        private void GameHack()
        {
            //Can only cheat if cheating is toggled
            if (_isCheating == true)
            {
                //Loops through the board of pieces
                for (int i = 0; i < GLOBAL_SIZE; i++)
                {
                    for (int j = 0; j < GLOBAL_SIZE; j++)
                    {
                        //If there is a piece
                        if (_board.Pieces[i, j] != null)
                        {
                            //If the curernt player who pressed to cheat is player 1
                            if (_board.CurrentPlayer.ID == PlayerType.Player1)
                            {
                                //If the piece belongs to player 2
                                if (_board.Pieces[i, j].Owner == PlayerType.Player2)
                                {
                                    //rewrite their piece as a bahn mi piece
                                    _board.Pieces[i, j] = new Vietnam(PlayerType.Player2, i, j);
                                }
                                //Otherwise the piece belongs to player 1
                                else
                                {
                                    //rewrite their piece as a hsiung piece
                                    _board.Pieces[i, j] = new Hsiung(PlayerType.Player1, i, j);
                                }
                            }
                            //Otherwise the current player is player 2
                            else
                            {
                                //If the piece belongs to player 1
                                if (_board.Pieces[i, j].Owner == PlayerType.Player1)
                                {
                                    //rewrite their piece as a bahn mi piece
                                    _board.Pieces[i, j] = new Vietnam(PlayerType.Player1, i, j);
                                }
                                //Otherwise the piece belongs to player 2
                                else
                                {

                                    //rewrite their piece as a hsiung piece
                                    _board.Pieces[i, j] = new Hsiung(PlayerType.Player2, i, j);
                                }
                            }
                        }

                    }
                }
                //If player 1 has some captured pieces of player 2 has some captured pieces
                if(_board.Player1.PiecesCaptured.Count > 0 || _board.Player2.PiecesCaptured.Count > 0)
                {
                    //Save player 1's and 2's captured pieces count
                    int p1 = _board.Player1.PiecesCaptured.Count;
                    int p2 = _board.Player2.PiecesCaptured.Count;
                    //If the current player is player 1
                    if (_board.CurrentPlayer.ID == PlayerType.Player1)
                    {
                        //Loop through player 1's captured pieces
                        for (int i = 0; i < p1; i++)
                        {
                            //Replace every captured piece with a bahn mi piece
                            _board.Player1.PiecesCaptured[i] = new Vietnam(PlayerType.Player2, 0, 0);
                        }
                        //Loop through player 2's captured pieces
                        for (int i = 0; i < p2; i++)
                        {
                            //Replace every captured piece with a hsiung piece
                            _board.Player2.PiecesCaptured[i] = new Hsiung(PlayerType.Player1, 0, 0);
                        }
                    }
                    //Otherwise the current player is player 2
                    else
                    {
                        //Loop through player 1's captured pieces
                        for (int i = 0; i < p1; i++)
                        {
                            //Replace every captured piece with a hsiung piece
                            _board.Player1.PiecesCaptured[i] = new Hsiung(PlayerType.Player2, 0, 0);
                        }
                        //Loop through player 2's captured pieces
                        for (int i = 0; i < p2; i++)
                        {
                            //Replace every captured piece with a bahn mi piece
                            _board.Player2.PiecesCaptured[i] = new Vietnam(PlayerType.Player1, 0, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If the user clicks the easter-egg cheat button
        /// </summary>
        private void btnCheat_Click(object sender, EventArgs e)
        {
            //If the battletype is a local multiplayer or an AI match
            if (_board.BattleType == BattleType.LocalMatch || _board.BattleType == BattleType.AIMatch)
            {
                //Toggle cheats
                _isCheating = true;
                //disable saving
                btnSave.Enabled = false;
                //enable cheats
                GameHack();
                //Clears the current selected piece
                _board.SelectedPiece = null;
                //Redraws everything
                Refresh();
                //Disables the saving input
                txtSaveName.Enabled = false;
            }
        }

        /// <summary>
        /// Flip the actual coin
        /// </summary>
        /// <param name="value">The value of the coin's result</param>
        public void FlipCoin(int value)
        {
            //The coin is flipping so set the flag to true
            _isCoinFlipping = true;
            //Set the flip value to the random number
            _flipValue = value;
        }

        /// <summary>
        /// Update the game based on server
        /// </summary>
        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            //If the coin is being flipped
            if (_isCoinFlipping)
            {
                //Display the value that has been flipped
                ShowCoinFlip(_flipValue);
                //Toggle coin flipping off
                _isCoinFlipping = false;
            }
            //If the board wants the be redrawn
            if (_isRefresh)
            {
                //Redraw the board
                Refresh();
                //Toggle the refresh to be false
                _isRefresh = false;
            }
            //If the lable is being updated
            if (_isLblUpdate)
            {
                //Update the lable
                LblServerName[_numFound].Text = _lblName;
                //The lable does not need to be updated anymore
                _isLblUpdate = false;
                //Redraw the board
                Refresh();
            }
            //If the server wants to be displayed
            if (_isDisplayServer)
            {
                //Update the server display
                lblPrompts.Text = "Connection Information:\nIp:" + _name + "\nPort: " + _port;
                //The server's display does not need to be displayed
                _isDisplayServer = false;
                //Redraws the board
                Refresh();
            }
            //If the victorn screen requires to be displayed
            if (_isDisplayWin)
            {
                //Show the victory screen
                ShowVictory();
                //The victory screen does not need to be displayed anymore
                _isDisplayWin = false;
                //Redraws the board
                Refresh();
            }
            //If the prompt requires to be erased
            if (_isErasePrompt)
            {
                //Erase the prompt, clear it
                lblPrompts.Text = "";
                //The prompt doesn't need to be cleared again
                _isErasePrompt = false;
            }
        }

        /// <summary>
        /// The quick refresh server button is pressed
        /// </summary>
        private void btnQuickRefresh_Click(object sender, EventArgs e)
        {
            //loops through the max number of servers available
            for (int i = 0; i < 5; ++i)
            {
                //Change the name to server not found if there is no server
                LblServerName[i].Text = SERVER_NOT_FOUND;
            }
            //Refresh the server and update the text to display available servers
            _board.RefreshServer(1);
            //Redraws the game
            Refresh();
        }

        /// <summary>
        /// A Direct connection to a server
        /// </summary>
        private void btnDirectConnect_Click(object sender, EventArgs e)
        {
            //Saves the ip address for the direct connection
            string ipAddress = txtDirectConnect.Text;
            //Initializes a port value
            int port;
            //converts the port value into an integer to compare
            int.TryParse(txtPort.Text, out port);
            //Gets the name of the button
            string name = ((Button)sender).Name;
            //If the name is not present
            if (name == SERVER_NOT_FOUND)
            {
                //Prompt the user that they have to select an active server
                MessageBox.Show("Error: You need to select an active server.");
            }
            //Otherwise
            else
            {
                //Join the selected server with the address and port
                _board.JoinServer(ipAddress, port);
                //Set the battletype to be local area network
                _board.BattleType = BattleType.LANMatch;
                //hides all screens
                HideAll();
            }
            //Disable saving
            btnSave.Enabled = false;
            txtSaveName.Enabled = false;
        }

        /// <summary>
        /// When the form is closing
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            //If the player isn't restarting
            if (!_isRestarting)
            {
                //If the player is currently in a LAN match
                if (_board.BattleType == BattleType.LANMatch)
                {
                    //They can't randomly decide to exit
                    e.Cancel = true;
                    //Prompt the user why they can't exit
                    MessageBox.Show("You cannot force-exit a multiplayer game. You must finish the game or forefit to exit.");
                }
                //Otherwise, dissconnect everything (servers)
                else
                {
                    //Exits all servers and connections
                    System.Environment.Exit(0);
                }
            }
        }
    }
}
