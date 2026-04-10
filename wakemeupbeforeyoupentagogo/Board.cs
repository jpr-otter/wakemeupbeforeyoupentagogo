using System;
using System.Linq;

namespace Pentago
{
    public enum StoneColor { None, Black, White }
    public enum RotationDirection { Clockwise, CounterClockwise }
    public enum GameResult { None, BlackWins, WhiteWins, Draw }

    public class Board
    {
        public StoneColor[][] Quadrants { get; private set; }
        private int[] blackMoveRotation;

        private int compMoveQuadrant;
        private int compMoveSquare;

        public const int RectSize = 9;

        public Board()
        {
            Quadrants = new StoneColor[4][];
            blackMoveRotation = new int[2];
            for (int i = 0; i < 4; i++)
                Quadrants[i] = new StoneColor[RectSize];
        }

        public StoneColor GetFlatColor(int flatIndex)
        {
            int row = flatIndex / 6;
            int col = flatIndex % 6;
            int q = (row / 3) * 2 + (col / 3);
            int sq = (row % 3) * 3 + (col % 3);
            return Quadrants[q][sq];
        }

        public void SetFlatColor(int flatIndex, StoneColor color)
        {
            int row = flatIndex / 6;
            int col = flatIndex % 6;
            int q = (row / 3) * 2 + (col / 3);
            int sq = (row % 3) * 3 + (col % 3);
            Quadrants[q][sq] = color;
        }

        public void Rotation(RotationDirection direction, int square)
        {
            StoneColor[] copySquare = new StoneColor[RectSize];
            for (int i = 0; i < RectSize; i++)
                copySquare[i] = Quadrants[square][i];

            if (direction == RotationDirection.CounterClockwise)
                RotateCounterClockwise(square, copySquare);
            else if (direction == RotationDirection.Clockwise)
                RotateClockwise(square, copySquare);
        }

        private void RotateCounterClockwise(int square, StoneColor[] copySquare)
        {
            int index = 0;
            for (int row = 0; row < 3; row++)
                for (int col = 6; col >= 0; col -= 3)
                    Quadrants[square][col + row] = copySquare[index++];
        }

        private void RotateClockwise(int square, StoneColor[] copySquare)
        {
            int index = 0;
            for (int row = 0; row < 3; row++)
                for (int col = 6; col >= 0; col -= 3)
                    Quadrants[square][index++] = copySquare[row + col];
        }

        public bool CheckWinningCondition(StoneColor color)
        {
            for (int row = 0; row < 6; row++)
                for (int col = 0; col <= 1; col++)
                    if (CheckLine(color, row * 6 + col, 1)) return true;

            for (int col = 0; col < 6; col++)
                for (int row = 0; row <= 1; row++) 
                    if (CheckLine(color, row * 6 + col, 6)) return true;

            for (int row = 0; row <= 1; row++)
                for (int col = 0; col <= 1; col++)
                {
                    if (CheckLine(color, row * 6 + col, 7)) return true;
                    if (CheckLine(color, row * 6 + (5 - col), 5)) return true;
                }
            return false;
        }

        private bool CheckLine(StoneColor color, int startIndex, int step)
        {
            int stoneInPosition = 0;
            for (int i = 0; i < 5; i++)
            {
                if (GetFlatColor(startIndex + i * step) == color)
                {
                    stoneInPosition++;
                    if (stoneInPosition == 5) return true;
                }
                else stoneInPosition = 0;
            }
            return false;
        }

        public bool IsBoardFull()
        {
            for (int i = 0; i < 36; i++)
                if (GetFlatColor(i) == StoneColor.None) return false;
            return true;
        }

        public GameResult CheckGameResult()
        {
            bool whiteWins = CheckWinningCondition(StoneColor.White);
            bool blackWins = CheckWinningCondition(StoneColor.Black);
            if (whiteWins && blackWins) return GameResult.Draw;
            if (whiteWins) return GameResult.WhiteWins;
            if (blackWins) return GameResult.BlackWins;
            if (IsBoardFull()) return GameResult.Draw;
            return GameResult.None;
        }

        public void RestartGame()
        {
            for (int q = 0; q < 4; q++)
                for (int i = 0; i < RectSize; i++)
                    Quadrants[q][i] = StoneColor.None;
        }

