using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardState
{
    public List<Piece> State;
    public string FEN;
    public Colour Turn;

    public BoardState(Colour turn = Colour.White)
    {
        Turn = turn;
        State = new List<Piece>();

        for (int i = 0; i < 64; i++)
        {
            State.Add(new Piece(PieceType.None, Colour.White));
        }
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
    public static List<Piece> GenerateBoardState(string FEN)
    {
        List<Piece> state = new List<Piece>();

        foreach (char c in FEN)
        {
            if (char.IsDigit(c))
            {
                for (int i = 0; i < (int)char.GetNumericValue(c); i++)
                {
                    state.Add(new Piece());
                }
            }
            else if (char.IsLetter(c))
            {
                state.Add(Piece.GetPieceFromLetter(c));
            }
        }

        if (state.Count != 64)
        {
            throw new ArgumentException();
        }

        return state;
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