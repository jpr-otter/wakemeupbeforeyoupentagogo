using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wakemeupbeforeyoupentagogo;

namespace Pentago
{
    public class Board
    {
        private Button[][] rectButton;
        private Button[] allRects;
        private Button blackMoveButton;
        private int[] blackMoveRotation;
        static int rectSize = 9;

        public Board(Button[] topRowButtons, Button[] secondRowButtons, Button[] thirdRowButtons, Button[] bottomRowButtons)
        {
            rectButton = new Button[4][];
            rectButton[0] = topRowButtons;
            rectButton[1] = secondRowButtons;
            rectButton[2] = thirdRowButtons;
            rectButton[3] = bottomRowButtons;
            blackMoveButton = new Button();
            blackMoveRotation = new int[2];
            allRects = new Button[rectSize * 4];

            for (int quadrant = 0; quadrant < 2; quadrant++)
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int column = 0; column < 3; column++)
                    {
                        allRects[column + (row * 6) + (18 * quadrant)] = rectButton[2 * quadrant][column + (row * 3)];
                    }
                    for (int column = 0; column < 3; column++)
                    {
                        allRects[column + 3 + (row * 6) + (18 * quadrant)] = rectButton[2 * quadrant + 1][column + (row * 3)];
                    }
                }
            }
        }

        public void IsNotEnabled()
        {

            foreach (Button[] squares in rectButton)
            {
                foreach (Button ball in squares)
                {
                    ball.IsEnabled = false;
                }
            }
        }
        public void IsEnabled()
        {

            foreach (Button[] squares in rectButton)
            {
                foreach (Button ball in squares)
                {
                    ball.IsEnabled = true;
                }
            }
        }

        public void Rotation(string direction, int square)
        {
            Brush[] copySquare = new Brush[rectSize];
            for (int i = 0; i < rectSize; i++)
            {
                copySquare[i] = rectButton[square][i].Background;
            }

            if (direction.Equals("counterclockwise", StringComparison.OrdinalIgnoreCase))
            {
                RotateCounterClockwise(square, copySquare);
            }
            else if (direction.Equals("clockwise", StringComparison.OrdinalIgnoreCase))
            {
                RotateClockwise(square, copySquare);
            }
            else
            {
                throw new ArgumentException("Invalid rotation direction. Use 'clockwise' or 'counterclockwise'.");
            }
        }

        private void RotateCounterClockwise(int square, Brush[] copySquare)
        {
            int index = 0;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 6; col >= 0; col -= 3)
                {
                    rectButton[square][col + row].Background = copySquare[index++];
                }
            }
        }

        private void RotateClockwise(int square, Brush[] copySquare)
        {
            int index = 0;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 6; col >= 0; col -= 3)
                {
                    rectButton[square][index++].Background = copySquare[row + col];
                }
            }
        }


        public bool CheckWinningCondition(Brush color)
        {
            // Check horizontal and vertical lines
            for (int i = 0; i < 6; i++)
            {
                if (CheckLine(color, i * 6, 1) || CheckLine(color, i, 6))
                    return true;
            }

            // Check diagonal lines
            if (CheckLine(color, 0, 7) || CheckLine(color, 4, 5))
                return true;

            // Check anti-diagonal lines
            if (CheckLine(color, 2, 5) || CheckLine(color, 6, 7))
                return true;

            return false;
        }

        private bool CheckLine(Brush color, int startIndex, int step)
        {
            int stoneInPosition = 0;

            for (int i = 0; i < 5; i++)
            {
                if (allRects[startIndex + i * step].Background == color)
                {
                    stoneInPosition++;
                }
                else
                {
                    stoneInPosition = 0;
                }

                if (stoneInPosition == 5)
                    return true;
            }

            return false;
        }


        public bool IsBoardFull()
        {
            foreach (Button stone in allRects)
            {
                if (stone.Background == Brushes.Transparent) return false;
            }
            return true;
        }

        public bool PresentWinner()
        {
            bool whiteWins = CheckWinningCondition(Brushes.White);
            bool blackWins = CheckWinningCondition(Brushes.Black);
            if (whiteWins == true && blackWins == true)
            {
                MainWindow.Play.ShowWinner("Draw!");
                IsNotEnabled();
                return true;
            }
            else if (whiteWins)
            {
                MainWindow.Play.ShowWinner("White won!");
                IsNotEnabled();
                return true;
            }
            else if (blackWins)
            {
                MainWindow.Play.ShowWinner("Black won!");
                IsNotEnabled();
                return true;
            }
            else if (IsBoardFull())
            {
                MainWindow.Play.ShowWinner("Draw!");
                IsNotEnabled();
                return true;
            }
            return false;
        }

        public void RestartGame(bool restartGame)
        {
            MainWindow.BlackMovement = false;
            foreach (Button ball in allRects)
            {
                ball.Background = Brushes.Transparent;
            }
            MainWindow.HideArrows();
            if (restartGame)
            {
                IsEnabled();
            }
            else IsNotEnabled();
        }

        public void ComputerTurns()
        {
            if (ThreeInLine(Brushes.White))
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                if (rectButton[i][4].Background == Brushes.Transparent)
                {
                    rectButton[i][4].Background = Brushes.Black;
                    blackMoveButton = rectButton[i][4];
                    return;
                }
            }
            Random random = new Random();
            while (true)
            {
                int number = random.Next(36);
                if (allRects[number].Background == Brushes.Transparent)
                {
                    allRects[number].Background = Brushes.Black;
                    blackMoveButton = allRects[number];
                    break;
                }
            }
        }

        public void ComputerTurnsArrow()
        {

            Random random = new Random();
            int direction = random.Next(2);
            int square = random.Next(4);
            blackMoveRotation[0] = direction;
            blackMoveRotation[1] = square;
            if (direction == 0) Rotation("counterclockwise", square);
            else Rotation("clockwise", square);
            IsEnabled();
        }

        private bool CheckWinnerWhite()
        {
            foreach (Button ball in allRects)
            {
                if (ball.Background == Brushes.Transparent)
                {
                    ball.Background = Brushes.White;
                    for (int i = 0; i < 4; i++)
                    {
                        Rotation("counterclockwise", i);
                        if (CheckWinningCondition(Brushes.White))
                        {
                            Rotation("clockwise", i);
                            ball.Background = Brushes.Transparent;
                            return true;
                        }
                        else
                        {
                            Rotation("clockwise", i);
                            Rotation("clockwise", i);
                            if (CheckWinningCondition(Brushes.Black))
                            {
                                Rotation("counterclockwise", i);
                                ball.Background = Brushes.Transparent;
                                return true;
                            }
                            else
                            {
                                Rotation("counterclockwise", i);

                            }
                        }
                    }
                    ball.Background = Brushes.Transparent;
                }
            }
            return false;
        }

        public void WhiteWins()
        {

            if (CheckWinnerWhite() == false) return;
            blackMoveButton.Background = Brushes.Transparent;
            if (blackMoveRotation[0] == 0)
            {
                Rotation("clockwise", blackMoveRotation[1]);
            }
            else
            {
                Rotation("counterclockwise", blackMoveRotation[1]);
            }
            bool notWin;
            foreach (Button Blackball in allRects)
            {
                if (Blackball.Background == Brushes.Transparent)
                {
                    Blackball.Background = Brushes.Black;
                    for (int j = 0; j < 4; j++)
                    {
                        notWin = true;
                        for (int k = 0; k < 2; k++)
                        {
                            if (k == 0)
                            {
                                Rotation("clockwise", j);
                            }
                            else
                            {
                                Rotation("counterclockwise", j);
                                Rotation("counterclockwise", j);
                            }
                            foreach (Button Whiteball in allRects)
                            {
                                if (Whiteball.Background == Brushes.Transparent)
                                {
                                    Whiteball.Background = Brushes.White;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        Rotation("counterclockwise", i);
                                        if (CheckWinningCondition(Brushes.White))
                                        {
                                            notWin = false;
                                            Rotation("clockwise", i);
                                        }
                                        else
                                        {
                                            Rotation("clockwise", i);
                                            Rotation("clockwise", i);
                                            if (CheckWinningCondition(Brushes.White))
                                            {
                                                notWin = false;
                                            }
                                            Rotation("counterclockwise", i);
                                        }
                                    }
                                    Whiteball.Background = Brushes.Transparent;
                                }
                            }
                            if (notWin == true)
                            {
                                return;
                            }
                            if (k == 1)
                            {
                                Rotation("clockwise", j);
                            }

                        }
                    }
                    Blackball.Background = Brushes.Transparent;
                }
            }
            blackMoveButton.Background = Brushes.Black;
            Rotation("clockwise", 2);
        }

        public bool BlackWins()
        {
            foreach (Button ball in allRects)
            {
                if (ball.Background == Brushes.Transparent)
                {
                    ball.Background = Brushes.Black;
                    for (int i = 0; i < 4; i++)
                    {
                        Rotation("counterclockwise", i);
                        if (CheckWinningCondition(Brushes.Black))
                        {
                            return true;
                        }
                        else
                        {
                            Rotation("clockwise", i);
                            Rotation("clockwise", i);
                            if (CheckWinningCondition(Brushes.Black))
                            {
                                return true;
                            }
                            else
                            {
                                Rotation("counterclockwise", i);

                            }
                        }
                    }
                    ball.Background = Brushes.Transparent;
                }
            }
            return false;
        }

        public void ComputerMoves()
        {
            IsNotEnabled();
            MainWindow.BlackMovement = !MainWindow.BlackMovement;
            if (!BlackWins())
            {
                ComputerTurns();
                ComputerTurnsArrow();
                WhiteWins();
            }
            PresentWinner();
            MainWindow.Play.ChangeTurn();
        }

        public bool ThreeInLine(Brush colorInLine)
        {
            for (int i = 0; i < 4; i++)
            {
                if (CheckAndCompleteLine(i, colorInLine))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckAndCompleteLine(int index, Brush colorInLine)
        {
            // Define the patterns to check for a three-in-line condition
            int[][] patterns = new int[][]
            {
                new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 }, // Rows
                new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 }, // Columns
                new[] { 0, 4, 8 }, new[] { 2, 4, 6 }                      // Diagonals
            };

            foreach (var pattern in patterns)
            {
                if (TryCompletePattern(index, pattern, colorInLine))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryCompletePattern(int index, int[] positions, Brush colorInLine)
        {
            Button[] buttons = rectButton[index];
            int countColorInLine = positions.Count(pos => buttons[pos].Background == colorInLine);
            int countTransparent = positions.Count(pos => buttons[pos].Background == Brushes.Transparent);

            if (countColorInLine == 2 && countTransparent == 1)
            {
                int transparentPosition = positions.First(pos => buttons[pos].Background == Brushes.Transparent);
                buttons[transparentPosition].Background = Brushes.Black;
                blackMoveButton = buttons[transparentPosition];
                return true;
            }
            return false;
        }
    }
}