        public void ComputerTurns()
        {
            if (ThreeInLine(StoneColor.White)) return;
            for (int i = 0; i < 4; i++)
            {
                if (Quadrants[i][4] == StoneColor.None)
                {
                    Quadrants[i][4] = StoneColor.Black;
                    compMoveQuadrant = i;
                    compMoveSquare = 4;
                    return;
                }
            }
            Random random = new Random();
            while (true)
            {
                int number = random.Next(36);
                if (GetFlatColor(number) == StoneColor.None)
                {
                    SetFlatColor(number, StoneColor.Black);
                    compMoveQuadrant = (number / 6 / 3) * 2 + ((number % 6) / 3);
                    compMoveSquare = ((number / 6) % 3) * 3 + ((number % 6) % 3);
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
            if (direction == 0) Rotation(RotationDirection.CounterClockwise, square);
            else Rotation(RotationDirection.Clockwise, square);
        }

        private bool CheckWinnerWhite()
        {
            for (int flat = 0; flat < 36; flat++)
            {
                if (GetFlatColor(flat) == StoneColor.None)
                {
                    SetFlatColor(flat, StoneColor.White);
                    for (int i = 0; i < 4; i++)
                    {
                        Rotation(RotationDirection.CounterClockwise, i);
                        if (CheckWinningCondition(StoneColor.White))
                        {
                            Rotation(RotationDirection.Clockwise, i);
                            SetFlatColor(flat, StoneColor.None);
                            return true;
                        }
                        else
                        {
                            Rotation(RotationDirection.Clockwise, i);
                            Rotation(RotationDirection.Clockwise, i);
                            if (CheckWinningCondition(StoneColor.Black))
                            {
                                Rotation(RotationDirection.CounterClockwise, i);
                                SetFlatColor(flat, StoneColor.None);
                                return true;
                            }
                            else Rotation(RotationDirection.CounterClockwise, i);
                        }
                    }
                    SetFlatColor(flat, StoneColor.None);
                }
            }
            return false;
        }

        public void WhiteWins()
        {
            if (!CheckWinnerWhite()) return;
            Quadrants[compMoveQuadrant][compMoveSquare] = StoneColor.None;
            if (blackMoveRotation[0] == 0) Rotation(RotationDirection.Clockwise, blackMoveRotation[1]);
            else Rotation(RotationDirection.CounterClockwise, blackMoveRotation[1]);
            
            bool notWin;
            for (int flatBlack = 0; flatBlack < 36; flatBlack++)
            {
                if (GetFlatColor(flatBlack) == StoneColor.None)
                {
                    SetFlatColor(flatBlack, StoneColor.Black);
                    for (int j = 0; j < 4; j++)
                    {
                        notWin = true;
                        for (int k = 0; k < 2; k++)
                        {
                            if (k == 0) Rotation(RotationDirection.Clockwise, j);
                            else { Rotation(RotationDirection.CounterClockwise, j); Rotation(RotationDirection.CounterClockwise, j); }
                            
                            for (int flatWhite = 0; flatWhite < 36; flatWhite++)
                            {
                                if (GetFlatColor(flatWhite) == StoneColor.None)
                                {
                                    SetFlatColor(flatWhite, StoneColor.White);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        Rotation(RotationDirection.CounterClockwise, i);
                                        if (CheckWinningCondition(StoneColor.White))
                                        {
                                            notWin = false;
                                            Rotation(RotationDirection.Clockwise, i);
                                        }
                                        else
                                        {
                                            Rotation(RotationDirection.Clockwise, i);
                                            Rotation(RotationDirection.Clockwise, i);
                                            if (CheckWinningCondition(StoneColor.White)) notWin = false;
                                            Rotation(RotationDirection.CounterClockwise, i);
                                        }
                                    }
                                    SetFlatColor(flatWhite, StoneColor.None);
                                }
                            }
                            if (notWin) return;
                            if (k == 1) Rotation(RotationDirection.Clockwise, j);
                        }
                    }
                    SetFlatColor(flatBlack, StoneColor.None);
                }
            }
            Quadrants[compMoveQuadrant][compMoveSquare] = StoneColor.Black;
            Rotation(RotationDirection.Clockwise, 2);
        }

        public bool BlackWins()
        {
            for (int flat = 0; flat < 36; flat++)
            {
                if (GetFlatColor(flat) == StoneColor.None)
                {
                    SetFlatColor(flat, StoneColor.Black);
                    for (int i = 0; i < 4; i++)
                    {
                        Rotation(RotationDirection.CounterClockwise, i);
                        if (CheckWinningCondition(StoneColor.Black))
                        {
                            SetFlatColor(flat, StoneColor.None);
                            Rotation(RotationDirection.Clockwise, i); // UNDO rotation correctly
                            return true;
                        }
                        else
                        {
                            Rotation(RotationDirection.Clockwise, i);
                            Rotation(RotationDirection.Clockwise, i);
                            if (CheckWinningCondition(StoneColor.Black))
                            {
                                SetFlatColor(flat, StoneColor.None);
                                Rotation(RotationDirection.CounterClockwise, i); // UNDO rotation correctly
                                return true;
                            }
                            else Rotation(RotationDirection.CounterClockwise, i);
                        }
                    }
                    SetFlatColor(flat, StoneColor.None);
                }
            }
            return false;
        }

        public void ComputerMoves()
        {
            if (!BlackWins())
            {
                ComputerTurns();
                ComputerTurnsArrow();
                WhiteWins();
            }
        }

        public bool ThreeInLine(StoneColor colorInLine)
        {
            for (int i = 0; i < 4; i++)
            {
                if (CheckAndCompleteLine(i, colorInLine)) return true;
            }
            return false;
        }

        private bool CheckAndCompleteLine(int index, StoneColor colorInLine)
        {
            int[][] patterns = new int[][]
            {
                new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 }, 
                new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 }, 
                new[] { 0, 4, 8 }, new[] { 2, 4, 6 }                      
            };
            foreach (var pattern in patterns)
                if (TryCompletePattern(index, pattern, colorInLine)) return true;
            return false;
        }

        private bool TryCompletePattern(int index, int[] positions, StoneColor colorInLine)
        {
            int countColorInLine = positions.Count(pos => Quadrants[index][pos] == colorInLine);
            int countNone = positions.Count(pos => Quadrants[index][pos] == StoneColor.None);
            if (countColorInLine == 2 && countNone == 1)
            {
                int nonePos = positions.First(pos => Quadrants[index][pos] == StoneColor.None);
                Quadrants[index][nonePos] = StoneColor.Black;
                compMoveQuadrant = index;
                compMoveSquare = nonePos;
                return true;
            }
            return false;
        }
    }
}
