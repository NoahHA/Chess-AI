using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the control flow of the game.
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;

    public bool AiMode { get; set; }
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

        if (AiSettings.Benchmarking)
        {
            startMenu.SetActive(false);
            StartCoroutine(BenchmarkAi());
        }
    }

    private IEnumerator BenchmarkAi()
    {
        int turnsPlayed = 0;
        AIController.benchmarking.StartBenchmarking();

        while (turnsPlayed < AiSettings.MaxComputerTurns)
        {
            ProcessMove(AIController.GetBestMove(MainBoard, PieceColour.White), rotate: false);
            yield return null;

            ProcessMove(AIController.GetBestMove(MainBoard, PieceColour.Black), rotate: false);
            yield return null;

            turnsPlayed++;
        }

        AIController.benchmarking.WriteMetricsToCsv(AiSettings.VersionDescription, AiSettings.TimeLimit_ms);
        AiSettings.Benchmarking = false;
    }

    private void ResetGame()
    {
        MainBoard.SetBoardToStartingPosition();
        BoardHelper.UpdateScreenFromBoard(MainBoard);
        startMenu.SetActive(true);
    }

    private void ProcessMove(Move move, bool rotate = true)
    {
        BoardHelper.ClearTiles(removeAll: true);
        MainBoard.MakeMove(move);
        MainBoard.ChangeTurn();
        BoardHelper.UpdateScreenFromBoard(MainBoard, rotate);

        if (MainBoard.IsInCheckmate(MainBoard.Turn))
        {
            onCheckmate?.Invoke(MainBoard.Turn.ChangeTurn());
        }
        else if (MainBoard.IsInStalemate(MainBoard.Turn))
        {
            onCheckmate?.Invoke(MainBoard.Turn);
        }

        _playerInputManager.HighLightSquare(move.StartSquare, TileType.MoveMadeTile);
        _playerInputManager.HighLightSquare(move.EndSquare, TileType.MoveMadeTile);
    }

    private IEnumerator HandleMoveMade(Move move)
    {
        ProcessMove(move);

        if (AiMode)
        {
            // Update screen before finding AI move
            MainBoard.ChangeTurn();
            BoardHelper.UpdateScreenFromBoard(MainBoard);
            MainBoard.ChangeTurn();
            yield return null;

            Move computerMove = AIController.GetBestMove(MainBoard);
            ProcessMove(computerMove);
        }
        else
        {
            BoardHelper.FlipCamera();
        }
    }

    public void HandleCheckmate(PieceColour winner)
    {
        Debug.Log($"Checkmate: {winner} wins!");
        ResetGame();
    }

    private void OnEnable()
    {
        PlayerInputManager.onMoveMade += move => StartCoroutine(HandleMoveMade(move));
        onCheckmate += HandleCheckmate;
    }

    private void OnDisable()
    {
        PlayerInputManager.onMoveMade -= move => StartCoroutine(HandleMoveMade(move));
        onCheckmate -= HandleCheckmate;
    }
}