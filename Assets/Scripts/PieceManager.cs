using UnityEngine;

/// <summary>
/// Singleton class to hold chess pieces
/// </summary>
public class PieceManager : MonoBehaviour
{
    public static PieceManager Instance { get; private set; }
    [Tooltip("List of chess piece GameObjects")]
    public GameObject[] pieces;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) { Destroy(this); }
        else { Instance = this; }
    }
}
