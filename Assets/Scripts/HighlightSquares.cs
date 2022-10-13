using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles highlighting tiles to show possible moves and current selected piece
/// </summary>
public class HighlightSquares : MonoBehaviour
{
    [Tooltip("Highlight for currently selected piece")]
    public GameObject tileHighlight;

    [Tooltip("Highlight for possible moves for currently selected piece")]
    public GameObject circleHighlight;

    /// <summary>
    /// Removes all highlighted tiles to reset the board
    /// </summary>
    public static void ClearTiles()
    {
        var tiles = GameObject.FindGameObjectsWithTag("Highlight");

        foreach (var tile in tiles)
            Destroy(tile);
    }

    /// <summary>
    /// Highlights the square the player has clicked on in green
    /// </summary>
    /// <param name="clickedSquare"> The chess tile the player has clicked on </param>
    public void HighLightCurrentSquare(ChessSquare clickedSquare)
    {
        var tile = Instantiate(tileHighlight, clickedSquare.Location, tileHighlight.transform.rotation);
        tile.tag = "Highlight";
    }

    /// <summary>
    /// Highlights all possible moves for the selected piece with green circles
    /// </summary>
    /// <param name="clickedSquare"> The chess tile the player has clicked on </param>
    public void HightLightPossibleMoves(ChessSquare clickedSquare)
    {
        List<ChessSquare> moves = GameController.GetLegalPieceMoves(clickedSquare, gameObject, GameController.playerTurn);

        // Loops through all legal moves and highlights them
        foreach (ChessSquare move in moves)
        {
            if (move.Row <= 7 && move.Row >= 0 && move.Col <= 7 && move.Col >= 0)
            {
                var tile = Instantiate(circleHighlight, move.Location, circleHighlight.transform.rotation);
                tile.tag = "Highlight";
            }
        }
    }

    private void OnMouseDown()
    {
        // If clicked piece is valid, highlight the piece's tile and the possible moves
        ClearTiles();

        if (Board.ValidPieceClicked(gameObject))
        {
            var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            var clickedSquare = new ChessSquare(Camera.main.ScreenToWorldPoint(mousePos));
            HighLightCurrentSquare(clickedSquare);
            HightLightPossibleMoves(clickedSquare);
        }
    }
}