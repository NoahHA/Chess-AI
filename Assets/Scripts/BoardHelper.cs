using UnityEngine;

/// <summary>
/// Some static helper functions for board operations.
/// </summary>
public static class BoardHelper
{
    public static GameObject[] GetPieces() => GameObject.FindGameObjectsWithTag("Piece");

    public static GameObject[] GetLongTermTiles() => GameObject.FindGameObjectsWithTag("LongHighlight");

    public static GameObject[] GetShortTermTiles() => GameObject.FindGameObjectsWithTag("ShortHighlight");

    public static GameObject GetCamera() => GameObject.FindGameObjectWithTag("MainCamera");

    public static void ClearScreen()
    {
        foreach (var piece in GetPieces())
        {
            GameObject.Destroy(piece);
        }
    }

    /// <summary>
    /// Updates the screen based on the given board state.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static void UpdateScreenFromBoard(Board board)
    {
        ClearScreen();

        for (int i = 0; i < 64; i++)
        {
            var position = new Square(i);
            Piece piece = board.FindPieceOnSquare(position);

            if (piece != null)
            {
                InstantiatePiece(piece, position, board.Turn);
            }
        }
    }

    /// <summary>
    /// Instantiates a piece prefab on a given square.
    /// </summary>
    /// <param name="piece">The piece to create.</param>
    /// <param name="square">The square on which to create the piece.</param>
    /// <returns>The created game object.</returns>
    public static GameObject InstantiatePiece(Piece piece, Square square, PieceColour turn)
    {
        Quaternion rotation = (turn == PieceColour.White) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);
        GameObject pieceGameObject = (GameObject)Object.Instantiate(
            Resources.Load("Pieces/" + piece.GetPrefabName()), square.ScreenPosition, rotation
        );

        pieceGameObject.transform.parent = GameObject.Find("Board/Pieces").transform;

        return pieceGameObject;
    }

    /// <summary>
    /// Removes all highlighted tiles to reset the board
    /// </summary>
    public static void ClearTiles(bool removeAll = false)
    {
        foreach (var tile in GetShortTermTiles())
        {
            Object.Destroy(tile);
        }

        if (removeAll)
        {
            foreach (var tile in GetLongTermTiles())
            {
                Object.Destroy(tile);
            }
        }
    }

    /// <summary>
    /// Flips the board perspective.
    /// </summary>
    public static void FlipCamera()
    {
        GameObject camera = GetCamera();
        camera.transform.Rotate(0, 0, 180);
    }
}