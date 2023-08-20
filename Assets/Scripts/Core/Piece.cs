using System;
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
    Black
}

public static class PieceExtensions
{
    public static PieceColour ChangeTurn(this PieceColour turn)
    {
        return (turn == PieceColour.White) ? PieceColour.Black : PieceColour.White;
    }
}

/// <summary>
/// Represents a Chess piece.
/// </summary>
public record Piece
{
    private char _letter;

    public PieceType Type;
    public PieceColour Colour;
    public char Letter
    {
        get => _letter;
        set { _letter = value; UpdatePieceFromLetter(value); }
    }

    private void UpdatePieceFromLetter(char letter)
    {
        Type = PieceDict[Char.ToLower(letter)];
        Colour = Char.IsUpper(letter) ? PieceColour.White : PieceColour.Black;
    }

    // Dictionary connecting piece names to their type
    private Dictionary<char, PieceType> PieceDict => new()
        {
            { 'p', PieceType.Pawn },
            { 'r', PieceType.Rook },
            { 'n', PieceType.Knight },
            { 'b', PieceType.Bishop },
            { 'q', PieceType.Queen },
            { 'k', PieceType.King },
        };

    private Dictionary<PieceType, Char> LetterDict => new()
        {
            { PieceType.Pawn, 'p' },
            { PieceType.Rook, 'r' },
            { PieceType.Knight, 'n' },
            { PieceType.Bishop, 'b' },
            { PieceType.Queen, 'q' },
            { PieceType.King, 'k' },
        };

    public Piece(PieceType type, PieceColour colour)
    {
        (Type, Colour) = (type, colour);
        Letter = (Colour == PieceColour.White) ? Char.ToUpper(LetterDict[type]) : LetterDict[type];
    }

    public string GetPrefabName()
    {
        char colourLetter = Char.ToLower(Colour.ToString()[0]);
        return colourLetter + "_" + Type.ToString().ToLower();
    }

    /// <summary>
    /// Generate a Piece object based on a FEN string letter.
    /// </summary>
    /// <param name="letter">A letter from a FEN string.</param>
    /// <exception cref="ArgumentException"></exception>
    public Piece(char letter)
    {
        Letter = letter;
    }

    public override string ToString()
    {
        return Colour.ToString() + " " + Type.ToString();
    }
}