using UnityEngine;

/// <summary>
/// Handles the control flow of the game.
/// </summary>
public class GameController : MonoBehaviour
{
    public delegate void OnCheckmate(PieceColour turn);

    public static OnCheckmate onCheckmate;

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

    private void HandleMoveMade(Move move)
    {
        MainBoard.MakeMove(move);
        BoardHelper.UpdateScreenFromBoard(MainBoard);
        MainBoard.ChangeTurn();

        if (MainBoard.IsInCheckmate())
        {
            onCheckmate?.Invoke(MainBoard.Turn);
        }
    }

    public void HandleCheckmate(PieceColour turn)
    {
        Debug.Log("Checkmate!");
    }

    private void OnEnable()
    {
        PlayerInputManager.onMoveMade += HandleMoveMade;
        onCheckmate += HandleCheckmate;
    }

    private void OnDisable()
    {
        PlayerInputManager.onMoveMade -= HandleMoveMade;
        onCheckmate -= HandleCheckmate;
    }
}