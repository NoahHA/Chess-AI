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

/// <summary>
/// Represents a Chess piece.
/// </summary>
public record Piece
{
    public PieceType Type;
    public PieceColour Colour;
    public char Letter;

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
        (Type, Colour, Letter) = (type, colour, '1');

        Letter = (Colour == PieceColour.Black) ? Char.ToUpper(LetterDict[type]) : LetterDict[type];
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
        (Type, Colour, Letter) = (PieceType.Pawn, PieceColour.White, letter);

        if (!PieceDict.ContainsKey(Char.ToLower(letter)))
            throw new ArgumentException($"Letter not recognized: {letter}", nameof(letter));

        Type = PieceDict[Char.ToLower(letter)];
        Colour = Char.IsUpper(letter) ? PieceColour.Black : PieceColour.White;
    }

    public override string ToString()
    {
        return Colour.ToString() + " " + Type.ToString();
    }
}