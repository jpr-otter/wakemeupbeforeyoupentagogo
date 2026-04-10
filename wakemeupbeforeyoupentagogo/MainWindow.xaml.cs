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
            Board = new Board();
            HideArrows();
            DisableBoard();
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

        public void EnableBoard()
        {
            foreach (var quadrant in squareButtons)
                foreach (var btn in quadrant)
                    btn.IsEnabled = true;
        }

        public void DisableBoard()
        {
            foreach (var quadrant in squareButtons)
                foreach (var btn in quadrant)
                    btn.IsEnabled = false;
        }

        public void StartNewGame()
        {
            if (Board != null)
            {
                Board.RestartGame();
                BlackMovement = false;
                SyncBoardToUI();
                EnableBoard();
                HideArrows();
            }
            if (Play != null)
            {
                Play.ResetUI();
            }
        }

        public void SyncBoardToUI()
        {
            for (int q = 0; q < 4; q++)
            {
                for (int i = 0; i < Board.RectSize; i++)
                {
                    StoneColor color = Board.Quadrants[q][i];
                    if (color == StoneColor.Black) squareButtons[q][i].Background = Brushes.Black;
                    else if (color == StoneColor.White) squareButtons[q][i].Background = Brushes.White;
                    else squareButtons[q][i].Background = Brushes.Transparent;
                }
            }
        }

        private void CheckGameState()
        {
            var result = Board.CheckGameResult();
            if (result != GameResult.None)
            {
                DisableBoard();
                if (result == GameResult.Draw) Play.ShowWinner("Draw!");
                else if (result == GameResult.WhiteWins) Play.ShowWinner("White won!");
                else if (result == GameResult.BlackWins) Play.ShowWinner("Black won!");
            }
        }

        private void ButtonStone_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Background != Brushes.Transparent)
                return;

            ToggleStoneColor(button);
            DisableBoard();
            
            // Map button click backward to Board
            for (int q = 0; q < 4; q++)
            {
                for (int i = 0; i < Board.RectSize; i++)
                {
                    if (squareButtons[q][i] == button)
                    {
                        Board.Quadrants[q][i] = button.Background == Brushes.Black ? StoneColor.Black : StoneColor.White;
                    }
                }
            }

            if (Board.CheckGameResult() == GameResult.None)
                SetArrowsVisibility(Visibility.Visible);
            else
                CheckGameState();
        }

        private void ToggleStoneColor(Button button)
        {
            button.Background = BlackMovement ? Brushes.Black : Brushes.White;
            BlackMovement = !BlackMovement;
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
            EnableBoard();

            var button = sender as Button;
            var direction = button.Name[12] == 'N' ? RotationDirection.CounterClockwise : RotationDirection.Clockwise;
            var squareNumber = button.Name[11] - '1';

            Board.Rotation(direction, squareNumber);
            SyncBoardToUI();

            if (Board.CheckGameResult() != GameResult.None)
            {
                CheckGameState();
                return;
            }

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            if (Menu.AgainstComputer)
            {
                DisableBoard();
                Board.ComputerMoves();
                SyncBoardToUI();
                BlackMovement = false; // Next player turn is White
                if (Board.CheckGameResult() == GameResult.None)
                {
                    EnableBoard();
                    Play.ChangeTurn();
                }
                else
                {
                    CheckGameState();
                    Play.ChangeTurn();
                }
            }
        }

        public static void HideArrows()
        {
            foreach (var arrow in arrowsButtons)
                arrow.Visibility = Visibility.Collapsed;
        }
    }
}
