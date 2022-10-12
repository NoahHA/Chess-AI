using System;
using UnityEngine;

/// <summary>
/// Handles behaviour for a tile on the chess board
/// </summary>
public class ChessSquare
{
    // FIELDS
    private int _row;

    private int _col;

    // CONSTRUCTORS
    public ChessSquare()
    { }

    public ChessSquare(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public ChessSquare(Vector3 location)
    {
        Location = location;
        ChessSquare position = GetPositionFromLocation(location);
        Row = position.Row;
        Col = position.Col;
    }

    // PROPERTIES
    public int Row
    {
        get { return _row; }
        set
        {
            if (value >= 0 && value <= 7)
                _row = value;
            else
            {
                _row = -1;
                _col = -1;
            }
        }
    }

    public int Col
    {
        get { return _col; }
        set
        {
            if (value >= 0 && value <= 7)
                _col = value;
            else
            {
                _row = -1;
                _col = -1;
            }
        }
    }

    public Vector3 Location
    {
        get { return GetLocationFromPosition(this); }
        private set { }
    }

    // METHODS

    /// <summary>
    /// Converts a tile position on the board to a coordinate in world space
    /// </summary>
    /// <param name="square"> The chess square to convert </param>
    /// <returns> </returns>
    private Vector3 GetLocationFromPosition(ChessSquare square)
    {
        float startingX = -3.5f;
        float startingY = 3.5f;

        float x = startingX + square.Col;
        float y = startingY - square.Row;

        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Converts a coordinate in world space to a tile position on the board
    /// </summary>
    /// <param name="position"> The coordinate to convert </param>
    /// <returns> </returns>
    private ChessSquare GetPositionFromLocation(Vector3 position)
    {
        var square = new ChessSquare();
        int offsetX = -4;
        int offsetY = 3;

        square.Col = (int)Math.Floor(position.x) - offsetX;
        square.Row = offsetY - (int)Math.Floor(position.y);

        return square;
    }
}