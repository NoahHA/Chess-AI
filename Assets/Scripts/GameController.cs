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
            else if (pieceName.Contains("rook")) moves.AddRange(GetRookMoves(position, piece));
            else if (pieceName.Contains("bishop")) moves.AddRange(GetBishopMoves(position, piece));
            else if (pieceName.Contains("queen")) moves.AddRange(GetQueenMoves(position, piece));
            else if (pieceName.Contains("king")) moves.AddRange(GetKingMoves(position, piece));
            else if (pieceName.Contains("knight")) moves.AddRange(GetKnightMoves(position, piece));

            piece.layer = LayerMask.NameToLayer("Default");
        }

        return moves;
    }

    // TODO: Overload GetLegalMoves so it can also take in a board position string and a piece name

    private static List<ChessSquare> GetPawnMoves(ChessSquare position, GameObject piece)
    {
        List<ChessSquare> pawnMoves = new();
        // Flag to indicate the something in the way of the pawn
        bool isBlocked = false;
        // White pieces have +1 direction, black have -1
        int direction = 1;
        if (piece.name.Contains("b_")) { direction = -1; }
        // Max number of forward moves the pawn can take
        int max_moves = 1;

        if ((position.Row == 6 && direction == 1) || (position.Row == 1 && direction == -1))
            max_moves++;

        // Checks for forward moves
        for (int i = 1; i < max_moves + 1; i++)
        {
            var square = new ChessSquare(position.Row - i * direction, position.Col);

            // Add square in front of pawn to moves if not blocked
            if (Board.FindPieceOnSquare(square) == null && !isBlocked)
                pawnMoves.Add(square);

            else { isBlocked = true; }
        }

        // Checks for diagonal moves
        for (int j = -1; j < 2; j += 2)
        {
            var square = new ChessSquare(position.Row - 1 * direction, position.Col + j);

            // Add diagonal square to moves if opponent's piece can be taken there
            Collider2D diagonalPiece = Board.FindPieceOnSquare(square);
            if (diagonalPiece != null)
            {
                if (Board.IsEnemyPiece(piece, diagonalPiece.gameObject))
                    pawnMoves.Add(square);
            }
        }

        return pawnMoves;
    }

    private static List<ChessSquare> GetRookMoves(ChessSquare position, GameObject piece)
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
                    
                    var square = new ChessSquare(position.Row + (i * row_dir), position.Col + (i * col_dir));

                    if (!isBlocked)
                    {
                        // If square is empty
                        if (Board.FindPieceOnSquare(square) == null)
                            rookMoves.Add(square);

                        // If square is occupied
                        else
                        {
                            // If square is occupied by an enemy piece
                            if (Board.IsEnemyPiece(piece, Board.FindPieceOnSquare(square).gameObject))
                                rookMoves.Add(square);

                            isBlocked = true;
                        }
                    }
                }
                isBlocked = false;
            }
        }
        return rookMoves;
    }
    
    private static List<ChessSquare> GetBishopMoves(ChessSquare position, GameObject piece)
    {
        List<ChessSquare> bishopMoves = new();
        bool isBlocked = false;

        for (int row_dir = -1; row_dir < 2; row_dir += 2)
        {
            for (int col_dir = -1; col_dir < 2; col_dir += 2)
            {
                for (int i = 1; i < 8; i++)
                {
                    var square = new ChessSquare(position.Row + (i * row_dir), position.Col + (i * col_dir));

                    if (!isBlocked)
                    {
                        // If square is empty
                        if (Board.FindPieceOnSquare(square) == null)
                            bishopMoves.Add(square);

                        // If square is occupied by piece
                        else
                        {
                            // If square is occupied by an enemy piece
                            if (Board.IsEnemyPiece(piece, Board.FindPieceOnSquare(square).gameObject))
                                bishopMoves.Add(square);
                        
                            isBlocked = true;
                        }
                    }
                }
                isBlocked = false;
            }
        }
        return bishopMoves;
    }

    private static List<ChessSquare> GetQueenMoves(ChessSquare position, GameObject piece)
    {
        List<ChessSquare> queenMoves = new();
        queenMoves.AddRange(GetBishopMoves(position, piece));
        queenMoves.AddRange(GetRookMoves(position, piece));
        
        return queenMoves;
    }

    private static List<ChessSquare> GetKingMoves(ChessSquare position, GameObject piece)
    {
        List<ChessSquare> kingMoves = new();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                var square = new ChessSquare(position.Row + i, position.Col + j);

                // If square is empty
                if (Board.FindPieceOnSquare(square) == null)
                    kingMoves.Add(square);

                // If square is occupied by an enemy piece
                else if (Board.IsEnemyPiece(piece, Board.FindPieceOnSquare(square).gameObject))
                    kingMoves.Add(square);
            }
        }
        return kingMoves;
    }
    
    private static List<ChessSquare> GetKnightMoves(ChessSquare position, GameObject piece)
    {
        List<ChessSquare> knightMoves = new();

        for (int i = -2; i < 5; i += 4)
        {
            for (int j = -1; j < 2; j += 2)
            {
                var s1 = new ChessSquare(position.Row + i, position.Col + j);
                var s2 = new ChessSquare(position.Row + j, position.Col + i);

                // If square is empty
                if (Board.FindPieceOnSquare(s1) == null)
                    knightMoves.Add(s1);

                // If square is occupied by an enemy piece
                else if (Board.IsEnemyPiece(piece, Board.FindPieceOnSquare(s1).gameObject))
                    knightMoves.Add(s1);

                // If square is empty
                if (Board.FindPieceOnSquare(s2) == null)
                    knightMoves.Add(s2);

                // If square is occupied by an enemy piece
                else if (Board.IsEnemyPiece(piece, Board.FindPieceOnSquare(s2).gameObject))
                    knightMoves.Add(s2);
            }
        }
        return knightMoves;
    }
}
