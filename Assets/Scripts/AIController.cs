using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public float Minimax(int depth, bool maximizingPlayer, bool player)
    {
        // Get the FEN string for the current position
        string currentPosition = Board.GetCurrentPosition();

        if (GameController.IsInCheckmate(!player))
        {
            return Mathf.Infinity;
        }
        else if (GameController.IsInCheckmate(player))
        {
            return -Mathf.Infinity;
        }
        else if (depth == 0)
        {
            return EvaluatePosition(currentPosition, maximizingPlayer);
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