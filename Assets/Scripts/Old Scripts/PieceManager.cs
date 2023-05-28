using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class to hold chess pieces
/// </summary>
public class PieceManager : MonoBehaviour
{
    [Tooltip("List of chess piece GameObjects")]
    public List<GameObject> pieces;

    public static PieceManager Instance { get; private set; }

    public void TestFUnctionBUtton()
    {
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
    }
}