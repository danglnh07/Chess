using Chess.Models.Game;


namespace Chess.ViewModels
{
    internal class PieceVM : ViewModelBase
    {
        #region Fields
        private Piece _piece;
        public Piece Piece {  get { return _piece; } }

        public Position Position => _piece.Position;
        public Rank Rank => _piece.Rank;
        public Color Color => _piece.Color;
        public List<Position> CanMove => _piece.CanMove;
        public string ImageSource
        {
            get
            {
                return $"../assets/icons/{Color}-{Rank}.png";
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        #endregion

        public PieceVM(Piece piece, ChessBoardVM chessboardVM)
        {
            _piece = piece ?? throw new ArgumentNullException($"Cannot assign a null piece to PieceVM -> {nameof(piece)}");
        }

    }
}
