using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceID : MonoBehaviour
{
    [SerializeField] private PieceType type;
    [SerializeField] private PieceColour colour;
    public Piece Piece { get; private set; }

    private void Awake()
    {
        Piece = new Piece(type, colour);
    }
}