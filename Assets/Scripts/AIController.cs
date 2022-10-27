using System;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public Tuple<Move, float> Minimax(int depth, bool maximizingPlayer)
    {
        // Get the FEN string for the current position
        string currentPosition = Board.GetCurrentPosition();

        float maxValue;
        Move bestMove = new Move();

        if (GameController.IsInCheckmate(!maximizingPlayer))
        {
            return Tuple.Create(bestMove, Mathf.Infinity);
        }
        else if (GameController.IsInCheckmate(maximizingPlayer))
        {
            return Tuple.Create(bestMove, -Mathf.Infinity);
        }
        else if (depth == 0)
        {
            return Tuple.Create(bestMove, EvaluatePosition(currentPosition, maximizingPlayer));
        }

        if (maximizingPlayer)
        {
            maxValue = -Mathf.Infinity;
            List<Move> legalMoves = GameController.GetLegalMoves(maximizingPlayer);

            foreach (Move move in legalMoves)
            {
                move.MakeMove();

                (Move newMove, float newValue) = Minimax(depth - 1, false);

                if (newValue > maxValue)
                {
                    maxValue = newValue;
                    bestMove = newMove;
                }
            }

            return Tuple.Create(bestMove, maxValue);
        }
        else
        {
            maxValue = Mathf.Infinity;
            List<Move> legalMoves = GameController.GetLegalMoves(maximizingPlayer);

            foreach (Move move in legalMoves)
            {
                move.MakeMove();

                (Move newMove, float newValue) = Minimax(depth - 1, true);

                if (newValue < maxValue)
                {
                    maxValue = newValue;
                    bestMove = newMove;
                }
            }

            return Tuple.Create(bestMove, maxValue);
        }
    }

    private float EvaluatePosition(string currentPosition, bool player)
    {
        float positionValue = 0f;

        foreach (char letter in currentPosition)
        {
            if ((char.IsUpper(letter) && player) || (char.IsLower(letter) && !player))
                positionValue += Board.GetPieceValue(letter);
            else
                positionValue -= Board.GetPieceValue(letter);
        }

        return positionValue;
    }
}