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

    public override string ToString()
    {
        return StartSquare.ToString() + EndSquare.ToString();
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }

        return this.ToString() == obj.ToString();
    }

    public static bool operator ==(Move obj1, Move obj2)
    {
        return obj1.ToString() == obj2.ToString();
    }

    public static bool operator !=(Move obj1, Move obj2)
    {
        return obj1.ToString() != obj2.ToString();
    }

    public override int GetHashCode()
    {
        return StartSquare.GetHashCode() ^ EndSquare.GetHashCode();
    }
}