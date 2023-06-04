using NUnit.Framework;
using System;

namespace Tests.EditModeTests
{
    public class TestBoard
    {
        [Test]
        public void TestGenerateBoardEmptyBoard()
        {
            // Arrange
            string Fen = "8/8/8/8/8/8/8/8";

            // Act
            var boardState = new Board(Fen);

            // Assert
            Assert.AreEqual(new Board().State, boardState.State);
        }

        [Test]
        public void TestGenerateBoardSimplePosition()
        {
            // Arrange
            string Fen = "8/p7/8/8/8/7P/8/8";

            // Act
            var boardState = new Board(Fen);

            // Assert
            var expectedState = new Board();
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
            var boardState = new Board(Fen);

            // Assert
            var expectedState = new Board();
            expectedState.SetBoardToStartingPosition();
            Assert.AreEqual(expectedState.State, boardState.State);
        }

        [Test]
        public void TestGenerateBoardFenTooLong()
        {
            // Arrange
            string Fen = "8/8/8/8/9/8/8/8";

            // Assert
            Assert.Throws<ArgumentException>(() => new Board(Fen));
        }

        [Test]
        public void TestGenerateBoardInvalidFen()
        {
            // Arrange
            string Fen = "8/7T/8/8/8/8/8/8";

            // Assert
            Assert.Throws<ArgumentException>(() => new Board(Fen));
        }

        [Test]
        public void TestBoardValueEquality()
        {
            // Arrange
            string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

            // Act
            var boardState1 = new Board(PieceColour.Black, Fen);
            var boardState2 = new Board(PieceColour.Black, Fen);
            var boardState3 = new Board(PieceColour.White, Fen);

            // Assert
            Assert.AreEqual(boardState1, boardState2);
            Assert.AreNotEqual(boardState1, boardState3);
        }

        [Test]
        public void TestGenerateFenFromPiecesSimple()
        {
            // Arrange
            Piece[] pieces = new Piece[64];
            pieces[3] = new Piece(PieceType.Queen, PieceColour.Black);

            // Act
            var boardState = new Board(pieces);

            // Assert
            string expectedFen = "3Q4/8/8/8/8/8/8/8";
            Assert.AreEqual(expectedFen, boardState.FEN);
        }

        [Test]
        public void TestGenerateFenFromPiecesStartingPosition()
        {
            // Arrange
            var tempBoard = new Board();
            tempBoard.SetBoardToStartingPosition();
            Piece[] pieces = tempBoard.State;

            // Act
            var boardState = new Board(pieces);

            // Assert
            string expectedFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            Assert.AreEqual(expectedFen, boardState.FEN);
        }
    }
}