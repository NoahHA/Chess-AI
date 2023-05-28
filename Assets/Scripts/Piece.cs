using System;
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

    public static Piece GetPieceFromLetter(char letter)
    {
        Piece piece = new Piece();

        // Dictionary connecting piece names to their type
        var pieceDict = new Dictionary<char, PieceType>
        {
            { 'p', PieceType.Pawn },
            { 'r', PieceType.Rook },
            { 'n', PieceType.Knight },
            { 'b', PieceType.Bishop },
            { 'q', PieceType.Queen },
            { 'k', PieceType.King },
        };

        if (!pieceDict.ContainsKey(Char.ToLower(letter)))
            throw new ArgumentException($"Letter not recognized: {letter}", nameof(letter));

        piece.Type = pieceDict[Char.ToLower(letter)];

        if (Char.IsUpper(letter))
        {
            piece.Colour = Colour.Black;
        }
        else
        {
            piece.Colour = Colour.White;
        }

        return piece;
    }
}