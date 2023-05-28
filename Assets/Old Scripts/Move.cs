using UnityEngine;

/// <summary>
/// Handles behaviour for moves in the game.
/// </summary>
public class Move
{
    public Move()
    { }

    public Move(GameObject piece, ChessSquare move)
    {
        Piece = piece;
        Square = move;
        StartingSquare = new ChessSquare(piece.transform.position);
    }

    [Tooltip("The chess piece being moved")]
    public GameObject Piece { get; private set; }

    [Tooltip("The chess square that the piece is being moved to")]
    public ChessSquare Square { get; private set; }

    [Tooltip("The chess square that the piece was on before making the move")]
    public ChessSquare StartingSquare { get; private set; }

    [Tooltip("The piece, if any, taken by the move")]
    public GameObject TakenPiece { get; private set; }

    public void MakeMove()
    {
        Piece.transform.position = Square.Location; // Make the move
        TakenPiece = Board.TakePiece(Piece); // Take any takeable pieces
    }

    public void UnmakeMove()
    {
        if (TakenPiece != null)
        {
            TakenPiece.SetActive(true); // Untake the piece
        }

        Piece.transform.position = StartingSquare.Location; // Undo move
    }
}