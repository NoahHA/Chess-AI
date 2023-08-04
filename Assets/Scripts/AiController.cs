using System;
using System.Collections.Generic;
using UnityEngine;

public static class AIController
{
    private static Tuple<Move, float> Minimax(Board board, int depth, float alpha = -Mathf.Infinity,
        float beta = Mathf.Infinity, PieceColour maximizingPlayer = PieceColour.White)
    {
        float maxValue;
        Move bestMove = new Move();

        // If white is in checkmate
        if (board.IsInCheckmate(PieceColour.White))
        {
            return Tuple.Create(bestMove, Mathf.Infinity);
        }

        // If black is in checkmate
        else if (board.IsInCheckmate(PieceColour.Black))
        {
            return Tuple.Create(bestMove, -Mathf.Infinity);
        }

        // If max depth is reached
        else if (depth == 0)
        {
            return Tuple.Create(bestMove, EvaluatePosition(board));
        }

        if (maximizingPlayer == PieceColour.White)
        {
            maxValue = -Mathf.Infinity;
            List<Move> legalMoves = board.FindAllLegalMoves(PieceColour.Black);

            foreach (Move move in legalMoves)
            {
                string initialFen = string.Copy(board.FEN);
                board.MakeMove(move);
                (Move _, float newValue) = Minimax(board, depth - 1, alpha, beta, PieceColour.Black);
                board.FEN = initialFen; // Undo move

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
            List<Move> legalMoves = board.FindAllLegalMoves(PieceColour.White);

            foreach (Move move in legalMoves)
            {
                string initialFen = string.Copy(board.FEN);
                board.MakeMove(move);
                (Move _, float newValue) = Minimax(board, depth - 1, alpha, beta, PieceColour.White);
                board.FEN = initialFen; // Undo move

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

    public static Move GetBestMove(Board board, int depth)
    {
        return Minimax(board, depth).Item1;
    }

    /// <summary>
    /// Returns a numerical evaluation of a given chess position,
    /// where more positive numbers indicate black is winning and
    /// more negative numbers indicate white.
    /// </summary>
    /// <param name="position"></param>
    private static float EvaluatePosition(Board position)
    {
        float positionValue = 0f;
        //List<Move> blackLegalMoves = board.FindAllLegalMoves(PieceColour.Black);
        //List<Move> whiteLegalMoves = board.FindAllLegalMoves(PieceColour.White);

        // Evaluates based on total piece value for each side
        foreach (Piece piece in position.State)
        {
            // White piece = negative (because computer is always black)
            if (piece?.Colour == PieceColour.White)
            {
                positionValue -= GetPieceValue(piece);
            }
            // Black piece = positive
            else
            {
                positionValue += GetPieceValue(piece);
            }
        }

        // Adds penality for lack of mobility
        //positionValue += 0.1f * (blackLegalMoves.Count - whiteLegalMoves.Count);

        return positionValue;
    }

    private static float GetPieceValue(Piece piece)
    {
        return piece?.Type switch
        {
            PieceType.Pawn => 1,
            PieceType.Knight => 3,
            PieceType.Bishop => 3,
            PieceType.Rook => 5,
            PieceType.Queen => 9,
            PieceType.King => 0,
            _ => 0,
        };
    }
}