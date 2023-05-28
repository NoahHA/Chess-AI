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
}