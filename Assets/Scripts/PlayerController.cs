using UnityEngine;

/// <summary>
/// Handles player inputs.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Board board = new Board();

    public delegate void OnPieceClicked(GameObject piece);
    public static OnPieceClicked onPieceClicked;

    public delegate void OnPieceDragged(GameObject piece);
    public static OnPieceClicked onPieceDragged;

    public delegate void OnPiecePlaced(GameObject piece);
    public static OnPieceClicked onPiecePlaced;

    public void OnEnable()
    {
        onPieceClicked += HandleClick;
    }

    public void OnDisable()
    {
        onPieceClicked -= HandleClick;
    }

    public void HandleClick(GameObject piece)
    {
        board.UpdateBoardFromScreen();
        //board.GetLegalMoves();
    }

    // on piece placed, check if the board state is in the list of legal board
    // states, if not then reset the board to how it was.

    // Also on click highlight all legal move squares for the clicked piece
}