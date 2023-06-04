using NUnit.Framework;
using System;

namespace Tests.EditModeTests
{
    public class TestBoard
    {
        [Test]
        public void TestGenerateEmptyBoard()
        {
            string Fen = "8/8/8/8/8/8/8/8";
            Assert.AreEqual(new Board(), new Board(fen: Fen));
        }

        [Test]
        public void TestGenerateSimplePosition()
        {
            string Fen = "8/p7/8/8/8/7P/8/8";

            var board = new Board(fen: Fen);

            var expectedBoard = new Board();
            expectedBoard.PlacePiece(new Piece('p'), new ChessSquare("a2"));
            expectedBoard.PlacePiece(new Piece('P'), new ChessSquare("h6"));

            Assert.AreEqual(expectedBoard, board);
        }

        [Test]
        public void TestSetBoardToStartingPosition()
        {
            string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            var board = new Board(fen: Fen);

            var expectedBoard = new Board();
            expectedBoard.SetBoardToStartingPosition();
            Assert.AreEqual(expectedBoard, board);
        }

        [Test]
        public void TestGenerateBoardFenTooLong()
        {
            string Fen = "8/8/8/8/9/8/8/8";

            Assert.Throws<ArgumentException>(() => new Board(fen: Fen));
        }

        [Test]
        public void TestGenerateBoardInvalidFen()
        {
            string Fen = "8/7T/8/8/8/8/8/8";

            Assert.Throws<ArgumentException>(() => new Board(fen: Fen));
        }

        [Test]
        public void TestBoardValueEquality()
        {
            string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

            var boardState1 = new Board(PieceColour.Black, Fen);
            var boardState2 = new Board(PieceColour.Black, Fen);
            var boardState3 = new Board(PieceColour.White, Fen);

            Assert.AreEqual(boardState1, boardState2);
            Assert.AreNotEqual(boardState1, boardState3);
        }
    }
}