using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
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

namespace Pentago
{

    public partial class Play : Page
    {
        public Play()
        {
            InitializeComponent();
        }

        private void BackMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Progress will be lost.", "Pentago", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(MainWindow.Menu);
                ((MainWindow)Application.Current.MainWindow).StartNewGame();
                ((MainWindow)Application.Current.MainWindow).DisableBoard();
            }

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Progress will be lost.", "Pentago", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        public void ChangeTurn()
        {
            MoveButton.Background = (MoveButton.Background == Brushes.White) ? Brushes.Black : Brushes.White;

        }

        public void ResetUI()
        {
            MoveButton.Visibility = Visibility.Visible;
            MoveButton.Background = Brushes.White;
            if (WhosTurnTextBlock != null)
            {
                WhosTurnTextBlock.Text = "Turn of";
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).StartNewGame();
        }

        public void ShowWinner(string winner)
        {
            WhosTurnTextBlock.Text = winner;
            MoveButton.Visibility = Visibility.Hidden;
        }
    }
}
