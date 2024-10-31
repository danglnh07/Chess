using Chess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess.Views
{
    /// <summary>
    /// Interaction logic for PawnPromotionPopup.xaml
    /// </summary>
    public partial class PawnPromotionPopup : UserControl
    {
        public PawnPromotionPopup()
        {
            InitializeComponent();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            // Define button names for easy reference
            string[] buttonNames = { "queen_btn", "bishop_btn", "knight_btn", "rook_btn" };

            // Clear the borders on all promotion buttons
            foreach (string buttonName in buttonNames)
            {
                Button button = (Button)this.FindName(buttonName);
                if (button != null)
                {
                    // Access the Border within each button's ControlTemplate and reset its thickness
                    Border border = (Border)button.Template.FindName("ChessButtonBorder", button);
                    if (border != null)
                    {
                        border.BorderThickness = new Thickness(0);
                    }
                }
            }

            // Set border on the clicked button
            Button clickedButton = (Button)sender;
            Border clickedBorder = (Border)clickedButton.Template.FindName("ChessButtonBorder", clickedButton);
            if (clickedBorder != null)
            {
                clickedBorder.BorderThickness = new Thickness(3);
                clickedBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }


    }
}
