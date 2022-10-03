using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Handles behaviour for the chess board
/// </summary>
public static class Board
{
    public const string startingPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    /// <summary>
    /// Checks whether a clicked piece is the correct colour for the player who clicked it
    /// </summary>
    /// <param name="piece">The chess piece that was clicked</param>
    /// <returns></returns>
    public static bool ValidPieceClicked(GameObject piece)
    {
        bool validWhiteClick = piece.name.Contains("w_") & GameController.playerTurn;
        bool validBlackClick = piece.name.Contains("b_") & !GameController.playerTurn;
        return validWhiteClick | validBlackClick;
    }

    /// <summary>
    /// Checks whether a given piece is the correct colour for the current turn
    /// </summary>
    /// <param name="piecename">The name of the piece to check</param>
    /// <returns></returns>
    public static bool ValidPieceClicked(string piecename)
    {
        bool validWhiteClick = piecename.Contains("w_") & GameController.playerTurn;
        bool validBlackClick = piecename.Contains("b_") & !GameController.playerTurn;
        return validWhiteClick | validBlackClick;
    }

    /// <summary>
    /// Converts a chess FEN string to an actual board state
    /// </summary>
    /// <param name="position">FEN string</param>
    public static void GeneratePosition(string position)
    {
        ClearBoard();
        ChessSquare currentSquare = new ChessSquare(0, 0);

        // loops through the string
        foreach (char letter in position)
        {
            // Digits represent skipped spaces
            if (char.IsDigit(letter))
                currentSquare.Col += (int)char.GetNumericValue(letter) - 1;
            
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
                var createdPiece = GameObject.Instantiate(piece, currentSquare.Location, piece.transform.rotation);
                createdPiece.tag = "Piece";
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
    /// <param name="letter">Letter from a FEN string representing a chess piece, uppercase=white, lowercase=black</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
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
}