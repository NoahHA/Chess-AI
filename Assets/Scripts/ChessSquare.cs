using UnityEngine;
using System;

public class ChessSquare
{
    private int _col;
    private int _row;

    public int Col
    {
        get { return _col; }
        set
        {
            if (value <= 8 && value >= 1)
            {
                _col = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public int Row
    {
        get { return _row; }
        set
        {
            if (value <= 8 && value >= 1)
            {
                _row = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public String Square;

    public ChessSquare(int col, int row)
    {
        Col = col;
        Row = row;
        Square = ((Char)(col + 96)).ToString() + row.ToString();
    }

    public ChessSquare(String square)
    {
        Square = square;
        Col = (int)square[0] - 96;
        Row = (int)Char.GetNumericValue(square[1]);
    }
}