using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayModeTests
{
    public class TestBoard
    {
        [UnityTest]
        public IEnumerator TestClearScreen()
        {
            var board = new Board();
            board.PlacePiece(new Piece('p'), new Square("e2"));
            board.UpdateScreenFromBoard();
            Assert.IsNotEmpty(BoardHelper.GetPieces());

            BoardHelper.ClearScreen();
            yield return null;

            Assert.IsEmpty(BoardHelper.GetPieces());
        }

        [UnityTest]
        public IEnumerator TestUpdateScreenSinglePiece(
            [Values('q', 'R')] char pieceName,
            [Values("a1", "f8")] string piecePosition)
        {
            BoardHelper.ClearScreen();
            var board = new Board();
            board.PlacePiece(new Piece(pieceName), new Square(piecePosition));

            board.UpdateScreenFromBoard();
            yield return null;

            var expectedBoard = new Board();
            expectedBoard.UpdateBoardFromScreen(BoardHelper.GetPieces());
            Assert.AreEqual(expectedBoard, board);
        }
    }
}