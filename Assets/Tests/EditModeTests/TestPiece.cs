using NUnit.Framework;
using System;

namespace Tests.EditModeTests
{
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
            Assert.AreEqual(new Piece(PieceType.Pawn, PieceColour.Black), piece);
            Assert.AreNotEqual(new Piece(PieceType.Queen, PieceColour.White), piece);
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
}