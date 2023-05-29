using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// The type of chess piece being represented.
/// </summary>
public enum PieceType
{
    None, // Default value
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
    None, // Default value
    White,
    Black
}

/// <summary>
/// Represents a Chess piece.
/// </summary>
public struct Piece
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

    public Piece(PieceType type = PieceType.None, PieceColour colour = PieceColour.None)
    {
        (Type, Colour, Letter) = (type, colour, '1');

        if (type != PieceType.None)
        {
            Letter = LetterDict[type];

            if (Colour == PieceColour.Black)
            {
                Letter = Char.ToUpper(Letter);
            }
        }
    }

    /// <summary>
    /// Generate a Piece object based on a FEN string letter.
    /// </summary>
    /// <param name="letter">A letter from a FEN string.</param>
    /// <exception cref="ArgumentException"></exception>
    public Piece(char letter)
    {
        (Type, Colour) = (PieceType.Pawn, PieceColour.None);
        Letter = letter;

        if (!PieceDict.ContainsKey(Char.ToLower(letter)))
            throw new ArgumentException($"Letter not recognized: {letter}", nameof(letter));

        (Type, Colour) = (PieceDict[Char.ToLower(letter)], PieceColour.White);

        if (Char.IsUpper(letter))
        {
            Colour = PieceColour.Black;
        }
    }

    public override string ToString()
    {
        if (Type == PieceType.None || Colour == PieceColour.None)
        {
            return "Empty Square";
        }

        return Colour.ToString() + " " + Type.ToString();
    }
}