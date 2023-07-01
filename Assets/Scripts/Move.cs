using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a chess move.
/// </summary>
public class Move
{
    public Square StartSquare;
    public Square EndSquare;

    public Move(Square startSquare, Square endSquare)
    {
        (StartSquare, EndSquare) = (startSquare, endSquare);
    }

    public Move(string startSquare, string endSquare)
    {
        (StartSquare, EndSquare) = (new Square(startSquare), new Square(endSquare));
    }
}