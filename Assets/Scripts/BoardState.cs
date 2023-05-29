using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

/// <summary>
/// Defines the state of a board, including the positions of every piece and who's turn it is.
/// </summary>
public class BoardState
{
    private Piece[] _state = new Piece[64];

    [Tooltip("A 64 element array containing the piece present on every square of the board.")]
    public Piece[] State
    {
        get => _state;
        set
        {
            _state = value;
            GenerateFenFromPieces();
        }
    }

    private string _fen;

    [Tooltip("A FEN string representing the current state of the board.")]
    public string FEN
    {
        get => _fen;
        set
        {
            _fen = value;
            GeneratePiecesFromFen();
        }
    }

    [Tooltip("An enum representing who's turn it is currently.")]
    public PieceColour Turn;

    public BoardState(string fen = "8/8/8/8/8/8/8/8")
    {
        (Turn, FEN) = (PieceColour.None, fen);
        GeneratePiecesFromFen();
    }

    public BoardState(PieceColour turn, Piece[] state)
    {
        (Turn, State) = (turn, state);
        GenerateFenFromPieces();
    }

    private void GenerateFenFromPieces()
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
            if (State[i].Type == PieceType.None)
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

                tempFen += State[i].Letter;
            }

            // End of board
            if (i == 63 && counter != 0)
            {
                tempFen += (counter++).ToString();
            }
        }

        FEN = tempFen;
    }

    public BoardState(Piece[] state)
    {
        (Turn, State) = (PieceColour.None, state);
        GenerateFenFromPieces();
    }

    public BoardState(PieceColour turn, string fen = "8/8/8/8/8/8/8/8")
    {
        Turn = turn;
        FEN = fen;
        GeneratePiecesFromFen();
    }

    public static bool operator ==(BoardState obj1, BoardState obj2)
    {
        return obj1.Turn == obj2.Turn
                    && obj1.FEN == obj2.FEN;
    }

    public static bool operator !=(BoardState obj1, BoardState obj2)
    {
        return !(obj1.Turn == obj2.Turn
                    && obj1.FEN == obj2.FEN);
    }

    /// <summary>
    /// Converts a chess FEN string to a board state.
    /// </summary>
    /// <param name="FEN">FEN string</param>
    private void GeneratePiecesFromFen()
    {
        int counter = 0;

        foreach (char c in FEN)
        {
            if (char.IsDigit(c))
            {
                counter += (int)char.GetNumericValue(c);
            }
            else if (char.IsLetter(c))
            {
                State[counter] = new Piece(c);
                counter++;
            }
        }

        if (counter != 64)
        {
            throw new ArgumentException($"FEN string is incorrect length: should be 64 but was {counter}", nameof(FEN));
        }
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
            BoardState boardState = (BoardState)obj;
            return Turn == boardState.Turn
                    && FEN == boardState.FEN;
        }
    }

    public override int GetHashCode()
    {
        return FEN.GetHashCode() ^ Turn.GetHashCode();
    }
}