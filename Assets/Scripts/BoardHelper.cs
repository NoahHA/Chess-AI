using UnityEngine;

/// <summary>
/// Some static helper functions for board operations.
/// </summary>
public static class BoardHelper
{
    public static GameObject[] GetPieces()
    {
        return GameObject.FindGameObjectsWithTag("Piece");
    }

    public static GameObject[] GetTiles()
    {
        return GameObject.FindGameObjectsWithTag("Highlight");
    }

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
                InstantiatePiece(piece, position);
            }
        }
    }

    /// <summary>
    /// Instantiates a piece prefab on a given square.
    /// </summary>
    /// <param name="piece">The piece to create.</param>
    /// <param name="square">The square on which to create the piece.</param>
    /// <returns>The created game object.</returns>
    public static GameObject InstantiatePiece(Piece piece, Square square)
    {
        GameObject pieceGameObject = (GameObject)GameObject.Instantiate(
            Resources.Load("Pieces/" + piece.GetPrefabName()), square.ScreenPosition, Quaternion.identity
        );

        pieceGameObject.transform.parent = GameObject.Find("Board/Pieces").transform;

        return pieceGameObject;
    }

    /// <summary>
    /// Removes all highlighted tiles to reset the board
    /// </summary>
    public static void ClearTiles()
    {
        foreach (var tile in GetTiles())
        {
            GameObject.Destroy(tile);
        }
    }
}