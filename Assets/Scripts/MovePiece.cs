using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
public class MovePiece : MonoBehaviour
{
    private Vector3 offset;
    private ChessSquare startingSquare;
    private ChessSquare clickedSquare;

    void OnMouseDown()
    {
        Vector3 clickPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(clickPos);
        Vector3 initialPos = Camera.main.ScreenToWorldPoint(clickPos) + offset;
        startingSquare = new ChessSquare(initialPos);
    }

    void OnMouseDrag()
    {
        if ((gameObject.name.Contains("b_") && !GameController.playerTurn) || (gameObject.name.Contains("w_") && GameController.playerTurn))
        {
            Vector3 startingPos = transform.position;
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            clickedSquare = new ChessSquare(curPosition);
            transform.position = clickedSquare.Location;
        
            if (transform.position != startingPos) 
                HighlightSquares.ClearTiles();
        }
    }

    private void OnMouseUp()
    {
        List<ChessSquare> legalMoves = GameController.GetLegalMoves(startingSquare, gameObject);
        legalMoves.Add(startingSquare);

        if (!legalMoves.Any(x => x.Location == clickedSquare.Location))
            transform.position = startingSquare.Location;

        if (transform.position != startingSquare.Location)
            GameController.playerTurn = !GameController.playerTurn;
    }
}
