using Chess.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor method of MainWindow class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //Set the width and height of the MainWindow to 85% of the user screen
            this.Width = SystemParameters.FullPrimaryScreenWidth * 0.85;
            this.Height = SystemParameters.FullPrimaryScreenHeight * 0.85;

            this.DataContext = new ChessBoardVM();
        }

        /// <summary>
        /// Method for allowing dragging the window by left-mouse click
        /// </summary>
        /// <param name="sender">The object that send the request for action</param>
        /// <param name="e">Event argument</param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}