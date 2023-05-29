using System;
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
    Black,
    None
}

public struct Piece
{
    public PieceType Type;
    public PieceColour Colour;

    public Piece(PieceType type, PieceColour colour)
    {
        Type = type;
        Colour = colour;
    }

    public override string ToString()
    {
        return Colour.ToString() + " " + Type.ToString();
    }

    /// <summary>
    /// Converts a letter in a FEN string to a Piece object.
    /// </summary>
    /// <param name="letter">The FEN string letter describing the piece.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Invalid letter.</exception>
    public static Piece GetPieceFromLetter(char letter)
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

        Piece piece = new Piece(pieceDict[Char.ToLower(letter)], PieceColour.White);

        if (Char.IsUpper(letter))
        {
            piece.Colour = PieceColour.Black;
        }

        return piece;
    }
}