using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPiece
{
    [Test]
    public void TestGetPieceFromLetterWhitePiece()
    {
        // Arrange
        char letter = 'n';

        // Act
        var piece = new Piece(letter);

        // Assert
        Assert.AreEqual(new Piece(PieceType.Knight, PieceColour.White), piece);
    }

    [Test]
    public void TestGetPieceFromLetterBlackPiece()
    {
        // Arrange
        char letter = 'P';

        // Act
        var piece = new Piece(letter);

        // Assert
        Assert.AreEqual(new Piece(PieceType.Pawn, PieceColour.Black), piece);
    }

    [Test]
    public void TestGetPieceFromLetterInvalidPiece()
    {
        // Arrange
        char letter = 'L';

        // Assert
        Assert.Throws<ArgumentException>(() => new Piece(letter));
    }
}