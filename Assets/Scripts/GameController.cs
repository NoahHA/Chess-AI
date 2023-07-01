using UnityEngine;

/// <summary>
/// Handles the control flow of the game.
/// </summary>
public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameController>();
            }

            return _instance;
        }
    }

    public Board Board = new Board(turn: PieceColour.White);

    private void Start()
    {
        Board.SetBoardToStartingPosition();
        Board.UpdateScreenFromBoard();
    }

    private void Update()
    {
    }
}