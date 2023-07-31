using MacFsWatcher;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player inputs.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    [Tooltip("Highlight for currently selected piece")]
    public GameObject tileHighlight;

    [Tooltip("Highlight for possible moves for currently selected piece")]
    public GameObject circleHighlight;

    [Tooltip("Highlight for enemy pieces that can be taken")]
    public GameObject takeablePieceHighlight;

    // The legal moves on click
    private List<Move> legalMoves = new();

    // The initial position of the clicked piece
    private Square startSquare;

    public delegate void OnPieceClicked(GameObject piece);

    public static OnPieceClicked onPieceClicked;

    public delegate void OnPieceDragged(GameObject piece);

    public static OnPieceClicked onPieceMoved;

    public delegate void OnPiecePlaced(GameObject piece);

    public static OnPieceClicked onPiecePlaced;

    public delegate void OnMoveMade(Move move);

    public static OnMoveMade onMoveMade;

    /// <summary>
    /// Highlights the square the player has clicked on.
    /// </summary>
    /// <param name="clickedSquare"> The chess tile the player has clicked on </param>
    public void HighLightSquare(Square clickedSquare)
    {
        Instantiate(tileHighlight, clickedSquare.ScreenPosition, tileHighlight.transform.rotation);
    }

    /// <summary>
    /// Highlights all legal moves for the selected piece.
    /// </summary>
    public void HightLightLegalMoves(List<Move> legalMoves)
    {
        // Loops through all legal moves and highlights them
        foreach (Move move in legalMoves)
        {
            Piece pieceOnSquare = GameController.Instance.MainBoard.FindPieceOnSquare(move.EndSquare);

            if (pieceOnSquare == null)
            {
                Instantiate(circleHighlight, move.EndSquare.ScreenPosition, circleHighlight.transform.rotation);
            }

            // Highlight differently if there's a takeable enemy piece
            else if (GameController.Instance.MainBoard.IsEnemyPiece(pieceOnSquare))
            {
                Instantiate(takeablePieceHighlight, move.EndSquare.ScreenPosition, circleHighlight.transform.rotation);
            }
        }
    }

    public void OnEnable()
    {
        onPieceClicked += HandlePieceClick;
        onPieceMoved += HandlePieceMoved;
        onPiecePlaced += HandlePiecePlaced;
    }

    public void OnDisable()
    {
        onPieceClicked -= HandlePieceClick;
        onPieceMoved -= HandlePieceMoved;
        onPiecePlaced -= HandlePiecePlaced;
    }

    public void HandlePieceClick(GameObject piece)
    {
        BoardHelper.ClearTiles();
        startSquare = new Square(piece.transform.position);
        legalMoves = GameController.Instance.MainBoard.FindLegalMoves(startSquare);

        // Only highlight if the piece wasn't already highlighted
        if (!startSquare.IsHighlighted())
        {
            HighLightSquare(startSquare);
            HightLightLegalMoves(legalMoves);
        }
    }

    public void HandlePiecePlaced(GameObject piece)
    {
        // Make sure move is valid
        try
        {
            var endSquare = new Square(piece.transform.position);
            var moveMade = new Move(startSquare, endSquare);
            moveMade.Castling = GameController.Instance.MainBoard.IsCastleMove(moveMade);

            // Reset the piece if move was not legal
            if (!legalMoves.Contains(moveMade))
            {
                piece.transform.position = startSquare.ScreenPosition;
            }
            else
            {
                onMoveMade?.Invoke(moveMade);
            }
        }
        catch (ArgumentOutOfRangeException) { }
    }

    public void HandlePieceMoved(GameObject piece)
    {
        BoardHelper.ClearTiles();
    }
}