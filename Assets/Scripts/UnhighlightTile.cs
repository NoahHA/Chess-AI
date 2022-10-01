using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnhighlightTile : MonoBehaviour
{
    void OnMouseDown()
    {
        HighlightSquares.ClearTiles();
    }
}
