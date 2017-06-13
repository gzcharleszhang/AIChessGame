/*
 * Charles, Nemo, Henry
 * Jan 25, 2017
 * ISU
 * BoardModel Class
 * Creates BoardModel objects and handles the game's mechanics
 * Stores all the objects on the chess board
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;

namespace ChessChallenge
{
    public class BoardModel
    {
        // Stores the pieces on the board
        private Piece[,] _pieces;
        // Stores the games difficulty, default is set to none
        private Difficulty _gameDifficulty = Difficulty.None;
        // Stores the spaces that a piece is able to move to
        private List<Point> _validSpaces;
        // Stores the current turn
        private PlayerType _turn;
        // Stores the two players that play teh game
        private Player _p1, _p2;
        // Stores the type of the battle
        private BattleType _battleType;
        // Stores the current round
        private int _round;
        // Stores the move history
        private string _moveLog;
        // Stores the number of seconds left until the game ends
        private int _secondsLeft;
        // State variable whether there is a server or not
        private bool _isServer;
        // Stores the piece that is selected
        private Piece _selectedPiece;
        // Stores the network that connects two players
        private Network _network;
        // Stores the setting of the game
        private Setting _setting;
        // Stores the form that draws all the graphics and the user interface
        private View _form;
        // Stores the receiver that gets information from the other player
        private Reciever _reciever;
        // State variable whether the players are connected to the game or not
        public bool isConnected = false;
        // Stores the type of player
        public PlayerType _player;
        // Stores the list of open servers
        private List<ServerInfo> _openServers;

        /// <summary>
        /// Allows the game difficulty to be visible to other classes
        /// and the value can be change externally
        /// </summary>
        public Difficulty GameDifficulty
        {
            get
            {
                return _gameDifficulty;
            }
            set
            {
                _gameDifficulty = value;
            }
        }

        /// <summary>
        /// Tells the other player that there is a pawn promotion
        /// </summary>
        /// <param name="x">The x coordinate of the pawn</param>
        /// <param name="y">the y coordinate of the pawn</param>
        public void SendPromote(int x, int y)
        {
            // The network is active and sends that information to the other player through the network
            _network.State = ServerState.Active;
            _network.SendPawnPromote(Pieces[x, y]);
        }

        /// <summary>
        /// Allows the settings to be visible to other classes
        /// </summary>
        public Setting Settings
        {
            get
            {
                return _setting;
            }
        }

        /// <summary>
        /// Allows the number of turns to be visible to other classes
        /// Can be changed externally
        /// </summary>
        public PlayerType Turn
        {
            get
            {
                return _turn;
            }
            set
            {
                _turn = value;
            }
        }

        /// <summary>
        /// Constructor that creates the boardmodel object
        /// </summary>
        /// <param name="form">The form that the board is created in</param>
        public BoardModel(View form)
        {
            // Assigns the boardmodel's starting values
            _form = form;
            // Creates the two players that play teh game
            _p1 = new Player("You", PlayerType.Player1);
            _p2 = new Player("Opponent", PlayerType.Player2);

            /*_player = PlayerType.Player1;
            _turn = PlayerType.Player2;*/
            // Creates settings, validspaces, and the receiver
            _setting = new Setting();
            _validSpaces = new List<Point>();
            _reciever = new Reciever(this);

            // Sets up the pieces and sets the start time
            SetupPieces();
            InitializeTime();
            //StartTurn();
        }

        /// <summary>
        /// Allows the form to be visible to other classes
        /// </summary>
        public View Form
        {
            get
            {
                return _form;
            }
        }

        /// <summary>
        /// Sets up the pieces
        /// </summary>
        private void SetupPieces()
        {
            // The max index that the pieces array can have
            // size - 1
            const int MAX_INDEX = 7;
            // Creates the pieces 2D array, a 8x8 grid
            _pieces = new Piece[MAX_INDEX + 1, MAX_INDEX + 1];
            // Creates the playertype object - an owner of a piece
            PlayerType owner = new PlayerType();
            // Loops twice for two players
            for (int i = 0; i < 2; i++)
            {
                // For the first time the loop runs
                if (i == 0)
                {
                    // Set the owner to the second player
                    owner = PlayerType.Player2;
                    // Sets the pawn's starting locations
                    for (int j = 0; j < 8; j++)
                    {
                        Pawn pawn = new Pawn(owner, j, 1);
                        _pieces[j, 1] = pawn;
                    }
                }
                // For the second time the loop runs
                else if (i == 1)
                {
                    // Set the owner to the first player
                    owner = PlayerType.Player1;
                    // Sets the pawn's starting locations
                    for (int j = 0; j < 8; j++)
                    {
                        Pawn pawn = new Pawn(owner, j, 6);
                        _pieces[j, 6] = pawn;
                    }
                }

                // Sets the rook's starting locations
                for (int j = 0; j < 2; j++)
                {
                    Rook rook = new Rook(owner, MAX_INDEX * j, MAX_INDEX * i);
                    _pieces[MAX_INDEX * j, MAX_INDEX * i] = rook;
                }

                // Sets the knights' starting locations
                for (int j = 0; j < 2; j++)
                {
                    Knight knight = new Knight(owner, (MAX_INDEX - 2) * j + 1, MAX_INDEX * i);
                    _pieces[(MAX_INDEX - 2) * j + 1, MAX_INDEX * i] = knight;
                }

                // Sets the bishop's starting locations
                for (int j = 0; j < 2; j++)
                {
                    Bishop bishop = new Bishop(owner, (MAX_INDEX - 4) * j + 2, MAX_INDEX * i);
                    _pieces[(MAX_INDEX - 4) * j + 2, MAX_INDEX * i] = bishop;
                }
                // Sets the king's starting locations
                King king = new King(owner, 4, MAX_INDEX * i);
                _pieces[4, MAX_INDEX * i] = king;

                // The two player's kingPiece are the kings created on this board
                if (i == 0)
                {
                    _p2.KingPiece = king;
                }
                else
                {
                    _p1.KingPiece = king;
                }


                // Sets up the queen's starting locations
                Queen queen = new Queen(owner, 3, MAX_INDEX * i);
                _pieces[3, MAX_INDEX * i] = queen;
            }
        }

        /// <summary>
        /// Sets the start time
        /// </summary>
        private void InitializeTime()
        {
            _secondsLeft = 90 * 60;
        }

        /// <summary>
        /// Updates the time remaining
        /// </summary>
        public void UpdateTime()
        {
            // Decreases the time by 1 second
            _secondsLeft--;
            // Set time remaining to 30 minutes if 40 rounds have passed 
            // and there is more than 30 minutes remaining
            if (_round == 41 && SecondsLeft > 60 * 30)
            {
                _secondsLeft = 60 * 30;
            }
        }

        /// <summary>
        /// Allows the current player to be visible to other classes
        /// </summary>
        public Player CurrentPlayer
        {
            get
            {
                if (_turn == PlayerType.Player1)
                {
                    return _p1;
                }
                else
                {
                    return _p2;
                }
            }
        }

        /// <summary>
        /// Allows the other player to be visible to other classes
        /// </summary>
        public Player OtherPlayer
        {
            get
            {
                if (_turn == PlayerType.Player1)
                {
                    return _p2;
                }
                else
                {
                    return _p1;
                }
            }
        }

        /// <summary>
        /// Allows the battleType to be visible to other classes
        /// 
        /// </summary>
        public BattleType BattleType
        {
            get
            {
                return _battleType;
            }
            //battleType can be changed externally
            set
            {
                _battleType = value;
            }
        }

        /// <summary>
        /// Allows the pieces to be visible to other classes
        /// </summary>
        public Piece[,] Pieces
        {
            get
            {
                return _pieces;
            }
            // Pieces can be changed externally
            set
            {
                _pieces = value;
            }
        }
        /// <summary>
        /// Allows the openServers to be visible to other classes
        /// </summary>
        public List<ServerInfo> OpenServers
        {
            get
            {
                return _openServers;
            }
        }
        /// <summary>
        /// Allows the first player to be visible to other classes
        /// </summary>
        public Player Player1
        {
            get
            {
                return _p1;
            }
        }
        /// <summary>
        /// Allows the second player to be visible to other classes
        /// </summary>
        public Player Player2
        {
            get
            {
                return _p2;
            }
        }
        /// <summary>
        /// Allows the validSpaces to be visible to other classes
        /// </summary>
        public List<Point> ValidSpaces
        {
            get
            {
                return _validSpaces;
            }
        }
        /// <summary>
        /// Allows isServer to be visible to other classes
        /// </summary>
        public bool IsServer
        {
            get
            {
                return _isServer;
            }
        }
        /// <summary>
        /// Allows the number of rounds to be visible to other classes
        /// </summary>
        public int Round
        {
            get
            {
                return _round;
            }
        }
        /// <summary>
        /// Allows the movelog to be visible to other classes
        /// </summary>
        public string MoveLog
        {
            get
            {
                return _moveLog;
            }
        }
        /// <summary>
        /// Allows the number of seconds left to be visible to other classes
        /// </summary>
        public int SecondsLeft
        {
            get
            {
                return _secondsLeft;
            }
        }

        /// <summary>
        /// Moves a selected piece to a destination
        /// </summary>
        /// <param name="piece">the piece that is selected</param>
        /// <param name="x">destination's x coordinate</param>
        /// <param name="y">destination's y coordinate</param>
        /// <returns></returns>
        public bool Move(Piece piece, int x, int y, bool startNextTurn)
        {
            // Loops for all the validspaces
            for (int i = 0; i < ValidSpaces.Count; i++)
            {
                // If the destination is one of the valid spaces of the piece
                if (ValidSpaces[i].X == x && ValidSpaces[i].Y == y)
                {
                    // if theres an enemy at the destination
                    if (startNextTurn && Pieces[x, y] != null && Pieces[x, y].Owner != _turn)
                    {
                        // Capture the piece
                        CurrentPlayer.PiecesCaptured.Add(Pieces[x, y]);
                    }
                    // if there is a piece at the destination
                    if (startNextTurn && _pieces[x, y] != null)
                    {
                        // saves the move
                        SaveMove(piece, x, y, "Captured " + _pieces[x, y].Type);
                    }

                    // if there is an ally piece at the destination 
                    if (_pieces[x, y] != null && startNextTurn && _pieces[x, y].Owner == CurrentPlayer.ID && (BattleType != BattleType.AIMatch || _turn == PlayerType.Player1))
                    {
                        // clears the valid spaces
                        ValidSpaces.Clear();

                        //NETWORK FOR CASTLING
                        // If the piece at the destination is a king
                        if (_pieces[x, y].Type == PieceType.King)
                        {
                            // If it is a LAN match
                            if (BattleType == BattleType.LANMatch)
                            {
                                // Tell the other player that the king is castling
                                _network.SendCastle(x, y, piece.X, piece.Y, _selectedPiece.HasMoved);
                            }
                            // The king castles, retursn true if it is valid, false if it's not
                            return DoCastle(_pieces[x, y], _selectedPiece);
                        }
                        // If the piece is a rook
                        else if (_pieces[x, y].Type == PieceType.Rook)
                        {
                            // If it is a LAN match
                            if (BattleType == BattleType.LANMatch)
                            {
                                // Tell the other play that the rook is being castled
                                _network.SendCastle(piece.X, piece.Y, x, y, _selectedPiece.HasMoved);
                            }
                            // The rook is being castled, returns true if it is valid, false if not
                            return DoCastle(_selectedPiece, _pieces[x, y]);
                        }
                    }
                    else
                    {
                        // Moves the piece to the destination
                        _pieces[x, y] = piece;
                        _pieces[piece.X, piece.Y] = null;

                        // If it is a LAn match
                        if (BattleType == BattleType.LANMatch)
                        {
                            // Tells the other play that the piece has moved
                            _network.SendMove(piece.X, piece.Y, x, y, _selectedPiece.HasMoved);
                        }

                        piece.Move(this, x, y);

                        // If the current player is being chedk
                        if (CurrentPlayer.IsChecked)
                        {
                            // The player is not being checked
                            CurrentPlayer.IsChecked = false;
                        }
                        // makes a temporary boardmodel
                        // Gets the piece's valid spaces
                        BoardModel thisBoard = this;
                        piece.GetValidSpaces(ref thisBoard, true);

                        // Loops through the list of valid spaces
                        for (int space = 0; space < ValidSpaces.Count; space++)
                        {
                            // If the valid space has a piece
                            // and it is an enemy king
                            if (Pieces[ValidSpaces[space].X, ValidSpaces[space].Y] != null
                                && Pieces[ValidSpaces[space].X, ValidSpaces[space].Y].Type == PieceType.King
                                && Pieces[ValidSpaces[space].X, ValidSpaces[space].Y].Owner != _turn)
                            {
                                // The other player is being checked
                                OtherPlayer.PiecesChecking.Add(piece);
                                OtherPlayer.IsChecked = true;
                            }
                        }
                        // Saves the current move
                        SaveMove(piece, x, y, "moved");

                        if (startNextTurn)
                        {
                            StartTurn();
                        }

                        ValidSpaces.Clear();
                        // Tells the form to refresh
                        Form.IsRefresh = true;

                        if (startNextTurn && canPromotePawn(_pieces[x, y]))
                        {
                            // If this is a LAN match
                            // and the piece is an enemy
                            if (this.BattleType == BattleType.LANMatch && _pieces[x, y].Owner != _player)
                            {
                                // Waits for the other player to move
                                Form.IsPromoting = true;
                                _network.State = ServerState.Waiting;
                            }
                            else
                            {
                                // Promotes the pawn
                                Form.ShowPawnPromote();
                            }
                        }
                        // The piece is able to move
                        return true;
                    }
                }
            }
            // Clears the list of validspaces
            ValidSpaces.Clear();
            // The piece is not able to move
            return false;
        }
        /// <summary>
        /// Tells the other player that the current player is forfeiting
        /// </summary>
        public void SendForfeit()
        {
            // If a network exists
            if (_network != null)
            {
                // Sends the forfeit message to the other player
                _network.SendForfeit();
            }
        }

        /// <summary>
        /// Allows the selected piece to be visible to other classes
        /// </summary>
        public Piece SelectedPiece
        {
            get
            {
                return _selectedPiece;
            }
            // Selected piece can be changed externally
            set
            {
                _selectedPiece = value;
            }
        }
        /// <summary>
        /// A king is castling with a rook or vice versa
        /// </summary>
        /// <param name="piece">the selected piece</param>
        /// <param name="piece2">the second seleted piece</param>
        /// <returns></returns>
        public bool DoCastle(Piece piece, Piece piece2)
        {
            // If the selected piece is a king and the second piece is a rook
            if (piece.Type == PieceType.King && piece2.Type == PieceType.Rook)
            {
                // If the king is on the right of the rook
                if (piece.X > piece2.X)
                {
                    // Moves the king two steps to the left
                    // Moves the rook three stesp to the right
                    _pieces[2, piece.Y] = piece;
                    _pieces[3, piece2.Y] = piece2;
                    _pieces[piece.X, piece.Y] = null;
                    _pieces[piece2.X, piece2.Y] = null;
                    piece.Move(this, 2, piece.Y);
                    piece2.Move(this, 3, piece2.Y);
                }
                else
                {
                    // Moves the king two stesp to the right
                    // Moves the rook two stesp to the left
                    _pieces[6, piece.Y] = piece;
                    _pieces[5, piece2.Y] = piece2;
                    _pieces[piece.X, piece.Y] = null;
                    _pieces[piece2.X, piece2.Y] = null;
                    piece.Move(this, 6, piece.Y);
                    piece2.Move(this, 5, piece2.Y);
                }
                // Saves the current move
                SaveMove(piece, piece.X, piece.Y, "castled");
                // Starts the next turn and refreshes the form
                Form.IsRefresh = true;
                StartTurn();
                // The two pieces can castle
                return true;
            }
            // The two pieces cannot castle
            return false;
        }
        /// <summary>
        /// Gets the server's IP adress
        /// </summary>
        /// <returns>the server's IP adress</returns>
        public string GetIp()
        {
            // Gets the Host's ip addresses
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            // Set the LAN ip address to null
            IPAddress lanIp = null;
            // Sets the LAN ip to one of the host's ip addresses if the ip works
            for (int i = 0; i < localIPs.Length; ++i)
            {
                if (localIPs[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    lanIp = localIPs[i];
                }
            }
            // If there is an IP, return the IP
            if (lanIp != null)
            {
                return lanIp.ToString();
            }
            else
            {
                // There is no IP
                return "Ip not found :(";
            }
        }

        /// <summary>
        /// Determines whether pawn can promtoe
        /// </summary>
        /// <param name="piece">The piece that is getting promoted</param>
        /// <returns>true if the piece can promote, false if not</returns>
        public bool canPromotePawn(Piece piece)
        {
            // If the piece is a pawn
            if (piece.Type == PieceType.Pawn)
            {
                // and if it reached the end of the board
                if ((BattleType == BattleType.AIMatch && Turn == PlayerType.Player1 && piece.Y == 0)
                    || (BattleType != BattleType.AIMatch && (Turn == PlayerType.Player1 && piece.Y == 7) || (Turn == PlayerType.Player2 && piece.Y == 0)))
                {
                    // Saves the current move
                    // Prmotes the pawn
                    SaveMove(piece, piece.X, piece.Y, "promoted");
                    return true;
                }
            }
            // or else, the piece cannot promote
            return false;
        }
        /// <summary>
        /// Promtoes the pawn
        /// </summary>
        /// <param name="piece">the pawn to be promoted</param>
        public void PromotePawn(Piece piece)
        {
            // Updates the piece on the board
            Pieces[piece.X, piece.Y] = piece;
            // if the player is playign an AI match
            if (_battleType == BattleType.AIMatch)
            {
                // end the turn when the pawn promotes
                StartTurn();
            }
            // Updates the form
            Form.IsPromoting = false;
            Form.IsRefresh = true;

        }

        /// <summary>
        /// Creates the AI
        /// </summary>
        public void MakeAI()
        {
            // creates and stores an AI object to play against the player
            AI ai = new AI(this);
            //AI will be player 2
            ai.KingPiece = _p2.KingPiece;
            _p2 = ai;
        }
        /// <summary>
        /// Starts the turn so that the other player gets to move
        /// </summary>
        public void StartTurn()
        {
            // Increases the round count by 1
            _round += 1;
            // State varialbe whether the play can win or lose
            bool canWin = true;
            bool canLose = true;

            // Loops through the board
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    // Creates a piece object which equals to a piece onthe board
                    Piece currPiece = Pieces[x, y];

                    // If the piece exists and it is an enemy
                    if (currPiece != null && currPiece.Owner != _player)
                    {
                        // Creates a temporary board
                        BoardModel thisBoard = this;

                        //SelectedPiece = currPiece;
                        // Gets the piece's valid spaces
                        currPiece.GetValidSpaces(ref thisBoard, false);

                        // If there is more than one valid space
                        if (ValidSpaces.Count > 1)
                        {
                            canWin = false;
                        }
                    }

                    // If ther is a piece and it is an ally
                    if (currPiece != null && currPiece.Owner == _player)
                    {
                        // Create a temporary board
                        BoardModel thisBoard = this;

                        //SelectedPiece = currPiece;
                        // Gets the valid spaces for the piece
                        currPiece.GetValidSpaces(ref thisBoard, false);
                        // If there is more than 1 valid space
                        if (ValidSpaces.Count > 1)
                        {

                            canLose = false;
                        }
                    }
                }
            }
            // Show the defeat screen when the player lost
            if (canLose)
            {
                this.Form.ShowDefeat();
            }
            // Show the victory screen whe nthe player won
            if (canWin)
            {
                this.Form.ShowVictory();
            }
            // The other player's turn to move
            if (_turn == PlayerType.Player1)
            {
                _turn = PlayerType.Player2;
            }
            else
            {
                _turn = PlayerType.Player1;
            }

            // If player is playing against AI, and if AI has not moved yet in its turn
            if (this.BattleType == BattleType.AIMatch && !AIMoved && _turn == PlayerType.Player2)
            {
                // The AI moves
                BoardModel thisBoard = this;
                ((AI)Player2).ExecuteMoves(ref thisBoard);
            }

            //Starts the other player's turn.
            CurrentPlayer.StartTurn();

            // Clears all valid spaces
            ValidSpaces.Clear();
        }

        // stores whether AI has moved or not
        private bool _aiMoved = false;
        /// <summary>
        /// Allows aiMoved to be visible to other classes
        /// </summary>
        public bool AIMoved
        {
            get
            {
                return _aiMoved;
            }
            // can be changed externally
            set
            {
                _aiMoved = value;
            }
        }
        /// <summary>
        /// Determines which player goes first
        /// </summary>
        /// <returns>0 or 1, indicates the order of which the player moves</returns>
        public int CoinFlip()
        {
            // Creates the random number generator
            Random numberGenerator = new Random();
            // Randomly choose 0 or 1
            int value = numberGenerator.Next(0, 2);

            //Form.ShowCoinFlip(value);
            // Shows the coin flip screen
            Form.FlipCoin(value);
            // Returns the value
            return value;
       } 
        /// <summary>
        /// Saves a move to the move log
        /// </summary>
        /// <param name="piece">the piece that is moving</param>
        /// <param name="x">the x coordinate of the piece</param>
        /// <param name="y">the y coordinate of the piece</param>
        /// <param name="action">what the piece does</param>
        private void SaveMove(Piece piece, int x, int y, string action)
        {
            // Saves the move log
            _moveLog += piece.Owner.ToString() + "'s " + piece.Type.ToString() + " " + action + " (" + x.ToString() + ", " + y.ToString() + ")" + "\n";
        }
        /// <summary>
        /// Refreshes the list of servers
        /// </summary>
        /// <param name="val">the value of the server</param>
        public void RefreshServer(int val)
        {
            // Get the list of open servers
            _openServers = _reciever.SearchServers(val);

            // Gets the server info from the list of the servers
            for (int i = 0; i < _openServers.Count; ++i)
            {
                ServerInfo info = _openServers[i];
            }

        }
        /// <summary>
        /// The player joins an existing server
        /// </summary>
        /// <param name="name">the name of the player</param>
        public void JoinServer(string name)
        {
            // Finds the index
            int index = name.Remove(0, 13).ToCharArray()[0] - '0';
            // If a network exists
            if (_network != null)
            {
                // Connect to the server
                _network.Disconnect(false);
            }
            // Creates a temporary board
            BoardModel thisBoard = this;
            // Creates a new network
            _network = new Client(ref thisBoard, _openServers[index].Ip, _openServers[index].Port);
        }
        /// <summary>
        /// THe player joins an existing server
        /// </summary>
        /// <param name="ip">the ip of the server</param>
        /// <param name="port">the port of the server</param>
        public void JoinServer(string ip, int port)
        {
            // Connect to the server if a network exists
            if (_network != null)
            {
                _network.Disconnect(false);
            }
            // Creates a temporary board
            BoardModel thisBoard = this;
            // Creates a network
            _network = new Client(ref thisBoard, ip, port);
        }
        /// <summary>
        /// Creates a server
        /// </summary>
        /// <param name="name">the name of the server</param>
        public void CreateServer(string name)
        {
            // Create a temporary board
            BoardModel thisBoard = this;
            // Creates a network
            _network = new Server(ref thisBoard, name);
        }

        /// <summary>
        /// Subprogram that saves the game
        /// </summary>
        /// <param name="name">the name of the save file</param>
        /// <returns></returns>
        public void SaveGame(string name)
        {
            //If a file with the same name already exists prompt user to overwrite
            if (File.Exists(name + ".chess"))
            {
                //Pops up a message box that the current save file exists
                DialogResult dialogResult = MessageBox.Show("Would you like to overwrite " + name + ".chess ?", "Previous progress will be overwritten", MessageBoxButtons.YesNo);
                //If the user chooses not to overwrite, go back to the game
                if (dialogResult == DialogResult.No)
                {
                    //Returns back to game
                    return;
                }
            }
            //Otherwise, if there isn't a file or the user want's to overwrite
            using (StreamWriter saveFile = new StreamWriter(name + ".chess", false))
            {
                //The first line that will be saved is the settings of the game
                saveFile.WriteLine((int)Settings.Theme);
                //The second line will be how much time is left for the first player
                saveFile.WriteLine(Player1.TurnStartTime);
                //The third line will be how much time is left for the second player
                saveFile.WriteLine(Player2.TurnStartTime);
                //Loops through the 8x8 board array
                for (int i = 0; i < View.GLOBAL_SIZE; i++)
                {
                    for (int j = 0; j < View.GLOBAL_SIZE; j++)
                    {
                        //If there is a piece, save it
                        if (Pieces[i, j] != null)
                        {
                            //Saves the piece as: type_owner
                            saveFile.WriteLine((int)Pieces[i, j].Type + "_" + (int)Pieces[i, j].Owner);
                        }
                        //Otherwise, write an asterisk
                        else
                        {
                            //Writes an asterisk
                            saveFile.WriteLine("*");
                        }
                    }
                }
                //stores how many pieces player 1 has captured
                int count = Player1.PiecesCaptured.Count;
                //Writes to the file the number of pieces player 1 has captured
                saveFile.WriteLine(count);
                //Loops thorugh all of the captured pieces
                for (int i = 0; i < count; i++)
                {
                    //Saves the captured piece as: type_owner
                    saveFile.WriteLine((int)Player1.PiecesCaptured[i].Type + "_" + (int)Player1.PiecesCaptured[i].Owner);
                }
                //re-initializes the count for player 2
                count = Player2.PiecesCaptured.Count;
                //Writes player 2's captured pieces
                saveFile.WriteLine(count);
                //Loops through all of the captured pieces
                for (int i = 0; i < count; i++)
                {
                    //Saves the captured piece as: type_owner
                    saveFile.WriteLine((int)Player2.PiecesCaptured[i].Type + "_" + (int)Player2.PiecesCaptured[i].Owner);
                }
            }
            //Creates or appends to the file of saved names
            using (StreamWriter saveName = new StreamWriter("_savedNames.txt", true))
            {
                //Writes the name of the saved file
                saveName.WriteLine(name);
            }
            //display that the game has been saved successfully
            DialogResult saveResult = MessageBox.Show("Game saved as " + name + ".chess", "Game has been saved successfully", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Method to read the saved game file
        /// </summary>
        /// <param name="name">The name of the file to open</param>
        public void LoadGame(string name)
        {
            //If the file exists, read the file
            if (File.Exists(name + ".chess") == true)
            {
                //Opens the file to perform functions and read the saved file
                using (StreamReader file = new StreamReader(name + ".chess"))
                {
                    //Stores integer to be used with the rest of the program
                    int integer;
                    //Reads the first line (Should be theme and sets the theme)
                    int.TryParse(file.ReadLine(), out integer);
                    Settings.Theme = (Theme)integer;
                    //Reads the time left from 2nd line and sets the time for player 1
                    int.TryParse(file.ReadLine(), out integer);
                    Player1.TurnStartTime = integer;
                    //Reads the time left from 2nd line and sets the time for player 2
                    int.TryParse(file.ReadLine(), out integer);
                    Player2.TurnStartTime = integer;
                    //Loops through the 2D array of saved pieces
                    for (int i = 0; i < View.GLOBAL_SIZE; i++)
                    {
                        for (int j = 0; j < View.GLOBAL_SIZE; j++)
                        {
                            //Gets information from 1 line
                            string line = file.ReadLine();
                            //Splits the line into sections as a string array
                            string[] components = line.Split('_');
                            //If it's not a blank piece, save it to the board
                            if (components[0] != "*")
                            {
                                //Convert string to integer to check the type then store the pieces
                                int[] properties = Array.ConvertAll(components, int.Parse);
                                //Check each saved piece with their respected lables and update the board
                                if (PieceType.Pawn == (PieceType)properties[0])
                                {
                                    //If the piece is a pawn, make a new pawn
                                    Pieces[i, j] = new Pawn((PlayerType)properties[1], i, j);
                                }
                                //If the saved piece is a Knight, make a new king piece on the board
                                else if (PieceType.Knight == (PieceType)properties[0])
                                {
                                    Pieces[i, j] = new Knight((PlayerType)properties[1], i, j);
                                }
                                //If the saved piece is a bishop, make a new bishop piece on the board
                                else if (PieceType.Bishop == (PieceType)properties[0])
                                {
                                    Pieces[i, j] = new Bishop((PlayerType)properties[1], i, j);
                                }
                                //If the saved piece is a queen, make a new queen piece on the board
                                else if (PieceType.Queen == (PieceType)properties[0])
                                {
                                    Pieces[i, j] = new Queen((PlayerType)properties[1], i, j);
                                }
                                //If the saved piece is a rook, make a new rook piece on the board
                                else if (PieceType.Rook == (PieceType)properties[0])
                                {
                                    Pieces[i, j] = new Rook((PlayerType)properties[1], i, j);
                                }
                                //Otherwise if it's not any piece, create an arbituary piece to be stored
                                //It will not reach this step because the player won't ever capture a king piece
                                else
                                {
                                    Pieces[i, j] = new King((PlayerType)properties[1], i, j);
                                }
                            }
                            //Otherwise there is no piece at that location
                            else
                            {
                                Pieces[i, j] = null;
                            }

                        }
                    }
                    //Checks the line of code that is how many captured pieces for player 1
                    int.TryParse(file.ReadLine(), out integer);
                    //Loops through all of the captured pieces if nessesary
                    for (int i = 0; i < integer; i++)
                    {
                        //Reads the next line that's supposed to be the captured piece
                        string line = file.ReadLine();
                        //Splits the line to its components
                        string[] components = line.Split('_');
                        //converts the components into integer for checking
                        int[] properties = Array.ConvertAll(components, int.Parse);
                        //Makes a new piece with no properties
                        Piece captured;
                        //Stores the pieces captured by checking each piece with the respected piece type
                        if (PieceType.Pawn == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a pawn to be stored
                            captured = new Pawn((PlayerType)properties[1], 0, 0);
                        }
                        else if (PieceType.Knight == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a Knight to be stored
                            captured = new Knight((PlayerType)properties[1], 0, 0);
                        }
                        else if (PieceType.Bishop == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a bishop to be stored
                            captured = new Bishop((PlayerType)properties[1], 0, 0);
                        }
                        else if (PieceType.Queen == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a queen to be stored
                            captured = new Queen((PlayerType)properties[1], 0, 0);
                        }
                        else if (PieceType.Rook == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a rook to be stored
                            captured = new Rook((PlayerType)properties[1], 0, 0);
                        }
                        //Otherwise if it's not any piece, create an arbituary piece to be stored
                        //It will not reach this step because the player won't ever capture a king piece
                        else
                        {
                            //Creates a new captured piece that's a king to be stored
                            captured = new King((PlayerType)properties[1], 0, 0);
                        }
                        //Stores the captured piece to player 1's list
                        Player1.PiecesCaptured.Add(captured);
                    }
                    //reads the line that says how many captured pieces there are for the second player
                    int.TryParse(file.ReadLine(), out integer);
                    //loops through all of the viable captured pieces
                    for (int i = 0; i < integer; i++)
                    {
                        //reads the next line for the captured piece's information
                        string line = file.ReadLine();
                        //splits the line to its component
                        string[] components = line.Split('_');
                        //converts the components to integer so they can be compared
                        int[] properties = Array.ConvertAll(components, int.Parse);
                        //Creates a new piece with no properties
                        Piece captured;
                        //Checks the piece type that is captured
                        if (PieceType.Pawn == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a pawn to be stored
                            captured = new Pawn((PlayerType)properties[1], 0, 0);
                        }
                        else if (PieceType.King == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a knight to be stored
                            captured = new Knight((PlayerType)properties[1], 0, 0);

                        }
                        else if (PieceType.Bishop == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a bishop to be stored
                            captured = new Bishop((PlayerType)properties[1], 0, 0);
                        }
                        else if (PieceType.Queen == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a queen to be stored
                            captured = new Queen((PlayerType)properties[1], 0, 0);
                        }
                        else if (PieceType.Rook == (PieceType)properties[0])
                        {
                            //Creates a new captured piece that's a rook to be stored
                            captured = new Rook((PlayerType)properties[1], 0, 0);
                        }
                        //Otherwise if it's not any piece, create an arbituary piece to be stored
                        //It will not reach this step because the player won't ever capture a king piece
                        else
                        {
                            //Creates a new captured piece that's a king to be stored
                            captured = new King((PlayerType)properties[1], 0, 0);
                        }
                        //Adds the capture piece to player 2's captured piece list
                        Player2.PiecesCaptured.Add(captured);
                    }
                }

            }
            //Otherwise if the file does not exist/unreadable
            else
            {
                MessageBox.Show("Error: Save file is unreadable");
            }
        }

    }
}
