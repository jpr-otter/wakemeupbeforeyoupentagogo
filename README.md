# Pentago

A Windows Presentation Foundation (WPF) implementation of the classic two-player strategy game Pentago, written in C#.

## About the Game
Pentago is an abstract strategy game played on a 6x6 board divided into four 3x3 quadrants. The objective is simple: be the first player to get five stones of your color in a row (horizontally, vertically, or diagonally). 

What makes Pentago unique is that after placing a stone, the player must rotate one of the four quadrants 90 degrees (either clockwise or counterclockwise). This constantly changing board state requires forward-thinking and dynamic strategies.

## Features
* Game Modes: Play against another human locally or challenge the built-in computer opponent.
* Modern UI: A polished visual experience featuring spherical, glass-like game pieces and responsive rotation controls.
* Clean Architecture: The project separates the core game logic from the graphical user interface, making the code maintainable and extensible.

## Getting Started

### Prerequisites
* .NET SDK (The project targets .NET 7.0 for Windows)
* Visual Studio or any other C# IDE that supports WPF applications.

### Installation and Execution
1. Clone this repository to your local machine.
2. Open the solution file (.sln) in Visual Studio.
3. Build and Run the project (F5).
Alternatively, you can run the application directly from the command line using the .NET CLI:
```sh
cd wakemeupbeforeyoupentagogo
dotnet run
```

## How to Play
1. Launch the application and select your game mode from the main menu ("versus Human" or "versus Computer").
2. The game starts with an empty board. White moves first by default.
3. Click on any empty circle on the board to place your stone.
4. After placing your stone, rotation arrows will appear at the edges of the board.
5. Click on an arrow to rotate the corresponding quadrant 90 degrees in the chosen direction.
6. The first player to align five stones of their color wins the game. If all spaces are filled without a winner, the game ends in a draw.