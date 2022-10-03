using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles manual movement of chess pieces
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class MovePiece : MonoBehaviour
{
    private Vector3 offset;
    private ChessSquare startingSquare;
    private ChessSquare clickedSquare;

    void OnMouseDown()
    {
        // Gets initial click position and tile
        Vector3 clickPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(clickPos);
        Vector3 initialPos = Camera.main.ScreenToWorldPoint(clickPos) + offset;
        startingSquare = new ChessSquare(initialPos);
    }

    void OnMouseDrag()
    {
        // Checks if the clicked piece is the right colour
        if (Board.ValidPieceClicked(gameObject))
        {
            // Finds the square the piece has been dragged to and moves it to that square
            Vector3 startingPos = transform.position;
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            clickedSquare = new ChessSquare(curPosition);
            transform.position = clickedSquare.Location;
            
            // If piece has been moved, unhighlight the tiles
            if (transform.position != startingPos) 
                HighlightSquares.ClearTiles();
        }
    }

    private void OnMouseUp()
    {
        // Gets the legal moves for that piece
        List<ChessSquare> legalMoves = GameController.GetLegalMoves(startingSquare, gameObject);
        legalMoves.Add(startingSquare);

        // If the new square is illegal, move the piece back to where it started
        if (!legalMoves.Any(x => x.Location == clickedSquare.Location))
            transform.position = startingSquare.Location;

        // If the piece has moved, go to next turn
        if (transform.position != startingSquare.Location)
            GameController.playerTurn = !GameController.playerTurn;
    }
}
