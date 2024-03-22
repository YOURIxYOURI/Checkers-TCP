using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers_server
{
    internal class CheckerGame
    {
        private char[,] board;
        private bool isPlayer1Turn;

        public CheckerGame()
        {
            board = new char[8, 8];
            InitializeBoard();
            isPlayer1Turn = true;
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if ((row + col) % 2 != 0)
                    {
                        if (row < 3)
                            board[row, col] = 'x';
                        else if (row > 4)
                            board[row, col] = 'o';
                        else
                            board[row, col] = ' ';
                    }
                    else
                    {
                        board[row, col] = ' ';
                    }
                }
            }
        }

        public bool IsPlayer1Turn()
        {
            return isPlayer1Turn;
        }

        public bool MakeMove(string move, char player)
        {
            if (string.IsNullOrWhiteSpace(move) || move.Length != 4)
            {
                return false; 
            }

            int startCol = move[0] - 1;
            int startRow = move[1] - 1;
            int endCol = move[2] - 1;
            int endRow = move[3] - 1;
            if (IsValidMove(startRow, startCol, endRow, endCol, player))
            {
                board[endRow, endCol] = player;
                board[startRow, startCol] = ' ';

                if ((endRow == 0 && player == 'o') || (endRow == 7 && player == 'x'))
                {
                    board[endRow, endCol] = char.ToUpper(player);
                }

                isPlayer1Turn = !isPlayer1Turn;
                return true;
            }
            return false;
        }

        private bool IsValidMove(int startRow, int startCol, int endRow, int endCol, char player)
        {

            if (startCol < 1 || startCol > 8||
                startRow < 1 || startRow > 8 ||
                endCol < 1 || endCol > 8||
                endRow < 1 || endRow > 8)
            {
                return false; 
            }

            if (board[startRow, startCol] != ' ' || board[startRow, startCol] != player)
            {
                return false;
            }
            else
            {
                if (board[endRow, endCol] == ' ')
                {
                    
                }
                else
                {
                    return false;
                }
            }

            return true; 
        }

        public string GetBoardState()
        {
            StringBuilder builder = new StringBuilder();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    builder.Append(board[row, col]);
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
