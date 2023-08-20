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
        string castleFen = fen.GetFenCastleSection();

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

    public static Square GetEnPassantSquareFromFen(this string fen)
    {
        string enPassantString = fen.GetFenEnPassantSection();
        return (enPassantString == "-") ? null : new Square(enPassantString);
    }

    public static bool[] GetCanCastleFromFen(this string fen)
    {
        bool[] canCastle = new bool[4] { false, false, false, false };

        if (fen.CanCastle(Castling.KingSide, PieceColour.White))
        {
            canCastle[0] = true;
        }
        if (fen.CanCastle(Castling.QueenSide, PieceColour.White))
        {
            canCastle[1] = true;
        }
        if (fen.CanCastle(Castling.KingSide, PieceColour.Black))
        {
            canCastle[2] = true;
        }
        if (fen.CanCastle(Castling.QueenSide, PieceColour.Black))
        {
            canCastle[3] = true;
        }

        return canCastle;
    }

    /// <summary>
    /// Gets just the section of the FEN representing piece positions.
    /// </summary>
    public static string GetFenPieceSection(this string fen) => fen.Split(' ')[0];

    /// <summary>
    /// Gets just the section of the FEN representing the current turn.
    /// </summary>
    public static string GetFenTurnsection(this string fen) => fen.Split(' ')[1];

    /// <summary>
    /// Gets just the section of the FEN representing castle options.
    /// </summary>
    public static string GetFenCastleSection(this string fen) => fen.Split(' ')[2];

    /// <summary>
    /// Gets just the section of the FEN representing en passant square.
    /// </summary>
    public static string GetFenEnPassantSection(this string fen) => fen.Split(' ')[3];
}