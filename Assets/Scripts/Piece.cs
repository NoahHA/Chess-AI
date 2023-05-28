using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    None,
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public enum Colour
{
    White,
    Black,
    None
}

public struct Piece
{
    public PieceType Type;
    public Colour Colour;

    public Piece(PieceType type = PieceType.None, Colour colour = Colour.None)
    {
        Type = type;
        Colour = colour;
    }
}