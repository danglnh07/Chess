using Chess.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Chess.ViewModels
{
    class PawnPromotionVM : ViewModelBase
    {
        #region Fields

        private bool _isVisible = false;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private string _queenImage = default!;
        public string QueenImage 
        {
            get => _queenImage; 
            set
            {
                _queenImage = value;
                OnPropertyChanged(nameof(QueenImage));
            }
        }

        private string _bishopImage = default!;
        public string BishopImage
        {
            get => _bishopImage;
            set
            {
                _bishopImage = value;
                OnPropertyChanged(nameof(BishopImage));
            }
        }

        private string _knightImage = default!;
        public string KnightImage
        {
            get => _knightImage;
            set
            {
                _knightImage = value;
                OnPropertyChanged(nameof(KnightImage)); 
            }
        }

        private string _rookImage = default!;
        public string RookImage
        {
            get => _rookImage;
            set
            {
                _rookImage = value;
                OnPropertyChanged(nameof(RookImage));
            }
        }

        public Rank? ChosenRank { get; set; }

        private ChessBoardVM _chessBoardVM;

        public RelayCommand ChoosePieceCommand { get; set; }
        public RelayCommand SubmitCommand { get; set; }
        #endregion

        public PawnPromotionVM(ChessBoardVM chessBoardVM)
        {
            //Set the imag source to empty
            QueenImage = "";
            KnightImage = "";
            BishopImage = "";
            RookImage = "";

            //Initialize the commands
            ChoosePieceCommand = new RelayCommand(Choose, null);
            SubmitCommand = new RelayCommand(Submit, CanSubmit);

            //Assigning value for chessboard view model
            _chessBoardVM = chessBoardVM;
        }

        public void GetColor(Color color)
        {
            QueenImage = $"pack://application:,,,/assets/pieces/{color.ToString()[..1].ToLower()}-queen.png";
            BishopImage = $"pack://application:,,,/assets/pieces/{color.ToString()[..1].ToLower()}-bishop.png";
            KnightImage = $"pack://application:,,,/assets/pieces/{color.ToString()[..1].ToLower()}-knight.png";
            RookImage = $"pack://application:,,,/assets/pieces/{color.ToString()[..1].ToLower()}-rook.png";
        }

        public void Choose(object? parameter)
        {
            if (parameter is not null and Rank)
            {
                ChosenRank = (Rank)parameter;
            }
        }

        public void Submit(object? parameter)
        {
            if (ChosenRank is not null)
            {
                _chessBoardVM.PawnPromote((Rank) ChosenRank);
                //Close the popup
                IsVisible = false;
            }
        }

        public bool CanSubmit(object? parameter) 
        { 
            return ChosenRank is not null;
        }
    }
}
