using UnityEngine;

/// <summary>
/// Remembers if a piece has previously moved, needs to be checked for castling.
/// </summary>
public class HasPieceMoved : MonoBehaviour
{
    [Tooltip("True if the piece has been moved during this game, false otherwise.")]
    public bool hasMoved;

    private void Awake() => hasMoved = false;
}