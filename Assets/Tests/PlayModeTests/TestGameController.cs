using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests.PlayModeTests
{
    public class TestGameController
    {
        private GameObject pieceParent = GameObject.Find("Pieces");

        [Test]
        public void TestUpdateScreenSinglePiece(char pieceName, string piecePosition)
        {
            //Board board = new Board();
            //Piece piece = new Piece(pieceName);

            //board.PlacePiece(piece, piecePosition);

            //GameController.UpdateScreen(board);

            //Assert.That(pieceParent.Children);
        }
    }
}