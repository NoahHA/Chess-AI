using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameController
{
    public static GameObject[] Pieces => GameObject.FindGameObjectsWithTag("Piece");

    public static void ClearScreen()
    {
        foreach (GameObject piece in Pieces)
        {
            GameObject.Destroy(piece);
        }
    }
}