using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSquares : MonoBehaviour
{
    public GameObject tileHighlight;
    public GameObject circleHighlight;

    private void OnMouseDown()
    {
        ClearTiles();

        if ((gameObject.name.Contains("b_") && !GameController.playerTurn) || (gameObject.name.Contains("w_") && GameController.playerTurn))
        {
            var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            ChessSquare clickedSquare = new ChessSquare(Camera.main.ScreenToWorldPoint(mousePos));
            HighLightCurrentSquare(clickedSquare);
            HightLightPossibleMoves(clickedSquare);
        }
    }

    public void HighLightCurrentSquare(ChessSquare clickedSquare)
    {
        var tile = Instantiate(this.tileHighlight, clickedSquare.Location, this.tileHighlight.transform.rotation);
        tile.tag = "Highlight";
    }

    public void HightLightPossibleMoves(ChessSquare clickedSquare)
    {
        List<ChessSquare> moves = GameController.GetLegalMoves(clickedSquare, gameObject);

        foreach (ChessSquare move in moves)
        {
            if (move.Row <= 7 && move.Row >= 0 && move.Col <= 7 && move.Col >= 0)
            {
                var tile = Instantiate(this.circleHighlight, move.Location, this.circleHighlight.transform.rotation);
                tile.tag = "Highlight";
            }
        }
    }

    public static void ClearTiles()
    {
        var tiles = GameObject.FindGameObjectsWithTag("Highlight");

        foreach (var tile in tiles)
            Destroy(tile);
    }
}
