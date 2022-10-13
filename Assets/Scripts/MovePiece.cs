using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Handles manual movement of chess pieces.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class MovePiece : MonoBehaviour
{
    private GameObject checkmateOverlay;

    private Vector3 offset;

    private ChessSquare startingSquare;

    private ChessSquare clickedSquare;

    private SortingGroup rend;

    private List<ChessSquare> LegalMoves { get; set; }

    private void Start()
    {
        rend = GetComponent<SortingGroup>();
        checkmateOverlay = GameObject.FindGameObjectWithTag("Checkmate");
    }

    private void OnMouseDown()
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

    private void OnMouseDrag()
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
        // If the piece has moved
        if (startingSquare.Location != transform.position)
        {
            GameObject takenPiece = Board.TakePiece(gameObject);

            // If the new square is illegal or the player is now in check, move the piece back to
            // where it started
            if (!LegalMoves.Any(move => move.Location == clickedSquare.Location) || GameController.IsInCheck(GameController.playerTurn))
            {
                transform.position = startingSquare.Location;
                // Untake the piece
                if (takenPiece != null)
                    takenPiece.SetActive(true);
            }

            // If player has castled
            if (gameObject.name.Contains("king"))
            {
                ChessSquare kingSquare = GameController.FindKing(GameController.playerTurn);

                if (Math.Abs(kingSquare.Col - startingSquare.Col) == 2)
                    Board.Castle(kingSquare, startingSquare, GameController.playerTurn);
            }

            // Set pieceMoved to true
            var pieceMoved = gameObject.GetComponent<HasPieceMoved>();
            pieceMoved.hasMoved = true;

            // If pawn has gotten to the end, turn it into a queen
            if (gameObject.name.Contains("pawn"))
            {
                var pawnSquare = new ChessSquare(transform.position);
                if (pawnSquare.Row == 0 || pawnSquare.Row == 7)
                    Board.QueenPawn(gameObject, GameController.playerTurn);
            }

            // Change turns
            GameController.playerTurn = !GameController.playerTurn;

            // If player is in checkmate, activate the checkmate overlay
            if (GameController.IsInCheckmate(GameController.playerTurn))
                checkmateOverlay.transform.Find("CheckmateText").gameObject.SetActive(true);

            Board.FlipBoard();
        }

        // Resets sorting order
        rend.sortingOrder = 0;
    }
}