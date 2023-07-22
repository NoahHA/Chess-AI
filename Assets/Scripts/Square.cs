using UnityEngine;
using System;
using UnityEngine.Tilemaps;

/// <summary>
/// Represents a chess square.
/// </summary>
public record Square
{
    private int _col;
    private int _row;

    [Tooltip("The name of the square in chess notation e.g. 'e4'")]
    public String Name => ((Char)(Col + 96)).ToString() + Row.ToString();

    [Tooltip("The index of the board square, ranges from 0-63")]
    public int Index => (Col - 1) + (Row - 1) * 8;

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

    public Square(int col, int row)
    {
        (Col, Row) = (col, row);
    }

    public Square(int index)
    {
        Col = (index + 1) % 8 != 0 ? (index + 1) % 8 : 8;
        Row = (int)Math.Ceiling((index + 1) / 8f);
    }

    public Square(String squareName)
    {
        Col = (int)squareName[0] - 96;
        Row = (int)Char.GetNumericValue(squareName[1]);
    }

    public Square(Vector3 screenPosition)
    {
        // Round the values to the nearest integer
        Col = (int)(screenPosition.x + 0.5);
        Row = (int)(screenPosition.y + 0.5);
    }

    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// Check if a square is valid before attempting to create it.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static bool IsValidSquare(int col, int row)
    {
        return col >= 1 && col <= 8 && row >= 1 && row <= 8;
    }

    public bool IsHighlighted()
    {
        foreach (GameObject tile in BoardHelper.GetTiles())
        {
            if (new Square(tile.transform.position) == this)
            {
                return true;
            }
        }

        return false;
    }
}