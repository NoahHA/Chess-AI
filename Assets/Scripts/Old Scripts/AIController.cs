using System;
using System.Collections.Generic;
using UnityEngine;

public static class AIController
{
    public static Tuple<Move, float> Minimax(int depth, float alpha = -Mathf.Infinity, float beta = Mathf.Infinity, bool maximizingPlayer = true)
    {
        // Get the FEN string for the current position
        string currentPosition = Board.GetCurrentPosition();

        float maxValue;
        Move bestMove = new Move();

        // If white is in checkmate
        if (GameController.IsInCheckmate(true))
        {
            return Tuple.Create(bestMove, Mathf.Infinity);
        }

        // If black is in checkmate
        else if (GameController.IsInCheckmate(false))
        {
            return Tuple.Create(bestMove, -Mathf.Infinity);
        }

        // If max depth is reached
        else if (depth == 0)
        {
            return Tuple.Create(bestMove, EvaluatePosition(currentPosition));
        }

        if (maximizingPlayer)
        {
            maxValue = -Mathf.Infinity;
            List<Move> legalMoves = GameController.GetLegalMoves(false);

            foreach (Move move in legalMoves)
            {
                move.MakeMove();
                (Move _, float newValue) = Minimax(depth - 1, alpha, beta, false);
                move.UnmakeMove();

                if (newValue > maxValue)
                {
                    maxValue = newValue;
                    bestMove = move;
                }

                alpha = Mathf.Max(alpha, maxValue);
                if (maxValue >= beta)
                {
                    break;
                }
            }
            return Tuple.Create(bestMove, maxValue);
        }
        else
        {
            maxValue = Mathf.Infinity;
            List<Move> legalMoves = GameController.GetLegalMoves(true);

            foreach (Move move in legalMoves)
            {
                move.MakeMove();
                (Move _, float newValue) = Minimax(depth - 1, alpha, beta, true);
                move.UnmakeMove();

                if (newValue < maxValue)
                {
                    maxValue = newValue;
                    bestMove = move;
                }

                beta = Mathf.Min(beta, maxValue);
                if (maxValue <= alpha)
                {
                    break;
                }

            }
            return Tuple.Create(bestMove, maxValue);
        }
    }

    /// <summary>
    /// Returns a numerical evaluation of a given chess position, 
    /// where more positive numbers indicate black is winning and 
    /// more negative numbers indicate white
    /// </summary>
    /// <param name="currentPosition"></param>
    private static float EvaluatePosition(string currentPosition)
    {
        float positionValue = 0f;
        List<Move> blackLegalMoves = GameController.GetLegalMoves(false);
        List<Move> whiteLegalMoves = GameController.GetLegalMoves(true);

        // Evaluates based on total piece value for each side
        foreach (char letter in currentPosition)
        {
            // White piece = negative (because computer is always black)
            if (char.IsUpper(letter))
                positionValue -= Board.GetPieceValue(letter);
            // Black piece = positive
            else
                positionValue += Board.GetPieceValue(letter);
        }

        // Adds penality for lack of mobility
        positionValue += 0.1f * (blackLegalMoves.Count - whiteLegalMoves.Count);

        return positionValue;
    }
}