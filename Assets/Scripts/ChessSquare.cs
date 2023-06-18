using UnityEngine;
using System;

public class ChessSquare
{
    private int _col;
    private int _row;

    [Tooltip("The name of the square in chess notation e.g. 'e4'")]
    public String SquareName => ((Char)(Col + 96)).ToString() + Row.ToString();

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

    [Tooltip("The position of the chess square on the screen.")]
    public Vector3 ScreenPosition => new Vector3(Col, Row, 0);

    public ChessSquare(int col, int row)
    {
        (Col, Row) = (col, row);
    }

    public ChessSquare(int index)
    {
        Col = (index + 1) % 8 != 0 ? (index + 1) % 8 : 8;
        Row = (int)Math.Ceiling((index + 1) / 8f);
    }

    public ChessSquare(String square)
    {
        Col = (int)square[0] - 96;
        Row = (int)Char.GetNumericValue(square[1]);
    }

    public ChessSquare(Vector3 screenPosition)
    {
        Col = (int)(screenPosition.x);
        Row = (int)(screenPosition.y);
    }

    public override string ToString()
    {
        return SquareName;
    }
}