using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestBoardState
{
    [Test]
    public void TestGenerateFenEmptyBoard()
    {
        // Arrange
        string Fen = "8/8/8/8/8/8/8/8";

        // Act
        List<Piece> boardState = BoardState.GenerateBoardState(Fen);

        // Assert
        Assert.AreEqual(boardState, new BoardState().State);
    }

    [Test]
    public void TestGenerateFenPawns()
    {
        // Arrange
        string Fen = "8/p7/8/8/8/7P/8/8";

        // Act
        List<Piece> boardState = BoardState.GenerateBoardState(Fen);

        // Assert
        BoardState expectedState = new BoardState();
        expectedState.State[8] = new Piece(PieceType.Pawn, Colour.White);
        expectedState.State[47] = new Piece(PieceType.Pawn, Colour.Black);
        Assert.AreEqual(boardState, expectedState.State);
    }
}