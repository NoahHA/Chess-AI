using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestLegalMoves
{
    [Test]
    public void onlyPawnsCanMoveAtGameStart()
    {
        //ARRANGE
        Board.GeneratePosition(Board.startingPosition);
        ChessSquare position = new ChessSquare(1, 1);
        GameObject pawn = Board.GetPieceFromLetter('p');

        //ACT
        List<ChessSquare> moves = GameController.GetLegalMoves(position, pawn);

        //ASSERT
        int expectedNumMoves = 2;
        Assert.AreEqual(expectedNumMoves, moves.Count);
    }
}
