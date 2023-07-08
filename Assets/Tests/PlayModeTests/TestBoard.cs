using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayModeTests
{
    public class TestBoard
    {
        [SetUp]
        public void SetupTest()
        {
            SceneManager.LoadScene("MainScene");
        }

        [UnityTest]
        public IEnumerator TestClearScreen()
        {
            var board = new Board();
            board.PlacePiece(new Piece('p'), new Square("e2"));
            BoardHelper.UpdateScreenFromBoard(board);
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

            BoardHelper.UpdateScreenFromBoard(board);
            yield return null;

            var expectedBoard = new Board();
            expectedBoard.UpdateBoardFromScreen();
            Assert.AreEqual(expectedBoard, board);
        }
    }
}