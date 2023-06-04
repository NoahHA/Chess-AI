using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.EditModeTests
{
    public class TestChessSquare
    {
        [Test]
        [TestCase(3, 5, "c5")]
        [TestCase(1, 7, "a7")]
        [TestCase(4, 2, "d2")]
        [TestCase(1, 1, "a1")]
        [TestCase(8, 8, "h8")]
        public void TestCreateValidChessSquareFromRowCol(int col, int row, string expectedSquareName)
        {
            // Act
            var square = new ChessSquare(col, row);

            // Assert
            Assert.AreEqual(expectedSquareName, square.SquareName);
        }

        [Test]
        [TestCase(-1, -1)]
        [TestCase(-1, 5)]
        [TestCase(1, -5)]
        [TestCase(0, 0)]
        [TestCase(3, 0)]
        [TestCase(0, 3)]
        [TestCase(9, 2)]
        [TestCase(2, 9)]
        [TestCase(15, 12)]
        [TestCase(15, -15)]
        public void TestCreateInvalidChessSquareFromRowCol(int col, int row)
        {
            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new ChessSquare(col, row));
        }

        [Test]
        [TestCase("c5", 3, 5)]
        [TestCase("a7", 1, 7)]
        [TestCase("d2", 4, 2)]
        [TestCase("a1", 1, 1)]
        [TestCase("h8", 8, 8)]
        public void TestCreateValidChessSquareFromString(string chessSquareName, int expectedCol, int expectedRow)
        {
            // Act
            var square = new ChessSquare(chessSquareName);

            // Assert
            Assert.AreEqual(expectedRow, square.Row);
            Assert.AreEqual(expectedCol, square.Col);
        }
    }
}