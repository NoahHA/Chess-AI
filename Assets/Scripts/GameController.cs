using System.Collections;
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
        MainBoard.SetBoardToStartingPosition();
        BoardHelper.UpdateScreenFromBoard(MainBoard);
        _playerInputManager = GetComponent<PlayerInputManager>();
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
        }

        if (AiMode && !stop)
        {
            // Update screen before finding AI move
            MainBoard.ChangeTurn();
            BoardHelper.UpdateScreenFromBoard(MainBoard);
            MainBoard.ChangeTurn();
            yield return null;

            Move computerMove = AIController.GetBestMove(MainBoard, depth);
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