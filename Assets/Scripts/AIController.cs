using System;
using System.Collections.Generic;
using UnityEngine;

public class AIController
{
    public Tuple<Move, float> Minimax(int depth, bool maximizingPlayer = true)
    {
        // Get the FEN string for the current position
        string currentPosition = Board.GetCurrentPosition();

        float maxValue;
        Move bestMove = new Move();

        // if white is in checkmate
        if (GameController.IsInCheckmate(true))
        {
            return Tuple.Create(bestMove, Mathf.Infinity);
        }
        // if black is in checkmate
        else if (GameController.IsInCheckmate(false))
        {
            return Tuple.Create(bestMove, -Mathf.Infinity);
        }
        else if (depth == 0)
        {
            return Tuple.Create(bestMove, EvaluatePosition(currentPosition, !maximizingPlayer));
        }

        if (maximizingPlayer)
        {
            maxValue = -Mathf.Infinity;
            List<Move> legalMoves = GameController.GetLegalMoves(false);

            foreach (Move move in legalMoves)
            {
                move.MakeMove();

                (Move _, float newValue) = Minimax(depth - 1, false);

                if (newValue > maxValue)
                {
                    maxValue = newValue;
                    bestMove = move;
                }

                move.UnmakeMove();
            }
            //Board.GeneratePosition(Board.currentPosition);
            return Tuple.Create(bestMove, maxValue);
        }
        else
        {
            maxValue = Mathf.Infinity;
            List<Move> legalMoves = GameController.GetLegalMoves(true);

            foreach (Move move in legalMoves)
            {
                move.MakeMove();

                (Move newMove, float newValue) = Minimax(depth - 1, true);

                if (newValue < maxValue)
                {
                    maxValue = newValue;
                    bestMove = newMove;
                }

                move.UnmakeMove();
            }
            //Board.GeneratePosition(Board.currentPosition);
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