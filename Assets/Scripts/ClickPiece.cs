using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider2D), typeof(SortingGroup))]
public class ClickPiece : MonoBehaviour
{
    private PieceID pieceID;
    private Square curSquare;
    private SortingGroup rend;
    private Square startSquare;

    public void Start()
    {
        rend = GetComponent<SortingGroup>();
        pieceID = GetComponent<PieceID>();
    }

    public void OnMouseDown()
    {
        // Checks if the clicked piece is the right colour
        if (GameController.Instance.MainBoard.Turn == pieceID.Piece.Colour)
        {
            // Puts selected piece on top of all other pieces
            rend.sortingOrder++;

            startSquare = new Square(transform.position);

            // Trigger a piece clicked event
            PlayerInputManager.onPieceClicked?.Invoke(gameObject);
        }
    }

    public void OnMouseDrag()
    {
        // Checks if the clicked piece is the right colour
        if (GameController.Instance.MainBoard.Turn == pieceID.Piece.Colour)
        {
            SnapPieceToSquare(gameObject);

            // If piece has moved to a new square
            if (transform.position != startSquare.ScreenPosition)
            {
                // Trigger a piece dragged event
                PlayerInputManager.onPieceMoved?.Invoke(gameObject);
            }
        }
    }

    public void OnMouseUp()
    {
        // Checks if the clicked piece is the right colour
        if (GameController.Instance.MainBoard.Turn == pieceID.Piece.Colour)
        {
            // Reset sorting order
            rend.sortingOrder = 0;

            // If piece has moved to a new square
            if (transform.position != startSquare.ScreenPosition)
            {
                // Trigger a piece placed event
                PlayerInputManager.onPiecePlaced?.Invoke(gameObject);
            }
        }
    }

    private void SnapPieceToSquare(GameObject piece)
    {
        // Finds the square the piece has been dragged to
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        try
        {
            curSquare = new Square(Camera.main.ScreenToWorldPoint(curScreenPoint));

            // Snaps piece into the centre of the square
            piece.transform.position = curSquare.ScreenPosition;
        }
        catch (ArgumentOutOfRangeException) { }
    }
}