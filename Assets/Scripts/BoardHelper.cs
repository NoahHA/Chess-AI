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

    public static void ClearScreen()
    {
        foreach (GameObject piece in GetPieces())
        {
            GameObject.Destroy(piece);
        }
    }
}