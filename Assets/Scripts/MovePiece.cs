using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

/// <summary>
/// Handles manual movement of chess pieces
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class MovePiece : MonoBehaviour
{
    private Vector3 offset;
    private ChessSquare startingSquare;
    private ChessSquare clickedSquare;
    private SortingGroup rend;
    [Tooltip("The current legal moves for the selected piece")]
    List<ChessSquare> LegalMoves { get; set; }

    private void Start()
    {
        rend = GetComponent<SortingGroup>();

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject[] pieces = PieceManager.Instance.pieces;

        // Reset rotations
        camera.transform.eulerAngles = new Vector3(0, 0, 0);

        foreach (GameObject piece in pieces)
            piece.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void OnMouseDown()
    {
        // Puts selected piece on top of all other pieces
        rend.sortingOrder++;
        // Gets initial click position and tile
        Vector3 clickPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(clickPos);
        Vector3 initialPos = Camera.main.ScreenToWorldPoint(clickPos) + offset;
        startingSquare = new ChessSquare(initialPos);

        // Gets the legal moves for that piece
        LegalMoves = GameController.GetLegalPieceMoves(startingSquare, gameObject, GameController.playerTurn);
        LegalMoves.Add(startingSquare);
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
        GameObject takenPiece = Board.TakePiece(gameObject);

        // If the new square is illegal or the player is now in check, move the piece back to where it started
        if (!LegalMoves.Any(x => x.Location == clickedSquare.Location) || GameController.IsInCheck(GameController.playerTurn))
        {
            transform.position = startingSquare.Location;
            if (takenPiece != null) takenPiece.SetActive(true);
        }

        // If the piece has moved, go to next turn and flip the board
        if (transform.position != startingSquare.Location)
        {
            GameController.playerTurn = !GameController.playerTurn;
            Board.FlipBoard();
        }

        // Resets sorting order
        rend.sortingOrder = 0;
    }
}