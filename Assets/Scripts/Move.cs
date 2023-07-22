using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the types of castling.
/// </summary>
public enum Castling
{
    KingSide,
    QueenSide
}

/// <summary>
/// Represents a chess move.
/// </summary>
public record Move
{
    public Square StartSquare;
    public Square EndSquare;

    /// <summary>
    /// Whether the move is to castle.
    /// </summary>
    public bool Castling = false;

    public Move(Square startSquare, Square endSquare, bool castling = false)
    {
        (StartSquare, EndSquare, Castling) = (startSquare, endSquare, castling);
    }

    public Move(string startSquare, string endSquare, bool castling = false)
    {
        (StartSquare, EndSquare, Castling) = (new Square(startSquare), new Square(endSquare), castling);
    }

    public override string ToString()
    {
        return StartSquare.ToString() + EndSquare.ToString();
    }
}