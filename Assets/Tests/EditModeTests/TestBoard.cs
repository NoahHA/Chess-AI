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
            expectedBoard.PlacePiece(new Piece('p'), new Square("a2"));
            expectedBoard.PlacePiece(new Piece('P'), new Square("h6"));

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
            board.Turn = colour;
            board.SetBoardToStartingPosition();
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
            var board = new Board(turn: colour);
            board.SetBoardToStartingPosition();

            Assert.Throws<InvalidOperationException>(() => board.MakeMove(new Move(startSquare, endSquare)));
        }

        [Test]
        [TestCase("a1", "b3")] // →↑↑
        [TestCase("f3", "e5")] // ←↑↑
        [TestCase("d4", "c2")] // ←↓↓
        [TestCase("g3", "h1")] // →↓↓
        [TestCase("a1", "c2")] // →→↑
        [TestCase("e5", "g4")] // →→↓
        [TestCase("e1", "c2")] // ←←↑
        [TestCase("e5", "c4")] // ←←↓
        public void TestMakeValidKnightMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.Knight, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            board.MakeMove(new Move(startSquare, endSquare));

            Assert.IsNull(board.FindPieceOnSquare(new Square(startSquare)));
            Assert.AreEqual(board.FindPieceOnSquare(new Square(endSquare)), piece);
        }

        [Test]
        [TestCase("a1", "b4")] // →↑↑↑
        [TestCase("f3", "e4")] // ←↑
        [TestCase("d4", "c1")] // ←↓↓↓
        [TestCase("g3", "h2")] // →↓
        public void TestMakeInvalidKnightMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.Knight, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            Assert.Throws<InvalidOperationException>(() => board.MakeMove(new Move(startSquare, endSquare)));
        }

        [Test]
        [TestCase("a1", "h8")] // ↗
        [TestCase("e4", "g2")] // ↘
        [TestCase("h6", "c1")] // ↙
        [TestCase("d1", "b3")] // ↖
        public void TestMakeValidBishopMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.Bishop, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            board.MakeMove(new Move(startSquare, endSquare));

            Assert.IsNull(board.FindPieceOnSquare(new Square(startSquare)));
            Assert.AreEqual(board.FindPieceOnSquare(new Square(endSquare)), piece);
        }

        [Test]
        [TestCase("a2", "h8")]
        [TestCase("e4", "h2")]
        public void TestMakeInvalidBishopMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.Bishop, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            Assert.Throws<InvalidOperationException>(() => board.MakeMove(new Move(startSquare, endSquare)));
        }

        [Test]
        [TestCase("a1", "h1")] // →
        [TestCase("b3", "b8")] // ↑
        [TestCase("g6", "d6")] // ←
        [TestCase("c5", "c2")] // ↓
        public void TestMakeValidRookMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.Rook, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            board.MakeMove(new Move(startSquare, endSquare));

            Assert.IsNull(board.FindPieceOnSquare(new Square(startSquare)));
            Assert.AreEqual(board.FindPieceOnSquare(new Square(endSquare)), piece);
        }

        [Test]
        [TestCase("a1", "b2")]
        [TestCase("e7", "g3")]
        public void TestMakeInvalidRookMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.Rook, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            Assert.Throws<InvalidOperationException>(() => board.MakeMove(new Move(startSquare, endSquare)));
        }

        [Test]
        [TestCase("a1", "h1")] // →
        [TestCase("b3", "b8")] // ↑
        [TestCase("g6", "d6")] // ←
        [TestCase("c5", "c2")] // ↓
        [TestCase("a1", "h8")] // ↗
        [TestCase("e4", "g2")] // ↘
        [TestCase("h6", "c1")] // ↙
        [TestCase("d1", "b3")] // ↖
        public void TestMakeValidQueenMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.Queen, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            board.MakeMove(new Move(startSquare, endSquare));

            Assert.IsNull(board.FindPieceOnSquare(new Square(startSquare)));
            Assert.AreEqual(board.FindPieceOnSquare(new Square(endSquare)), piece);
        }

        [Test]
        [TestCase("e7", "g3")]
        [TestCase("a2", "h8")]
        [TestCase("e4", "h2")]
        public void TestMakeInvalidQueenMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.Queen, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            Assert.Throws<InvalidOperationException>(() => board.MakeMove(new Move(startSquare, endSquare)));
        }

        [Test]
        [TestCase("a1", "b1")] // →
        [TestCase("b3", "b4")] // ↑
        [TestCase("g6", "f6")] // ←
        [TestCase("c5", "c4")] // ↓
        [TestCase("a1", "b2")] // ↗
        [TestCase("e4", "f3")] // ↘
        [TestCase("d2", "c1")] // ↙
        [TestCase("d1", "c2")] // ↖
        public void TestMakeValidKingMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.King, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            board.MakeMove(new Move(startSquare, endSquare));

            Assert.IsNull(board.FindPieceOnSquare(new Square(startSquare)));
            Assert.AreEqual(board.FindPieceOnSquare(new Square(endSquare)), piece);
        }

        [Test]
        [TestCase("e7", "g3")]
        [TestCase("a2", "h8")]
        [TestCase("e4", "h2")]
        public void TestMakeInvalidKingMove(string startSquare, string endSquare)
        {
            var board = new Board();
            var piece = new Piece(PieceType.King, board.Turn);

            board.PlacePiece(piece, new Square(startSquare));
            Assert.Throws<InvalidOperationException>(() => board.MakeMove(new Move(startSquare, endSquare)));
        }
    }
}