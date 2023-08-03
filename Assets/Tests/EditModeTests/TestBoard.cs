using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests.EditModeTests
{
    public class TestBoard
    {
        [Test]
        public void TestGenerateEmptyBoard()
        {
            string Fen = "8/8/8/8/8/8/8/8 w KQkq -";
            Assert.AreEqual(new Board(), new Board(fen: Fen));
        }

        [Test]
        public void TestGenerateSimplePosition()
        {
            var board = new Board("8/8/7P/8/8/8/p7/8 w KQkq -");

            var expectedBoard = new Board();
            expectedBoard.PlacePiece(new Piece('p'), new Square("a2"));
            expectedBoard.PlacePiece(new Piece('P'), new Square("h6"));

            Assert.AreEqual(expectedBoard, board);
        }

        [Test]
        public void TestSetBoardToStartingPosition()
        {
            string Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -";
            var board = new Board(fen: Fen);

            var expectedBoard = new Board();
            expectedBoard.SetBoardToStartingPosition();
            Assert.AreEqual(expectedBoard, board);
        }

        [Test]
        public void TestGenerateBoardFenTooLong()
        {
            string Fen = "8/8/8/8/9/8/8/8 w KQkq -";

            Assert.Throws<ArgumentException>(() => new Board(fen: Fen));
        }

        [Test]
        public void TestGenerateBoardInvalidFen()
        {
            string Fen = "8/7T/8/8/8/8/8/8 w KQkq -";

            Assert.Throws<ArgumentException>(() => new Board(fen: Fen));
        }

        [Test]
        public void TestBoardValueEquality()
        {
            string whiteFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -";
            string blackFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq -";

            var boardState1 = new Board(blackFen);
            var boardState2 = new Board(blackFen);
            var boardState3 = new Board(whiteFen);

            Assert.AreEqual(boardState1, boardState2);
            Assert.AreNotEqual(boardState1, boardState3);
        }

        [Test]
        [TestCase(PieceColour.White, "a2", "a3")]
        [TestCase(PieceColour.White, "a2", "a4")]
        [TestCase(PieceColour.White, "h2", "h3")]
        [TestCase(PieceColour.White, "h2", "h4")]
        [TestCase(PieceColour.White, "e2", "e3")]
        [TestCase(PieceColour.Black, "a7", "a6")]
        [TestCase(PieceColour.Black, "a7", "a5")]
        [TestCase(PieceColour.Black, "h7", "h6")]
        [TestCase(PieceColour.Black, "h7", "h5")]
        [TestCase(PieceColour.Black, "e7", "e6")]
        public void TestMakeValidPawnMove(PieceColour colour, string startSquare, string endSquare)
        {
            var board = new Board();
            board.SetBoardToStartingPosition();
            board.Turn = colour;
            board.MakeMove(new Move(startSquare, endSquare));

            Assert.IsNull(board.FindPieceOnSquare(new Square(startSquare)));
            Assert.AreEqual(board.FindPieceOnSquare(new Square(endSquare)), new Piece(PieceType.Pawn, colour));
        }

        [Test]
        [TestCase(PieceColour.White, "a2", "a5")] // Too many spaces
        [TestCase(PieceColour.White, "b2", "c3")] // Diagonal with nothing to take
        [TestCase(PieceColour.White, "h7", "h6")] // Wrong colour
        [TestCase(PieceColour.Black, "d7", "d8")] // Backwards move
        public void TestMakeInvalidPawnMove(PieceColour colour, string startSquare, string endSquare)
        {
            var board = new Board();
            board.SetBoardToStartingPosition();
            board.Turn = colour;
            Assert.IsFalse(board.IsLegalMove(new Move(startSquare, endSquare)));
        }

        [Test]
        [TestCase(PieceType.Knight, "a1", "b3")] // →↑↑
        [TestCase(PieceType.Knight, "f3", "e5")] // ←↑↑
        [TestCase(PieceType.Knight, "d4", "c2")] // ←↓↓
        [TestCase(PieceType.Knight, "g3", "h1")] // →↓↓
        [TestCase(PieceType.Knight, "a1", "c2")] // →→↑
        [TestCase(PieceType.Knight, "e5", "g4")] // →→↓
        [TestCase(PieceType.Knight, "e1", "c2")] // ←←↑
        [TestCase(PieceType.Knight, "e5", "c4")] // ←←↓
        [TestCase(PieceType.Bishop, "a1", "h8")] // ↗
        [TestCase(PieceType.Bishop, "e4", "g2")] // ↘
        [TestCase(PieceType.Bishop, "h6", "c1")] // ↙
        [TestCase(PieceType.Bishop, "d1", "b3")] // ↖
        [TestCase(PieceType.Rook, "a1", "h1")] // →
        [TestCase(PieceType.Rook, "b3", "b8")] // ↑
        [TestCase(PieceType.Rook, "g6", "d6")] // ←
        [TestCase(PieceType.Rook, "c5", "c2")] // ↓
        [TestCase(PieceType.Queen, "a1", "h1")] // →
        [TestCase(PieceType.Queen, "b3", "b8")] // ↑
        [TestCase(PieceType.Queen, "g6", "d6")] // ←
        [TestCase(PieceType.Queen, "c5", "c2")] // ↓
        [TestCase(PieceType.Queen, "a1", "h8")] // ↗
        [TestCase(PieceType.Queen, "e4", "g2")] // ↘
        [TestCase(PieceType.Queen, "h6", "c1")] // ↙
        [TestCase(PieceType.Queen, "d1", "b3")] // ↖
        [TestCase(PieceType.King, "a1", "b1")] // →
        [TestCase(PieceType.King, "b3", "b4")] // ↑
        [TestCase(PieceType.King, "g6", "f6")] // ←
        [TestCase(PieceType.King, "c5", "c4")] // ↓
        [TestCase(PieceType.King, "a1", "b2")] // ↗
        [TestCase(PieceType.King, "e4", "f3")] // ↘
        [TestCase(PieceType.King, "d2", "c1")] // ↙
        [TestCase(PieceType.King, "d1", "c2")] // ↖
        public void TestMakeValidNonPawnMove(PieceType type, string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(type, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            board.MakeMove(new Move(startSquare, endSquare));

            Assert.IsNull(board.FindPieceOnSquare(new Square(startSquare)));
            Assert.AreEqual(piece, board.FindPieceOnSquare(new Square(endSquare)));
        }

        [Test]
        [TestCase(PieceType.Knight, "a1", "b4")] // →↑↑↑
        [TestCase(PieceType.Knight, "f3", "e4")] // ←↑
        [TestCase(PieceType.Knight, "d4", "c1")] // ←↓↓↓
        [TestCase(PieceType.Knight, "g3", "h2")] // →↓
        [TestCase(PieceType.Bishop, "a2", "h8")]
        [TestCase(PieceType.Bishop, "e4", "h2")]
        [TestCase(PieceType.Rook, "a1", "b2")]
        [TestCase(PieceType.Rook, "e7", "g3")]
        [TestCase(PieceType.Queen, "e7", "g3")]
        [TestCase(PieceType.Queen, "a2", "h8")]
        [TestCase(PieceType.Queen, "e4", "h2")]
        [TestCase(PieceType.King, "e7", "g3")]
        [TestCase(PieceType.King, "a2", "h8")]
        [TestCase(PieceType.King, "e4", "h2")]
        public void TestMakeInvalidNonPawnMove(PieceType type, string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(type, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            Assert.IsFalse(board.IsLegalMove(new Move(startSquare, endSquare)));
        }

        [TestCase("d1", "d7", true)]
        [TestCase("d1", "d6", false)]
        [TestCase("f1", "f7", true)]
        [TestCase("f1", "c2", false)]
        public void TestIsInCheck(string startSquare, string endSquare, bool IsCheck)
        {
            var board = new Board();
            board.SetBoardToStartingPosition();
            board.MakeMove(new Move(startSquare, endSquare));
            board.ChangeTurn();
            Assert.AreEqual(IsCheck, board.IsInCheck());
        }
    }
}