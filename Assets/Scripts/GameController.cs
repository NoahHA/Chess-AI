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

    [SerializeField] private GameObject startMenu;

    [Header("AI Settings")]
    [Space(5)]
    [Tooltip("Whether to limit the AI to a certain depth or a certain amount of search time.")]
    [SerializeField] private AiLimitationMode AiLimitMode;

    [SerializeField] private int depth;
    [SerializeField] private float timeLimit_ms;

    [Header("Benchmarking Settings")]
    [Space(5)]
    [Tooltip("When true, the AI will play itself and record benchmarking data for analysis.")]
    public bool BenchmarkingMode = false;

    [Tooltip("Maximum number of computer vs computer turns, only used in benchmarking mode.")]
    [SerializeField] private int maxComputerTurns = 10;

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

    public Board MainBoard = new();

    public void Start()
    {
        ResetGame();
        BoardHelper.UpdateScreenFromBoard(MainBoard);
        _playerInputManager = GetComponent<PlayerInputManager>();

        if (BenchmarkingMode)
        {
            startMenu.SetActive(false);
            StartCoroutine(PlayComputerVsComputer());
        }
    }

    private IEnumerator PlayComputerVsComputer()
    {
        int turnsPlayed = 0;

        while (turnsPlayed < maxComputerTurns)
        {
            Move whiteMove = (AiLimitMode == AiLimitationMode.Depth) ? AIController.GetBestMove(MainBoard, depth, PieceColour.White) : AIController.GetBestMove(MainBoard, timeLimit_ms, PieceColour.White);
            MainBoard.MakeMove(whiteMove);

            if (MainBoard.IsInCheckmate(PieceColour.White) || MainBoard.IsInStalemate(PieceColour.White))
            {
                break;
            }

            MainBoard.ChangeTurn();
            BoardHelper.UpdateScreenFromBoard(MainBoard, rotate: false);
            yield return null;

            Move blackMove = (AiLimitMode == AiLimitationMode.Depth) ? AIController.GetBestMove(MainBoard, depth, PieceColour.Black) : AIController.GetBestMove(MainBoard, timeLimit_ms, PieceColour.Black);
            MainBoard.MakeMove(blackMove);

            if (MainBoard.IsInCheckmate(PieceColour.Black) || MainBoard.IsInStalemate(PieceColour.Black))
            {
                break;
            }

            MainBoard.ChangeTurn();
            BoardHelper.UpdateScreenFromBoard(MainBoard, rotate: false);
            yield return null;

            turnsPlayed++;
        }

        Debug.Log("Game Over");
        BenchmarkingMode = false;
        ResetGame();
    }

    private void ResetGame()
    {
        MainBoard.SetBoardToStartingPosition();
        BoardHelper.UpdateScreenFromBoard(MainBoard);
        startMenu.SetActive(true);
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