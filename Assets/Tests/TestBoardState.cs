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
    public void TestGenerateFenSimplePosition()
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

    [Test]
    public void TestGenerateFenStartingPosition()
    {
        // Arrange
        string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        // Act
        List<Piece> boardState = BoardState.GenerateBoardState(Fen);

        // Assert
        BoardState expectedState = new BoardState();
        expectedState.State[0] = new Piece(PieceType.Rook, Colour.White);
        expectedState.State[1] = new Piece(PieceType.Knight, Colour.White);
        expectedState.State[2] = new Piece(PieceType.Bishop, Colour.White);
        expectedState.State[3] = new Piece(PieceType.Queen, Colour.White);
        expectedState.State[4] = new Piece(PieceType.King, Colour.White);
        expectedState.State[5] = new Piece(PieceType.Bishop, Colour.White);
        expectedState.State[6] = new Piece(PieceType.Knight, Colour.White);
        expectedState.State[7] = new Piece(PieceType.Rook, Colour.White);

        for (int i = 8; i < 16; i++)
        {
            expectedState.State[i] = new Piece(PieceType.Pawn, Colour.White);
        }

        for (int i = 48; i < 56; i++)
        {
            expectedState.State[i] = new Piece(PieceType.Pawn, Colour.Black);
        }

        expectedState.State[56] = new Piece(PieceType.Rook, Colour.Black);
        expectedState.State[57] = new Piece(PieceType.Knight, Colour.Black);
        expectedState.State[58] = new Piece(PieceType.Bishop, Colour.Black);
        expectedState.State[59] = new Piece(PieceType.Queen, Colour.Black);
        expectedState.State[60] = new Piece(PieceType.King, Colour.Black);
        expectedState.State[61] = new Piece(PieceType.Bishop, Colour.Black);
        expectedState.State[62] = new Piece(PieceType.Knight, Colour.Black);
        expectedState.State[63] = new Piece(PieceType.Rook, Colour.Black);

        Assert.AreEqual(boardState, expectedState.State);
    }
}