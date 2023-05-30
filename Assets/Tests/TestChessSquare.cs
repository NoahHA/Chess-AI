using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestChessSquare
{
    [Test]
    public void TestCreateValidChessSquareFromRowCol()
    {
        // Act
        var square = new ChessSquare(3, 5);

        // Assert
        Assert.AreEqual("c5", square.Square);
    }

    [Test]
    public void TestCreateInvalidChessSquareFromRowCol()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChessSquare(-3, 5));
    }

    [Test]
    public void TestCreateValidChessSquareFromString()
    {
        // Act
        var square = new ChessSquare("e2");

        // Assert
        Assert.AreEqual(2, square.Row);
        Assert.AreEqual(5, square.Col);
    }
}