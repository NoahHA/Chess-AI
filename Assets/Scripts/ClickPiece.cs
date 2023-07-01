using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider2D), typeof(SortingGroup))]
public class ClickPiece : MonoBehaviour
{
    private PieceID pieceID;
    private Square startingSquare;
    private Square curSquare;
    private SortingGroup rend;

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

            // Stores initial piece location
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
            // Trigger a piece dragged event
            PlayerController.onPieceDragged?.Invoke(gameObject);
        }
    }

    public void OnMouseUp()
    {
        // Checks if the clicked piece is the right colour
        if (GameController.Instance.Board.Turn == pieceID.Piece.Colour)
        {
            rend.sortingOrder = 0;

            // Trigger a piece dragged event
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

    public void OnEnable()
    {
        PlayerController.onPieceDragged += SnapPieceToSquare;
    }

    public void OnDisable()
    {
        PlayerController.onPieceDragged -= SnapPieceToSquare;
    }
}