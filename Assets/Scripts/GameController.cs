using UnityEngine;

public class GameController : MonoBehaviour
{
    public Board board = new Board(turn: PieceColour.White);

    private void Start()
    {
        board.SetBoardToStartingPosition();
        board.UpdateScreenFromBoard();
    }

    private void Update()
    {
    }
}