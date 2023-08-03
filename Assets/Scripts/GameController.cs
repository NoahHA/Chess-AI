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
                _instance = FindObjectOfType<GameController>();
            }

            return _instance;
        }
    }

    public Board MainBoard = new Board();

    public void Start()
    {
        MainBoard.SetBoardToStartingPosition();
        BoardHelper.UpdateScreenFromBoard(MainBoard);
    }

    public void HandleMoveMade(Move move)
    {
        MainBoard.MakeMove(move);
        BoardHelper.UpdateScreenFromBoard(MainBoard);
        MainBoard.ChangeTurn();
    }

    private void OnEnable()
    {
        PlayerInputManager.onMoveMade += HandleMoveMade;
    }

    private void OnDisable()
    {
        PlayerInputManager.onMoveMade -= HandleMoveMade;
    }
}