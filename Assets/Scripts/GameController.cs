using System.Linq;
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
        Debug.Log(MainBoard.FEN);
        MainBoard.ChangeTurn();
        BoardHelper.UpdateScreenFromBoard(MainBoard);

        if (MainBoard.IsInCheckmate())
        {
            onCheckmate?.Invoke(MainBoard.Turn);
        }

        BoardHelper.FlipCamera();
    }

    public void HandleCheckmate(PieceColour turn)
    {
        MainBoard.ChangeTurn();
        Debug.Log($"Checkmate: {MainBoard.Turn} wins!");
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