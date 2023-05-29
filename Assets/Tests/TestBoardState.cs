using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestBoardState
{
    [Test]
    public void TestGenerateBoardStateEmptyBoard()
    {
        // Arrange
        string Fen = "8/8/8/8/8/8/8/8";

        // Act
        BoardState boardState = new BoardState(PieceColour.None, Fen);

        // Assert
        Assert.AreEqual(new BoardState().State, boardState.State);
    }

    [Test]
    public void TestGenerateBoardStateSimplePosition()
    {
        // Arrange
        string Fen = "8/p7/8/8/8/7P/8/8";

        // Act
        BoardState boardState = new BoardState(PieceColour.None, Fen);

        // Assert
        BoardState expectedState = new BoardState();
        expectedState.State[8] = new Piece(PieceType.Pawn, PieceColour.White);
        expectedState.State[47] = new Piece(PieceType.Pawn, PieceColour.Black);
        Assert.AreEqual(expectedState.State, boardState.State);
    }

    [Test]
    public void TestSetBoardToStartingPosition()
    {
        // Arrange
        string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        // Act
        BoardState boardState = new BoardState(Fen);

        // Assert
        BoardState expectedState = new BoardState();
        expectedState.SetBoardToStartingPosition();
        Assert.AreEqual(expectedState.State, boardState.State);
    }

    [Test]
    public void TestGenerateBoardStateFenTooLong()
    {
        // Arrange
        string Fen = "8/8/8/8/9/8/8/8";

        // Assert
        Assert.Throws<ArgumentException>(() => new BoardState(Fen));
    }

    [Test]
    public void TestGenerateBoardStateInvalidFen()
    {
        // Arrange
        string Fen = "8/7T/8/8/8/8/8/8";

        // Assert
        Assert.Throws<ArgumentException>(() => new BoardState(Fen));
    }

    [Test]
    public void TestBoardStateValueEquality()
    {
        // Arrange
        string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        // Act
        var boardState1 = new BoardState(PieceColour.Black, Fen);
        var boardState2 = new BoardState(PieceColour.Black, Fen);

        // Assert
        Assert.AreEqual(boardState1, boardState2);
    }
}