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
        var tile = Instantiate(tileHighlight, clickedSquare.ScreenPosition, tileHighlight.transform.rotation);
    }

    /// <summary>
    /// Highlights all legal moves for the selected piece.
    /// </summary>
    public void HightLightLegalMoves(List<Move> legalMoves)
    {
        //// Loops through all legal moves and highlights them
        //foreach (Move move in moves)
        //{
        //    Collider2D pieceOnSquare = Board.FindPieceOnSquare(move.Square);

        //    // Highlight differently if there's a takeable enemy piece
        //    if (Board.FindPieceOnSquare(move.Square) != null)
        //    {
        //        if (Board.IsEnemyPiece(GameController.playerTurn, pieceOnSquare.gameObject))
        //            Instantiate(takeablePieceHighlight, move.Square.Location, circleHighlight.transform.rotation);
        //    }
        //    else
        //        Instantiate(circleHighlight, move.Square.Location, circleHighlight.transform.rotation);
        //}
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
            HighLightSquare(new Square(piece.transform.position));
            //HightLightLegalMoves(board.GetLegalMoves());
        }
    }

    public void HandlePieceMoved(GameObject piece)
    {
        BoardHelper.ClearTiles();
    }
}