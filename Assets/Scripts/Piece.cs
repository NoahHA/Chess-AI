using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The type of chess piece being represented.
/// </summary>
public enum PieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

/// <summary>
/// Which side a piece or player is on.
/// </summary>
public enum PieceColour
{
    White,
    Black,
    None
}

/// <summary>
/// Represents a Chess piece.
/// </summary>
public struct Piece
{
    public PieceType Type;
    public PieceColour Colour;

    public Piece(PieceType type, PieceColour colour)
    {
        Type = type;
        Colour = colour;
    }

    /// <summary>
    /// Generate a Piece object based on a FEN string letter.
    /// </summary>
    /// <param name="letter">A letter from a FEN string.</param>
    /// <exception cref="ArgumentException"></exception>
    public Piece(char letter)
    {
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

        (Type, Colour) = (pieceDict[Char.ToLower(letter)], PieceColour.White);

        if (Char.IsUpper(letter))
        {
            Colour = PieceColour.Black;
        }
    }

    public override string ToString()
    {
        return Colour.ToString() + " " + Type.ToString();
    }
}