using UnityEngine;

/// <summary>
/// A script to attach to piece prefabs to identify what type of piece they are.
/// </summary>
public class PieceID
{
    [SerializeField] private PieceType type;
    [SerializeField] private PieceColour colour;
    public Piece Piece => new Piece(type, colour);
}