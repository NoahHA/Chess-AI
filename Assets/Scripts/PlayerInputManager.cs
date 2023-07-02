using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player inputs.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Tooltip("Highlight for currently selected piece")]
    public GameObject tileHighlight;

    [Tooltip("Highlight for possible moves for currently selected piece")]
    public GameObject circleHighlight;

    [Tooltip("Highlight for enemy pieces that can be taken")]
    public GameObject takeablePieceHighlight;

    private Board board = new Board();

    public delegate void OnPieceClicked(GameObject piece);
    public static OnPieceClicked onPieceClicked;

    public delegate void OnPieceDragged(GameObject piece);
    public static OnPieceClicked onPieceMoved;

    public delegate void OnPiecePlaced(GameObject piece);
    public static OnPieceClicked onPiecePlaced;

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
            Piece pieceOnSquare = board.FindPieceOnSquare(move.EndSquare);

            if (pieceOnSquare == null)
            {
                Instantiate(circleHighlight, move.EndSquare.ScreenPosition, circleHighlight.transform.rotation);
            }

            // Highlight differently if there's a takeable enemy piece
            else if (board.IsEnemyPiece(pieceOnSquare))
            {
                Instantiate(takeablePieceHighlight, move.EndSquare.ScreenPosition, circleHighlight.transform.rotation);
            }
        }
    }

    public void OnEnable()
    {
        onPieceClicked += HandlePieceClick;
        onPieceMoved += HandlePieceMoved;
    }

    public void OnDisable()
    {
        onPieceClicked -= HandlePieceClick;
        onPieceMoved -= HandlePieceMoved;
    }

    public void HandlePieceClick(GameObject piece)
    {
        BoardHelper.ClearTiles();
        board.UpdateBoardFromScreen();

        if (GameObject.FindGameObjectWithTag("Highlight") == null)
        {
            Square square = new Square(piece.transform.position);
            HighLightSquare(square);
            HightLightLegalMoves(board.FindLegalMoves(square));
        }
    }

    public void HandlePieceMoved(GameObject piece)
    {
        BoardHelper.ClearTiles();
    }
}