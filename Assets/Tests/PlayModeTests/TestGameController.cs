using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayModeTests
{
    public class TestGameController
    {
        private float waitTime = 0.1f;

        [SetUp]
        public void Setup()
        {
            GameController.ClearScreen();
        }

        [UnityTest]
        public IEnumerator TestClearScreen()
        {
            var board = new Board();
            board.PlacePiece(new Piece('p'), new ChessSquare("e2"));
            board.UpdateScreenFromBoard();
            Assert.IsNotNull(GameController.Pieces);

            GameController.ClearScreen();
            yield return new WaitForFixedUpdate();

            Assert.IsEmpty(GameController.Pieces);
        }

        [UnityTest]
        public IEnumerator TestUpdateScreenSinglePiece(
            [Values('q', 'R')] char pieceName,
            [Values("a1", "f8")] string piecePosition)
        {
            var board = new Board();
            board.PlacePiece(new Piece(pieceName), new ChessSquare(piecePosition));

            board.UpdateScreenFromBoard();
            yield return new WaitForSeconds(waitTime);

            var expectedBoard = new Board();
            expectedBoard.UpdateBoardFromScreen(GameController.Pieces);
            Assert.AreEqual(expectedBoard, board);
        }
    }
}