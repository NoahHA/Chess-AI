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

    [Test]
    public void TestGenerateFenFromPiecesSimple()
    {
        // Arrange
        Piece[] pieces = new Piece[64];
        pieces[3] = new Piece(PieceType.Queen, PieceColour.Black);

        // Act
        BoardState boardState = new BoardState(pieces);

        // Assert
        string expectedFen = "3Q4/8/8/8/8/8/8/8";
        Assert.AreEqual(expectedFen, boardState.FEN);
    }

    [Test]
    public void TestGenerateFenFromPiecesStartingPosition()
    {
        // Arrange
        var tempBoardState = new BoardState();
        tempBoardState.SetBoardToStartingPosition();
        Piece[] pieces = tempBoardState.State;

        // Act
        var boardState = new BoardState(pieces);

        // Assert
        string expectedFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        Assert.AreEqual(expectedFen, boardState.FEN);
    }
}