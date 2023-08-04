using System.Linq;
using UnityEngine;

/// <summary>
/// Handles the control flow of the game.
/// </summary>
public class GameController : MonoBehaviour
{
    public bool AiMode { get; set; }

    [SerializeField] private int depth;

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
        MainBoard.ChangeTurn();
        BoardHelper.UpdateScreenFromBoard(MainBoard);

        if (MainBoard.IsInCheckmate(MainBoard.Turn))
        {
            onCheckmate?.Invoke(MainBoard.Turn);
        }

        if (AiMode)
        {
            Move computerMove = AIController.GetBestMove(MainBoard, depth);
            MainBoard.MakeMove(computerMove);
            MainBoard.ChangeTurn();
            BoardHelper.UpdateScreenFromBoard(MainBoard);
        }
        else
        {
            BoardHelper.FlipCamera();
        }
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