using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider2D), typeof(SortingGroup))]
public class ClickPiece : MonoBehaviour
{
    private PieceID pieceID;
    private Square startingSquare;
    private Square curSquare;
    private SortingGroup rend;

    private void Start()
    {
        rend = GetComponent<SortingGroup>();
        pieceID = GetComponent<PieceID>();
    }

    private void OnMouseDown()
    {
        // Puts selected piece on top of all other pieces
        rend.sortingOrder++;

        // Stores initial piece location
        startingSquare = new Square(transform.position);
    }

    private void OnMouseDrag()
    {
        // Checks if the clicked piece is the right colour
        if (GameController.Instance.Board.Turn == pieceID.Piece.Colour)
        {
            // Finds the square the piece has been dragged to
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            curSquare = new Square(Camera.main.ScreenToWorldPoint(curScreenPoint));

            // Snaps piece into the centre of the square
            transform.position = curSquare.ScreenPosition;
        }
    }

    private void OnMouseUp()
    {
        // Resets sorting order
        rend.sortingOrder = 0;
    }
}