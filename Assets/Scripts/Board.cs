﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles behaviour for the chess board
/// </summary>
public static class Board
{
    [Tooltip("The FEN string for the starting position of a standard chess game.")]
    public const string startingPosition = "rnbqkbnr/pppppppp/8/8/8/8/8/RNBQKBNR"; // public const string startingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    /// <summary>
    /// Checks whether a clicked piece is the correct colour for the player who clicked it.
    /// </summary>
    /// <param name="piece"> The chess piece that was clicked. </param>
    /// <returns> </returns>
    public static bool ValidPieceClicked(GameObject piece)
    {
        bool validWhiteClick = piece.name.Contains("w_") & GameController.playerTurn;
        bool validBlackClick = piece.name.Contains("b_") & !GameController.playerTurn;
        return validWhiteClick | validBlackClick;
    }

    /// <summary>
    /// Checks whether a given piece is the correct colour for the current turn.
    /// </summary>
    /// <param name="piecename"> The name of the piece to check. </param>
    /// <returns> </returns>
    public static bool ValidPieceClicked(string piecename)
    {
        bool validWhiteClick = piecename.Contains("w_") & GameController.playerTurn;
        bool validBlackClick = piecename.Contains("b_") & !GameController.playerTurn;
        return validWhiteClick | validBlackClick;
    }

    /// <summary>
    /// Converts a chess FEN string to an actual board state
    /// </summary>
    /// <param name="position"> FEN string </param>
    public static void GeneratePosition(string position)
    {
        ClearBoard();
        ChessSquare currentSquare = new ChessSquare(0, 0);

        // loops through the string
        foreach (char letter in position)
        {
            // Digits represent skipped spaces
            if (char.IsDigit(letter))
                if (currentSquare.Col == 0)
                    currentSquare.Col += (int)char.GetNumericValue(letter) - 1;
                else
                    currentSquare.Col += (int)char.GetNumericValue(letter);

            // slashes represent ends of rows
            else if (letter.Equals('/'))
            {
                currentSquare.Row++;
                currentSquare.Col = 0;
            }

            // Anything else is a piece to instantiate
            else
            {
                GameObject piece = GetPieceFromLetter(letter);
                var createdPiece = GameObject.Instantiate(piece, currentSquare.Location, Quaternion.Euler(0, 0, 0));

                if (currentSquare.Col < 7) { currentSquare.Col++; }
            }
        };
    }

    /// <summary>
    /// Destroys all pieces on the board to reset the game
    /// </summary>
    public static void ClearBoard()
    {
        var pieces = GameObject.FindGameObjectsWithTag("Piece");

        foreach (var piece in pieces)
            GameObject.Destroy(piece);
    }

    /// <summary>
    /// Converts a letter in a FEN string to the appropriate chess piece object
    /// </summary>
    /// <param name="letter">
    /// Letter from a FEN string representing a chess piece, uppercase=white, lowercase=black
    /// </param>
    /// <returns> </returns>
    /// <exception cref="ArgumentException"> </exception>
    public static GameObject GetPieceFromLetter(char letter)
    {
        // Dictionary connecting piece names to their index in the pieces array
        var pieceDict = new Dictionary<char, int>
        {
            { 'p', 0 },
            { 'r', 1 },
            { 'n', 2 },
            { 'b', 3 },
            { 'q', 4 },
            { 'k', 5 },
            { 'P', 6 },
            { 'R', 7 },
            { 'N', 8 },
            { 'B', 9 },
            { 'Q', 10 },
            { 'K', 11 }
        };

        if (!pieceDict.ContainsKey(letter))
            throw new ArgumentException($"Letter not recognized: {letter}", nameof(letter));

        return PieceManager.Instance.pieces[pieceDict[letter]];
    }

    /// <summary>
    /// Checks if a piece exists on a given square
    /// </summary>
    /// <param name="square"> The chess square being checked </param>
    /// <returns> The piece located on that square, or null if none found </returns>
    public static Collider2D FindPieceOnSquare(ChessSquare square)
    {
        float radius = 0.1f;
        return Physics2D.OverlapCircle(square.Location, radius, LayerMask.GetMask("Default"));
    }

    /// <summary>
    /// Checks if two pieces are on opposite colours
    /// </summary>
    /// <returns> </returns>
    public static bool IsEnemyPiece(GameObject mainPiece, GameObject otherPiece)
    {
        bool isEnemyForBlack = mainPiece.name.Contains("b_") & otherPiece.name.Contains("w_");
        bool isEnemyForWhite = mainPiece.name.Contains("w_") & otherPiece.name.Contains("b_");

        return isEnemyForBlack | isEnemyForWhite;
    }

