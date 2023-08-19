using UnityEngine;

/// <summary>
/// A script to attach to piece prefabs to identify what type of piece they are.
/// </summary>
public class PieceID : MonoBehaviour
{
    [SerializeField] private PieceType type;
    [SerializeField] private PieceColour colour;
    public Piece Piece { get; private set; }

    public void Awake()
    {
        Piece = new Piece(type, colour);
    }
}