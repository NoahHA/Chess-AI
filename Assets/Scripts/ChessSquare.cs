using UnityEngine;
using System;

public class ChessSquare
{
    private int _col;
    private int _row;
    private String _squareName;

    [Tooltip("The name of the square in chess notation e.g. 'e4'")]
    public String SquareName
    {
        get { return ((Char)(Col + 96)).ToString() + Row.ToString(); }
        set { _squareName = value; }
    }

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
    public Vector2 ScreenPosition;

    public ChessSquare(int col, int row)
    {
        (Col, Row) = (col, row);
        ScreenPosition = new Vector2(Col * 3.5f, Row * 3.5f);
    }

    public ChessSquare(String square)
    {
        SquareName = square;
        Col = (int)square[0] - 96;
        Row = (int)Char.GetNumericValue(square[1]);
        ScreenPosition = new Vector2(Col * 3.5f, Row * 3.5f);
    }

    public ChessSquare(Vector2 screenPosition)
    {
        ScreenPosition = screenPosition;
        Col = (int)(screenPosition.x / 3.5f);
        Row = (int)(screenPosition.y / 3.5f);
    }
}