    /// <summary>
    /// Checks if two pieces are on opposite colours
    /// </summary>
    /// <returns> </returns>
    public static bool IsEnemyPiece(string mainPieceName, GameObject otherPiece)
    {
        bool isEnemyForBlack = mainPieceName.Contains("b_") & otherPiece.name.Contains("w_");
        bool isEnemyForWhite = mainPieceName.Contains("w_") & otherPiece.name.Contains("b_");

        return isEnemyForBlack | isEnemyForWhite;
    }

    /// <summary>
    /// Checks if two pieces are on opposite colours
    /// </summary>
    /// <returns> </returns>
    public static bool IsEnemyPiece(string mainPieceName, string otherPieceName)
    {
        bool isEnemyForBlack = mainPieceName.Contains("b_") & otherPieceName.Contains("w_");
        bool isEnemyForWhite = mainPieceName.Contains("w_") & otherPieceName.Contains("b_");

        return isEnemyForBlack | isEnemyForWhite;
    }

    /// <summary>
    /// Checks if two pieces are on opposite colours
    /// </summary>
    /// <returns> </returns>
    public static bool IsEnemyPiece(GameObject mainPiece, string otherPieceName)
    {
        bool isEnemyForBlack = mainPiece.name.Contains("b_") & otherPieceName.Contains("w_");
        bool isEnemyForWhite = mainPiece.name.Contains("w_") & otherPieceName.Contains("b_");

        return isEnemyForBlack | isEnemyForWhite;
    }

    /// <summary>
    /// Checks if two pieces are on opposite colours
    /// </summary>
    /// <returns> </returns>
    public static bool IsEnemyPiece(bool playerTurn, string otherPieceName)
    {
        bool isEnemyForBlack = !playerTurn & otherPieceName.Contains("w_");
        bool isEnemyForWhite = playerTurn & otherPieceName.Contains("b_");

        return isEnemyForBlack | isEnemyForWhite;
    }

    /// <summary>
    /// Checks if two pieces are on opposite colours
    /// </summary>
    /// <returns> </returns>
    public static bool IsEnemyPiece(bool playerTurn, GameObject otherPiece)
    {
        bool isEnemyForBlack = !playerTurn & otherPiece.name.Contains("w_");
        bool isEnemyForWhite = playerTurn & otherPiece.name.Contains("b_");

        return isEnemyForBlack | isEnemyForWhite;
    }

    /// <summary>
    /// Flips the board and all the pieces after each turn.
    /// </summary>
    public static void FlipBoard()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        camera.transform.Rotate(0, 0, 180);
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");

        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].transform.Rotate(0, 0, 180);
        }
    }

    /// <summary>
    /// Checks if an enemy piece is on the same square, and if it is then takes it.
    /// </summary>
    /// <param name="playerPiece"> </param>
    /// <returns> </returns>
    public static GameObject TakePiece(GameObject playerPiece)
    {
        // temporarily ignore piece so it doesn't collide with itself
        playerPiece.layer = LayerMask.NameToLayer("Ignore");

        Collider2D pieceToTake = FindPieceOnSquare(new ChessSquare(playerPiece.transform.position));

        if (pieceToTake != null)
        {
            pieceToTake.gameObject.SetActive(false);
            playerPiece.layer = LayerMask.NameToLayer("Default");
            return pieceToTake.gameObject;
        }

        playerPiece.layer = LayerMask.NameToLayer("Default");
        return null;
    }

    public static void Castle(ChessSquare kingSquare, ChessSquare startingSquare, bool turn)
    {
        // If player castled queenside
        var newCastlePosition = new ChessSquare(kingSquare.Row, kingSquare.Col + 1);
        var castlePosition = new ChessSquare(startingSquare.Row, startingSquare.Col - 4);

        // If player castled kingside
        if (kingSquare.Col > startingSquare.Col)
        {
            newCastlePosition = new ChessSquare(kingSquare.Row, kingSquare.Col - 1);
            castlePosition = new ChessSquare(startingSquare.Row, startingSquare.Col + 3);
        }

        GameObject castlePiece = FindPieceOnSquare(castlePosition).gameObject;
        castlePiece.transform.position = newCastlePosition.Location;
    }

    public static void QueenPawn(GameObject pawn, bool turn)
    {
        // Letter defines whether queen is white or black
        char letter = 'q';
        if (turn) letter = 'Q';

        // Create a queen piece
        GameObject queenPiece = GetPieceFromLetter(letter);
        GameObject.Instantiate(queenPiece, pawn.transform.position, pawn.transform.rotation);

        // Deactivate the pawn
        pawn.SetActive(false);
    }
}