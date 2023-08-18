using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the control flow of the game.
/// </summary>
public class GameController : MonoBehaviour
{
    public enum AiLimitationMode
    {
        Time, Depth
    }

    public bool AiMode { get; set; }

    [Tooltip("Whether to limit the AI to a certain depth or a certain amount of search time.")]
    [SerializeField] private AiLimitationMode AiLimitMode;

    [SerializeField] private int depth;
    [SerializeField] private float timeLimit_ms;

    public delegate void OnCheckmate(PieceColour turn);

    public static OnCheckmate onCheckmate;

    private static GameController _instance;
    private PlayerInputManager _playerInputManager;

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
        ResetGame();
        BoardHelper.UpdateScreenFromBoard(MainBoard);
        _playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void ResetGame()
    {
        MainBoard.SetBoardToStartingPosition();
        BoardHelper.UpdateScreenFromBoard(MainBoard);
    }

    private IEnumerator HandleMoveMade(Move move, bool stop = false)
    {
        BoardHelper.ClearTiles(removeAll: true);
        MainBoard.MakeMove(move);
        MainBoard.ChangeTurn();
        BoardHelper.UpdateScreenFromBoard(MainBoard);

        if (MainBoard.IsInCheckmate(MainBoard.Turn))
        {
            onCheckmate?.Invoke(MainBoard.Turn);
            yield break;
        }
        else if (MainBoard.IsInStalemate(MainBoard.Turn))
        {
            onCheckmate?.Invoke(MainBoard.Turn.ChangeTurn());
            yield break;
        }

        if (AiMode && !stop)
        {
            // Update screen before finding AI move
            MainBoard.ChangeTurn();
            BoardHelper.UpdateScreenFromBoard(MainBoard);
            MainBoard.ChangeTurn();
            yield return null;

            Move computerMove = (AiLimitMode == AiLimitationMode.Depth) ? AIController.GetBestMove(MainBoard, depth) : AIController.GetBestMove(MainBoard, timeLimit_ms);
            StartCoroutine(HandleMoveMade(computerMove, stop: true));
        }
        else if (!AiMode)
        {
            BoardHelper.FlipCamera();
        }

        if (stop)
        {
            _playerInputManager.HighLightSquare(move.StartSquare, TileType.MoveMadeTile);
            _playerInputManager.HighLightSquare(move.EndSquare, TileType.MoveMadeTile);
        }
    }

    private void HandleMoveMade(Move move)
    {
        if (AiMode)
        {
            StartCoroutine(HandleMoveMade(move, false));
        }
        else
        {
            StartCoroutine(HandleMoveMade(move, true));
        }
    }

    public void HandleCheckmate(PieceColour turn)
    {
        MainBoard.ChangeTurn();
        Debug.Log($"Checkmate: {MainBoard.Turn} wins!");
        ResetGame();
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