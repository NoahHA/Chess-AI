using UnityEngine;

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