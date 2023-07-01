using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider2D), typeof(SortingGroup))]
public class ClickPiece : MonoBehaviour
{
    private PieceID pieceID;
    private Square curSquare;
    private SortingGroup rend;
    private Square startingSquare;

    public void Start()
    {
        rend = GetComponent<SortingGroup>();
        pieceID = GetComponent<PieceID>();
    }

    public void OnMouseDown()
    {
        // Checks if the clicked piece is the right colour
        if (GameController.Instance.Board.Turn == pieceID.Piece.Colour)
        {
            // Puts selected piece on top of all other pieces
            rend.sortingOrder++;

            startingSquare = new Square(transform.position);

            // Trigger a piece clicked event
            PlayerController.onPieceClicked?.Invoke(gameObject);
        }
    }

    public void OnMouseDrag()
    {
        // Checks if the clicked piece is the right colour
        if (GameController.Instance.Board.Turn == pieceID.Piece.Colour)
        {
            SnapPieceToSquare(gameObject);

            // If piece has moved to a new square
            if (transform.position != startingSquare.ScreenPosition)
            {
                // Trigger a piece dragged event
                PlayerController.onPieceMoved?.Invoke(gameObject);
            }
        }
    }

    public void OnMouseUp()
    {
        // Checks if the clicked piece is the right colour
        if (GameController.Instance.Board.Turn == pieceID.Piece.Colour)
        {
            // Reset sorting order
            rend.sortingOrder = 0;

            // Trigger a piece placed event
            PlayerController.onPiecePlaced?.Invoke(gameObject);
        }
    }

    private void SnapPieceToSquare(GameObject piece)
    {
        // Finds the square the piece has been dragged to
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        curSquare = new Square(Camera.main.ScreenToWorldPoint(curScreenPoint));

        // Snaps piece into the centre of the square
        piece.transform.position = curSquare.ScreenPosition;
    }
}