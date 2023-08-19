using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public static class AIController
{
    private static Stopwatch _stopWatch;
    private static float _maxTime_ms = Mathf.Infinity;

    // Benchmarking variables
    public static Benchmarking benchmarking = new Benchmarking();

    private static int _nodesSearched = 0;

    private static Tuple<Move, float> Minimax(
        Board board,
        int depth,
        float alpha = -Mathf.Infinity,
        float beta = Mathf.Infinity,
        bool maximizingPlayer = true,
        PieceColour computerSide = PieceColour.Black
        )
    {
        float maxValue;
        Move bestMove = new();

        if (board.IsInCheckmate(computerSide) || board.IsInStalemate(computerSide))
        {
            _nodesSearched++;
            return Tuple.Create(bestMove, Mathf.Infinity);
        }
        else if (board.IsInCheckmate(computerSide.ChangeTurn()) || board.IsInStalemate(computerSide.ChangeTurn()))
        {
            _nodesSearched++;
            return Tuple.Create(bestMove, -Mathf.Infinity);
        }

        // If max depth or time limit is reached
        else if (depth == 0 || _stopWatch.ElapsedMilliseconds > _maxTime_ms)
        {
            _nodesSearched++;
            return Tuple.Create(bestMove, EvaluatePosition(board, computerSide));
        }

        if (maximizingPlayer)
        {
            maxValue = -Mathf.Infinity;
            List<Move> legalMoves = board.FindAllLegalMoves(computerSide);

            foreach (Move move in legalMoves)
            {
                string initialFen = string.Copy(board.FEN);
                board.MakeMove(move);
                (Move _, float newValue) = Minimax(board, depth - 1, alpha, beta, false, computerSide);
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
            List<Move> legalMoves = board.FindAllLegalMoves(computerSide.ChangeTurn());

            foreach (Move move in legalMoves)
            {
                string initialFen = string.Copy(board.FEN);
                board.MakeMove(move);
                (Move _, float newValue) = Minimax(board, depth - 1, alpha, beta, true, computerSide);
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

    public static Move GetBestMove(Board board, int depth, PieceColour computerSide = PieceColour.Black)
    {
        _nodesSearched = 0;
        return Minimax(board, depth, computerSide: computerSide).Item1;
    }

    public static Move GetBestMove(Board board, float maxTime_ms, PieceColour computerSide = PieceColour.Black, bool benchmarkMode = false)
    {
        _nodesSearched = 0;
        _stopWatch = Stopwatch.StartNew();
        _maxTime_ms = maxTime_ms;

        Move bestMove = new();
        float maxValue = -Mathf.Infinity;
        int depth = 0;

        while (_stopWatch.ElapsedMilliseconds < _maxTime_ms)
        {
            depth++;
            (Move newBestMove, float newValue) = Minimax(board, depth, computerSide: computerSide);
            bestMove = (newValue > maxValue) ? newBestMove : bestMove;
        }

        if (benchmarkMode)
        {
            benchmarking.RecordMetrics(_nodesSearched, depth, maxTime_ms);
        }

        return bestMove;
    }

    /// <summary>
    /// Returns a numerical evaluation of a given chess position,
    /// where more negative numbers indicate black is winning and
    /// more positive numbers indicate white.
    /// </summary>
    /// <param name="position"></param>
    private static float EvaluatePosition(Board position, PieceColour computerSide)
    {
        float positionValue = 0f;
        //List<Move> computerLegalMoves = board.FindAllLegalMoves(computerSide);
        //List<Move> otherSideLegalMoves = board.FindAllLegalMoves(computerSide.ChangeTurn());

        // Evaluates based on total piece value for each side
        foreach (Piece piece in position.State)
        {
            // Computer piece = positive
            if (piece?.Colour == computerSide)
            {
                positionValue += GetPieceValue(piece);
            }
            // Opposite side piece = negative
            else
            {
                positionValue -= GetPieceValue(piece);
            }
        }

        // Adds penality for lack of mobility
        //positionValue -= 0.1f * (computerLegalMoves.Count - otherSideLegalMoves.Count);

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