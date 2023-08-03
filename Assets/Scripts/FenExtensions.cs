using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Extension methods for FEN strings.
/// </summary>
public static class FenExtensions
{
    /// <summary>
    /// Checks whether castling is still available according to the FEN string.
    /// </summary>
    /// <returns>Whether the specified type of castling is available.</returns>
    public static bool CanCastle(this string fen, Castling castleType, PieceColour turn)
    {
        string castleFen = fen.Split(' ')[2];
        if (castleType == Castling.KingSide)
        {
            return (turn == PieceColour.White && castleFen.Contains('K')) ||
                (turn == PieceColour.Black && castleFen.Contains('k'));
        }
        else
        {
            return (turn == PieceColour.White && castleFen.Contains('Q')) ||
                (turn == PieceColour.Black && castleFen.Contains('q'));
        }
    }

    public static string UpdateTurn(this string fen, PieceColour turn)
    {
        char[] charArrFen = fen.ToCharArray();
        charArrFen[fen.IndexOf(' ') + 1] = (turn == PieceColour.White) ? 'w' : 'b';
        return new string(charArrFen);
    }

    public static PieceColour GetTurnFromFen(this string fen)
    {
        return (fen[fen.IndexOf(' ') + 1] == 'w') ? PieceColour.White : PieceColour.Black;
    }

    /// <summary>
    /// Converts a chess FEN string to a board state.
    /// </summary>
    /// <param name="fen">FEN string</param>
    public static Piece[] GetStateFromFen(this string fen)
    {
        int counter = 0;
        Piece[] state = new Piece[64];
        int spaceIdx = fen.IndexOf(' ');

        foreach (char c in fen[0..spaceIdx])
        {
            if (char.IsDigit(c))
            {
                counter += (int)char.GetNumericValue(c);
            }
            else if (char.IsLetter(c))
            {
                state[counter] = new Piece(c);
                counter++;
            }
        }

        if (counter != 64)
        {
            throw new ArgumentException($"FEN string is incorrect length: should be 64 but was {counter}", nameof(fen));
        }

        return state;
    }
}