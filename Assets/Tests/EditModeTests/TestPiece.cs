using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests.EditModeTests
{
    public class TestPiece
    {
        [Test]
        [TestCase('p', PieceType.Pawn)]
        [TestCase('n', PieceType.Knight)]
        [TestCase('b', PieceType.Bishop)]
        [TestCase('r', PieceType.Rook)]
        [TestCase('q', PieceType.Queen)]
        [TestCase('k', PieceType.King)]
        public void TestGetBlackPieceFromLetter(char letter, PieceType expectedPieceType)
        {
            // Act
            var piece = new Piece(letter);

            // Assert
            Piece expectedPiece = new Piece(expectedPieceType, PieceColour.Black);
            Assert.AreEqual(expectedPiece, piece);
        }

        [Test]
        [TestCase('P', PieceType.Pawn)]
        [TestCase('N', PieceType.Knight)]
        [TestCase('B', PieceType.Bishop)]
        [TestCase('R', PieceType.Rook)]
        [TestCase('Q', PieceType.Queen)]
        [TestCase('K', PieceType.King)]
        public void TestGetWhitePieceFromLetter(char letter, PieceType expectedPieceType)
        {
            // Act
            var piece = new Piece(letter);

            // Assert
            Piece expectedPiece = new Piece(expectedPieceType, PieceColour.White);
            Assert.AreEqual(expectedPiece, piece);
        }

        [Test]
        [TestCase('L')]
        [TestCase('c')]
        [TestCase(' ')]
        [TestCase('-')]
        public void TestGetPieceFromLetterInvalidPiece(char letter)
        {
            // Assert
            Assert.Throws<KeyNotFoundException>(() => new Piece(letter));
        }
    }
}