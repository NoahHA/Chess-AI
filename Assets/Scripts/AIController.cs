using System;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public float Minimax(int depth, bool maximizingPlayer, bool player)
    {
        if (GameController.IsInCheckmate(!player))
        {
            return 1f;
        }
        else if (GameController.IsInCheckmate(player))
        {
            return -1f;
        }
        else if (depth == 0)
        {
            return EvaluatePosition();
        }

        if (maximizingPlayer)
        {
            float value = -Mathf.Infinity;
            Dictionary<GameObject, List<ChessSquare>> legalMoves = GameController.GetLegalMoves(maximizingPlayer);

            foreach (GameObject piece in legalMoves.Keys)
            {
                foreach (ChessSquare move in legalMoves[piece])
                {
                    piece.transform.position = move.Location; // Move the piece
                    Board.TakePiece(piece); // Take any takeable pieces

                    value = Mathf.Max(value, Minimax(depth - 1, false, player));
                }
            }

            return value;
        }
        else
        {
            float value = Mathf.Infinity;
            Dictionary<GameObject, List<ChessSquare>> legalMoves = GameController.GetLegalMoves(maximizingPlayer);

            foreach (GameObject piece in legalMoves.Keys)
            {
                foreach (ChessSquare move in legalMoves[piece])
                {
                    piece.transform.position = move.Location; // Move the piece
                    Board.TakePiece(piece); // Take any takeable pieces

                    value = Mathf.Max(value, Minimax(depth - 1, true, player));
                }
            }

            return value;
        }
    }

    private float EvaluatePosition()
    {
        throw new NotImplementedException();
    }
}