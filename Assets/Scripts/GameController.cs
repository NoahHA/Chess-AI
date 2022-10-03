using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles overall game behaviour
/// </summary>
public class GameController : MonoBehaviour
{
    [Tooltip("Who's turn it is, false=black, true=white")]
    public static bool playerTurn = true;
    [Tooltip("Radius ")]
    private static float radius = 0.1f;

    /// <summary>
    /// Returns the legal moves for a given piece and board location
    /// </summary>
    /// <param name="position">Chess tile that the selected piece is on</param>
    /// <param name="piece">The selected piece</param>
    /// <returns></returns>
    public static List<ChessSquare> GetLegalMoves(ChessSquare position, GameObject piece)
    {
        List<ChessSquare> moves = new();
        string pieceName = piece.name;

        if (Board.ValidPieceClicked(pieceName))
        {
            // temporarily ignore piece so it doesn't collide with itself
            piece.layer = LayerMask.NameToLayer("Ignore");

            // Checks for the type of piece and calls the relevant function
            if (pieceName.Contains("pawn")) moves.AddRange(GetPawnMoves(position, piece));

            else if (pieceName.Contains("rook")) moves.AddRange(GetRookMoves(position));

            else if (pieceName.Contains("bishop")) moves.AddRange(GetBishopMoves(position));

            else if (pieceName.Contains("queen")) moves.AddRange(GetQueenMoves(position));

            else if (pieceName.Contains("king")) moves.AddRange(GetKingMoves(position));

            else if (pieceName.Contains("knight")) moves.AddRange(GetKnightMoves(position));

            piece.layer = LayerMask.NameToLayer("Default");
        }

        return moves;
    }

    // TODO: Overload GetLegalMoves so it can also take in a board position string and a piece name

    private static List<ChessSquare> GetPawnMoves(ChessSquare position, GameObject piece)
    {
        List<ChessSquare> pawnMoves = new();
        bool isBlocked = false;
        // white pieces have +1 direction, black have -1
        int direction = 1;
        if (piece.name.Contains("b_")) { direction = -1; }

        for (int i = 1; i < 3; i++)
        {
            var square = new ChessSquare(position.Row - i * direction, position.Col);

            if ((Physics2D.OverlapCircle(square.Location, radius, LayerMask.GetMask("Default")) == null) && (!isBlocked))
                pawnMoves.Add(square);

            else { isBlocked = true; }
        }
        return pawnMoves;
    }

    private static List<ChessSquare> GetRookMoves(ChessSquare position)
    {
        List<ChessSquare> rookMoves = new();
        bool isBlocked = false;

        for (int row_dir = -1; row_dir < 2; row_dir += 1)
        {
            for (int col_dir = -1; col_dir < 2; col_dir += 1)
            {
                for (int i = 1; i < 8; i++)
                {
                    if (row_dir != 0 && col_dir != 0) { continue; }
                    
                    var square = new ChessSquare(position.Row + i * row_dir, position.Col + i * col_dir);

                    if ((Physics2D.OverlapCircle(square.Location, radius, LayerMask.GetMask("Default")) == null) && (!isBlocked))
                        rookMoves.Add(square);

                    else { isBlocked = true; }
                }
                isBlocked = false;
            }
        }
        return rookMoves;
    }
    
    private static List<ChessSquare> GetBishopMoves(ChessSquare position)
    {
        List<ChessSquare> bishopMoves = new();
        bool isBlocked = false;

        for (int row_dir = -1; row_dir < 2; row_dir += 2)
        {
            for (int col_dir = -1; col_dir < 2; col_dir += 2)
            {
                for (int i = 1; i < 8; i++)
                {                    
                    var square = new ChessSquare(position.Row + i * row_dir, position.Col + i * col_dir);

                    if ((Physics2D.OverlapCircle(square.Location, radius, LayerMask.GetMask("Default")) == null) && (!isBlocked))
                        bishopMoves.Add(square);

                    else { isBlocked = true; }
                }
                isBlocked = false;
            }
        }
        return bishopMoves;
    }

    private static List<ChessSquare> GetQueenMoves(ChessSquare position)
    {
        List<ChessSquare> queenMoves = new();
        queenMoves.AddRange(GetBishopMoves(position));
        queenMoves.AddRange(GetRookMoves(position));
        
        return queenMoves;
    }

    private static List<ChessSquare> GetKingMoves(ChessSquare position)
    {
        List<ChessSquare> kingMoves = new();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                ChessSquare square = new ChessSquare(position.Row + i, position.Col + j);

                if (Physics2D.OverlapCircle(square.Location, radius, LayerMask.GetMask("Default")) == null)
                    kingMoves.Add(square);
            }
        }
        return kingMoves;
    }
    
    private static List<ChessSquare> GetKnightMoves(ChessSquare position)
    {
        List<ChessSquare> knightMoves = new();

        for (int i = -2; i < 5; i += 4)
        {
            for (int j = -1; j < 2; j += 2)
            {
                ChessSquare s1 = new ChessSquare(position.Row + i, position.Col + j);
                ChessSquare s2 = new ChessSquare(position.Row + j, position.Col + i);

                if (Physics2D.OverlapCircle(s1.Location, radius, LayerMask.GetMask("Default")) == null)
                    knightMoves.Add(s1);
                if (Physics2D.OverlapCircle(s2.Location, radius, LayerMask.GetMask("Default")) == null)
                    knightMoves.Add(s2);
            }
        }
        return knightMoves;
    }
}
