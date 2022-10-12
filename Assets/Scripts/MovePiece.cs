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
        GameObject takenPiece = Board.TakePiece(gameObject);

        // If the new square is illegal or the player is now in check, move the piece back to where
        // it started
        if (!LegalMoves.Any(move => move.Location == clickedSquare.Location) || GameController.IsInCheck(GameController.playerTurn))
        {
            transform.position = startingSquare.Location;
            // Untake the piece
            if (takenPiece != null)
                takenPiece.SetActive(true);
        }

        // If the piece has moved, go to next turn and flip the board
        else if (startingSquare.Location != transform.position)
        {
            // If player has castled, move the castle to the right spot
            if (gameObject.name.Contains("king"))
            {
                ChessSquare kingSquare = GameController.FindKing(GameController.playerTurn);

                if (Math.Abs(kingSquare.Col - startingSquare.Col) == 2)
                {
                    // If player castled queenside
                    var newCastlePosition = new ChessSquare(kingSquare.Row, kingSquare.Col + 1);
                    var castlePosition = new ChessSquare(startingSquare.Row, startingSquare.Col - 4);

                    // If player castled kingside
                    if (kingSquare.Col > startingSquare.Col)
                    {
                        newCastlePosition = new ChessSquare(kingSquare.Row, kingSquare.Col - 1);
                        castlePosition = new ChessSquare(startingSquare.Row, startingSquare.Col + 3);
                    }

                    GameObject castlePiece = Board.FindPieceOnSquare(castlePosition).gameObject;
                    castlePiece.transform.position = newCastlePosition.Location;
                }
            }

            // Set pieceMoved to true
            var pieceMoved = gameObject.GetComponent<HasPieceMoved>();
            pieceMoved.hasMoved = true;

            // If pawn has gotten to the end, turn it into a queen
            if (gameObject.name.Contains("pawn"))
            {
                var pawnSquare = new ChessSquare(transform.position);
                if (pawnSquare.Row == 0 || pawnSquare.Row == 7)
                {
                    // letter defines whether queen is white or black
                    char letter = 'q';
                    if (GameController.playerTurn) letter = 'Q';

                    GameObject queenPiece = Board.GetPieceFromLetter(letter);
                    Instantiate(queenPiece, pawnSquare.Location, gameObject.transform.rotation);
                    gameObject.SetActive(false);
                }
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