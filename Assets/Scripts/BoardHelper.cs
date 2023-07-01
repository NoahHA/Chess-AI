using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHelper : MonoBehaviour
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