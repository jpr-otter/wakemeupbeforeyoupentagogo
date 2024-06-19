using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pentago
{
    public partial class MainWindow : Window
    {
        private Button[][] squareButtons;
        private static Button[]? arrowsButtons;
        public static Board? Board { get; private set; }
        public static Menu? Menu { get; private set; }
        public static Play? Play { get; private set; }
        public static bool BlackMovement { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Menu = new Menu();
            Play = new Play();
            MenuFrame.NavigationService.Navigate(Menu);
            InitializeButtons();
            Board = new Board(squareButtons[0], squareButtons[1], squareButtons[2], squareButtons[3]);
            HideArrows();
            Board.IsNotEnabled();
            BlackMovement = false;
        }

        private void InitializeButtons()
        {
            squareButtons = new Button[][]
            {
                new[] { Button11, Button12, Button13, Button14, Button15, Button16, Button17, Button18, Button19 },
                new[] { Button21, Button22, Button23, Button24, Button25, Button26, Button27, Button28, Button29 },
                new[] { Button31, Button32, Button33, Button34, Button35, Button36, Button37, Button38, Button39 },
                new[] { Button41, Button42, Button43, Button44, Button45, Button46, Button47, Button48, Button49 }
            };

            arrowsButtons = new[]
            {
                ButtonArrow1C, ButtonArrow1N, ButtonArrow2C, ButtonArrow2N,
                ButtonArrow3C, ButtonArrow3N, ButtonArrow4C, ButtonArrow4N
            };
        }

        private void ButtonStone_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Background != Brushes.Transparent)
                return;

            ToggleStoneColor(button);
            Board.IsNotEnabled();
            ShowArrowsIfNoWinner();
        }

        private void ToggleStoneColor(Button button)
        {
            button.Background = BlackMovement ? Brushes.Black : Brushes.White;
            BlackMovement = !BlackMovement;
        }

        private void ShowArrowsIfNoWinner()
        {
            if (!Board.PresentWinner())
                SetArrowsVisibility(Visibility.Visible);
        }

        private void SetArrowsVisibility(Visibility visibility)
        {
            foreach (var arrow in arrowsButtons)
                arrow.Visibility = visibility;
        }

        private void ButtonArrow_Click(object sender, RoutedEventArgs e)
        {
            HideArrows();
            Play.ChangeTurn();
            Board.IsEnabled();

            var button = sender as Button;
            var direction = button.Name[12] == 'N' ? "counterclockwise" : "clockwise";
            var squareNumber = button.Name[11] - '1';

            Board.Rotation(direction, squareNumber);

            if (Board.PresentWinner()) return;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            if (Menu.AgainstComputer)
                Board.ComputerMoves();
        }

        public static void HideArrows()
        {
            foreach (var arrow in arrowsButtons)
                arrow.Visibility = Visibility.Collapsed;
        }
    }
}
