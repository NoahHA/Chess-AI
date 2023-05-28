using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public enum PieceColour
{
    White,
    Black
}

public class Piece
{
    public PieceType Type;
    public PieceColour Colour;

    public Piece(PieceType type, PieceColour colour)
    {
        Type = type;
        Colour = colour;
    }
}