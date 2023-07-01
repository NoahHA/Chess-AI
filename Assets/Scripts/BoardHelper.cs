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