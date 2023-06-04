using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public ChessSquare StartSquare;
    public ChessSquare EndSquare;

    public Move(ChessSquare startSquare, ChessSquare endSquare)
    {
        (StartSquare, EndSquare) = (startSquare, endSquare);
    }

    public Move(string startSquare, string endSquare)
    {
        (StartSquare, EndSquare) = (new ChessSquare(startSquare), new ChessSquare(endSquare));
    }
}