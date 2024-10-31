using Chess.Models.Game;
using Chess.Util.ModelUtil;

namespace Chess.ViewModels
{
    /// <summary>
    /// The View Model class: indicated each Square in the chessboard
    /// </summary>
    internal class SquareVM : ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Backing field: The nullable Piece currently in this square (if null, it means that this square is empty (has no piece reside))
        /// </summary>
        private Piece? _piece;
        /// <summary>
        /// Gets or sets the Piece reside in this square. This property is nullable
        /// </summary>
        public Piece? Piece
        {
            get => _piece;
            set
            {
                if (_piece != value)
                {
                    _piece = value;
                    //If there is a piece move to or leave this square, notify the View
                    OnPropertyChanged(nameof(Piece));
                }
            }
        }

        /// <summary>
        /// Gets the Position of this square in the chessboard
        /// </summary>
        public Position Position { get; }

        /// <summary>
        /// Gets or sets the background of the square. Note that this is not including the highlight case
        /// </summary>
        public string Background {  get; set; }

        /// <summary>
        /// Backing field: a boolean value to check if the current piece in this square (if any) is being selected. 
        /// The default value is false (no piece being selected at the start of the game)
        /// </summary>
        private bool _isPieceSelected = false;
        /// <summary>
        /// Gets or sets the value of _isPieceSelected
        /// </summary>
        public bool IsPieceSelected
        {
            get => _isPieceSelected;
            set
            {
                _isPieceSelected = value;
                //After the value change (a select or de-select event), notify the UI
                OnPropertyChanged(nameof(IsPieceSelected));
            }
        }

        /// <summary>
        /// Backing field: a boolean value to check if this square is highlighted. 
        /// Note that a selected piece cannot have its square highlight and vice versa. 
        /// The default value is false (no square is being highlight at the start of the game) 
        /// </summary>
        private bool _isHighlighted = false;
        /// <summary>
        /// Gets or sets the value of _isHighlighted 
        /// </summary>
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                _isHighlighted = value;
                //If the square is being highlighted, or unhighlighted, notify the UI to update
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }

        /// <summary>
        /// Backing field: the source of the image of the Piece currently located in this Square. 
        /// If there is no Piece in this square, it will be an empty string instead of null
        /// </summary>
        private string _imageSource = default!;
        /// <summary>
        /// Gets or sets the source of Piece's image
        /// </summary>
        public string ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                //If there is an action that leads to a change of image source (either moving, being taken or pawn promotion), notify the UI
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        /// <summary>
        /// Gets the ChessBoardVM object.
        /// </summary>
        public ChessBoardVM ChessBoardVM { get; } //We delegate all the logic to ChessBoardVM despite the Command is declared here

        /// <summary>
        /// Gets the ClickedCommand of this SquareVM. 
        /// </summary>
        public RelayCommand ClickedCommand { get; }
        #endregion //End Fields

        /// <summary>
        /// Constructor of class SquareVM
        /// </summary>
        /// <param name="pos">The Position of the Square in the chessboard. This value is fixed and should not be changed</param>
        /// <param name="piece">The nullable Piece that is currently located in this square.</param>
        /// <param name="chessBoardVM">The reference to ChessBoardVM object</param>
        /// <exception cref="ArgumentException">If pos is invalid, throw ArgumentException</exception>
        public SquareVM(Position pos, Piece? piece, ChessBoardVM chessBoardVM)
        {
            //Check if pos is a valid position
            if (!Validator.IsPositionValid(pos))
            {
                throw new ArgumentException("Position invalid for this square!");
            }

            //Assigning the value for Position
            Position = pos;
            //Assigning the reference of ChessBoardVM class to ChessBoardVM property
            ChessBoardVM = chessBoardVM;
            //Assigning the value of Piece
            Piece = piece;
            //Assigning the value for square background
            Background = GetBackground();
            //Assigning the value for piece image in that square (if any)
            ImageSource = GetImageSource();
            //Create a new RelayCommand, which receive the OnSquareClick method (action) and canExecute method
            ClickedCommand = new RelayCommand(OnSquareClick, CanExecuteOnSquareClick);
        }

        /// <summary>
        /// Helper method, used to calculate the Background of this square. 
        /// </summary>
        /// <returns>The hexa color value of background</returns>
        private string GetBackground()
        {
            //If the first square is the lighter color, then the rule is: (row + col) is even -> light, else dark
            return (Position.Row + Position.Column) % 2 != 0 ? "#EBECD0" : "#779556";
        }

        /// <summary>
        /// Helper method, used to calculate the ImageSource for the Piece located in this square (if any)
        /// </summary>
        /// <returns>The source of the image. It should be located at Chess/assets/pieces</returns>
        public string GetImageSource()
        {
            return Piece is null ? "" :
                $"pack://application:,,,/assets/pieces/{Piece.Color.ToString()[..1].ToLower()}-{Piece.Rank.ToString().ToLower()}.png";
        }

        /// <summary>
        /// The action for click command in this square, which will we passed to the RelayCommand constructor as _execute delegate
        /// </summary>
        /// <param name="parameter">The nullable object parameter</param>
        public void OnSquareClick(object? parameter)
        {
            //Delegate the actual decision to ChessBoardVM for handling
            ChessBoardVM.OnClick(this);
        }

        /// <summary>
        /// The constraint on the action that can be taken or not when a clicked event occurs. This will be passed to RelayCommand constructor 
        /// as _canExecute delegate
        /// </summary>
        /// <param name="parameter">The nullable object parameter</param>
        /// <returns>Boolean value indicates whether this action can be executed or not</returns>
        public bool CanExecuteOnSquareClick(object? parameter)
        {
            /*
             * If the current turn is not the Player's turn, then we disallow them for performing any action. More specifically,
             * 1. If this is PvP mode, then we check for the turn with current player's side
             * 2. If this PvE mode (playing with bot), then it's simpler, we only check if this is the White turn or not (bot is Black)
             */

            //Still in development phase, so we return true here for easier development
            return true;
        }

        //Testing purpose
        public override string ToString()
        {
            return $"Square {Position} holding piece: {Piece?.ToString()} -> Is selected: {IsPieceSelected} -> Is highlighted: {IsHighlighted}";
        }
    }
}
