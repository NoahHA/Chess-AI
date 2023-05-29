using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

public class BoardState
{
    public Piece[] State;

    private string _fen;

    public string FEN
    {
        get => _fen;
        set
        {
            _fen = value;
            State = GenerateBoardState(_fen);
        }
    }

    public PieceColour Turn;

    public BoardState(PieceColour turn = PieceColour.None,
        string fen = "8/8/8/8/8/8/8/8")
    {
        Turn = turn;
        FEN = fen;
        State = GenerateBoardState(FEN);
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
    public static Piece[] GenerateBoardState(string FEN)
    {
        Piece[] state = new Piece[64];
        int counter = 0;

        foreach (char c in FEN)
        {
            if (char.IsDigit(c))
            {
                counter += (int)char.GetNumericValue(c);
            }
            else if (char.IsLetter(c))
            {
                state[counter] = Piece.GetPieceFromLetter(c);
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