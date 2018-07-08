using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools {

    public static GameObject GetPole(int x, int y)
    {
        return GameObject.Find("Pole" + x + y);
    }

    public static int PiecesOnPole(int x, int z, int[,,] gameBoard)
    {
        int count = 0;
        for (int i = 0; i < gameBoard.GetLength(1); i++)
        {
            count += (int)Mathf.Abs(gameBoard[x, i, z]);
        }
        return count;
    }

    public static bool CheckBoardWin(int[,,] board)
    {
        int size = board.GetLength(0);
        for (int side = 0; side < 3; side++)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (CheckStraightWin(side, i, j, board))
                        return true;
                }
            }
        }
        for (int i = 0; i < size; i++)
        {
            for (int side = 0; side < 3; side++)
            {
                if (CheckDiagWin(side, i, board))
                    return true;
            }
        }
        return CheckMultiDiagonals(board);
    }

    static bool CheckStraightWin(int side, int x, int y, int[,,] gameBoard)
    {
        int size = gameBoard.GetLength(0);

        int sum = 0;
        if (side == 0)
        {
            for (int i = 0; i < size; i++)
                sum += gameBoard[x, y, i];
        }
        else if (side == 1)
        {
            for (int i = 0; i < size; i++)
                sum += gameBoard[x, i, y];
        }
        else
        {
            for (int i = 0; i < size; i++)
                sum += gameBoard[i, x, y];
        }
        return (sum == 4 || sum == -4);
    }

    static bool CheckDiagWin(int side, int level, int[,,] gameBoard)
    {
        int size = gameBoard.GetLength(0);

        int sum1 = 0;
        int sum2 = 0;
        if (side == 0)
        {
            for (int i = 0; i < size; i++)
            {
                int j = size - i - 1;
                sum1 += gameBoard[level, i, i];
                sum2 += gameBoard[level, i, j];
            }
        }
        else if (side == 1)
        {
            for (int i = 0; i < size; i++)
            {
                int j = size - i - 1;
                sum1 += gameBoard[i, level, i];
                sum2 += gameBoard[i, level, j];
            }
        }
        else
        {
            for (int i = 0; i < size; i++)
            {
                int j = size - i - 1;
                sum1 += gameBoard[i, i, level];
                sum2 += gameBoard[i, j, level];
            }
        }
        return (Mathf.Abs(sum1) == 4 || Mathf.Abs(sum2) == 4);
    }

    static bool CheckMultiDiagonals(int[,,] gameBoard)
    {
        int size = gameBoard.GetLength(1);
        int[] sums = new int[4];
        for (int i = 0; i < size; i++)
        {
            int j = size - 1 - i;
            sums[0] += gameBoard[i, i, i];
            sums[1] += gameBoard[i, i, j];
            sums[2] += gameBoard[i, j, i];
            sums[3] += gameBoard[j, i, i];
        }
        foreach (int sum in sums)
            if (sum == 4 || sum == -4)
                return true;
        return false;
    }
}