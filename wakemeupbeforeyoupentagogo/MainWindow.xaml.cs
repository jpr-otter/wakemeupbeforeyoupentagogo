using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Pentago
{
    public partial class MainWindow : Window
    {
        private Button[] squareButton1;
        private Button[] squareButton2;
        private Button[] squareButton3;
        private Button[] squareButton4;
        private static Button[]? arrowsButton;
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
            InitializeSquareButtons();
            InitializeArrowButtons();
            Board = new Board(squareButton1, squareButton2, squareButton3, squareButton4);
            HideArrows();
            Board.IsNotEnabled();
            BlackMovement = false;
        }

        private void InitializeSquareButtons()
        {
            squareButton1 = InitializeButtonArray(new[] { Button11, Button12, Button13, Button14, Button15, Button16, Button17, Button18, Button19 });
            squareButton2 = InitializeButtonArray(new[] { Button21, Button22, Button23, Button24, Button25, Button26, Button27, Button28, Button29 });
            squareButton3 = InitializeButtonArray(new[] { Button31, Button32, Button33, Button34, Button35, Button36, Button37, Button38, Button39 });
            squareButton4 = InitializeButtonArray(new[] { Button41, Button42, Button43, Button44, Button45, Button46, Button47, Button48, Button49 });
        }

        private Button[] InitializeButtonArray(Button[] buttons)
        {
            var buttonArray = new Button[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                buttonArray[i] = buttons[i];
            }
            return buttonArray;
        }

        private void InitializeArrowButtons()
        {
            arrowsButton = InitializeButtonArray(new[] { ButtonArrow1C, ButtonArrow1N, ButtonArrow2C, ButtonArrow2N,
                                                         ButtonArrow3C, ButtonArrow3N, ButtonArrow4C, ButtonArrow4N });
        }

        private void ButtonStone_Click(object sender, RoutedEventArgs e)
        {
            ToggleStoneColor(sender);
            Board.IsNotEnabled();
            ShowArrowsIfNoWinner();
        }

        private void ToggleStoneColor(object sender)
        {
            var button = sender as Button;
            button.Background = BlackMovement ? Brushes.Black : Brushes.White;
            BlackMovement = !BlackMovement;
        }

        private void ShowArrowsIfNoWinner()
        {
            if (!Board.PresentWinner())
            {
                SetArrowsVisibility(Visibility.Visible);
            }
        }

        private void SetArrowsVisibility(Visibility visibility)
        {
            foreach (Button arrow in arrowsButton)
            {
                arrow.Visibility = visibility;
            }
        }


        private void ButtonArrow_Click(object sender, RoutedEventArgs e)
        {
            HideArrows();
            Play.ChangeTurn();
            Board.IsEnabled();
            string nameButton = (sender as Button).Name;

            char directionChar = nameButton[12];
            string direction = "";
            if (directionChar == 'N')  //lolololol
            {
                direction = "counterclockwise";
            }
            else if (directionChar == 'C')
            {
                direction = "clockwise";
            }

            char NumberSquare = nameButton[11];
            Board.Rotation(direction, NumberSquare - 49);
            if (Board.PresentWinner()) return;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            if (Menu.AgainstComputer == true)
            {
                Board.ComputerMoves();
            }
        }

        public static void HideArrows()
        {
            foreach (Button arrow in arrowsButton)
            {
                arrow.Visibility = Visibility.Collapsed;
            }
        }
    }
}

