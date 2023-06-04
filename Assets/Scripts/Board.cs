using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

/// <summary>
/// Defines the state of a board, including the positions of every piece and who's turn it is.
/// </summary>
public class Board
{
    [Tooltip("A 64 element array containing the piece present on every square of the board.")]
    private Piece[] _state = new Piece[64];

    [Tooltip("A FEN string representing the current state of the board.")]
    public string FEN
    {
        get => GenerateFenFromState(_state);
        set
        {
            _state = GenerateStateFromFen(value);
        }
    }

    [Tooltip("An enum representing who's turn it is currently.")]
    public PieceColour Turn;

    public Board(PieceColour turn = PieceColour.None, string fen = "8/8/8/8/8/8/8/8")
    {
        (Turn, FEN) = (PieceColour.None, fen);
    }

    public void PlacePiece(Piece piece, ChessSquare position)
    {
        int stateIndex = (position.Row - 1) * 8 + (position.Col - 1);
        _state[stateIndex] = piece;
    }

    public static string GenerateFenFromState(Piece[] state)
    {
        int counter = 0;
        string tempFen = "";

        for (int i = 0; i < 64; i++)
        {
            // End of row
            if (i % 8 == 0 && i != 0)
            {
                if (counter != 0)
                {
                    tempFen += counter.ToString();
                    counter = 0;
                }

                tempFen += '/';
            }

            // Empty square
            if (state[i].Type == PieceType.None)
            {
                counter++;
            }

            // Piece
            else
            {
                if (counter != 0)
                {
                    tempFen += counter.ToString();
                    counter = 0;
                }

                tempFen += state[i].Letter;
            }

            // End of board
            if (i == 63 && counter != 0)
            {
                tempFen += (counter++).ToString();
            }
        }

        return tempFen;
    }

    public static bool operator ==(Board obj1, Board obj2)
    {
        return obj1.Turn == obj2.Turn
                    && obj1.FEN == obj2.FEN;
    }

    public static bool operator !=(Board obj1, Board obj2)
    {
        return !(obj1.Turn == obj2.Turn
                    && obj1.FEN == obj2.FEN);
    }

    /// <summary>
    /// Converts a chess FEN string to a board state.
    /// </summary>
    /// <param name="FEN">FEN string</param>
    public static Piece[] GenerateStateFromFen(string fen)
    {
        int counter = 0;
        Piece[] state = new Piece[64];

        foreach (char c in fen)
        {
            if (char.IsDigit(c))
            {
                counter += (int)char.GetNumericValue(c);
            }
            else if (char.IsLetter(c))
            {
                state[counter] = new Piece(c);
                counter++;
            }
        }

        if (counter != 64)
        {
            throw new ArgumentException($"FEN string is incorrect length: should be 64 but was {counter}", nameof(FEN));
        }

        return state;
    }

    public void SetBoardToStartingPosition()
    {
        FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Board boardState = (Board)obj;
            return Turn == boardState.Turn
                    && FEN == boardState.FEN;
        }
    }

    public override int GetHashCode()
    {
        return FEN.GetHashCode() ^ Turn.GetHashCode();
    }

    public override string ToString()
    {
        return Turn.ToString() + " - " + FEN;
    }
}