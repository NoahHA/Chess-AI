using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        set => _state = GenerateStateFromFen(value);
    }

    [Tooltip("An enum representing who's turn it is currently.")]
    public PieceColour Turn;

    public Board(PieceColour turn = PieceColour.White, string fen = "8/8/8/8/8/8/8/8")
    {
        (Turn, FEN) = (turn, fen);
    }

    public void PlacePiece(Piece piece, Square position)
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
            if (state[i] == null)
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
        return (obj1.Turn, obj1.FEN) == (obj2.Turn, obj2.FEN);
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
            return (Turn, FEN) == (boardState.Turn, boardState.FEN);
        }
    }

    /// <summary>
    /// Updates the board to reflect the pieces on the screen.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void UpdateBoardFromScreen()
    {
        ClearBoard();

        foreach (GameObject pieceObject in BoardHelper.GetPieces())
        {
            Piece piece = pieceObject.GetComponent<PieceID>().Piece;
            var square = new Square(pieceObject.transform.position);
            PlacePiece(piece, square);
        }
    }

    private void ClearBoard()
    {
        _state = new Piece[64];
    }

    /// <summary>
    /// Updates the screen based on the given board state.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void UpdateScreenFromBoard()
    {
        BoardHelper.ClearScreen();

        for (int i = 0; i < 64; i++)
        {
            var position = new Square(i);
            Piece piece = FindPieceOnSquare(position);

            if (piece != null)
            {
                GameObject pieceGameObject = (GameObject)GameObject.Instantiate(
                    Resources.Load("Pieces/" + piece.GetPrefabName()), position.ScreenPosition, Quaternion.identity
                );

                pieceGameObject.transform.parent = GameObject.Find("Board/Pieces").transform;
            }
        }
    }

    public List<Move> FindLegalMoves(Square square)
    {
        Piece piece = FindPieceOnSquare(square);

        switch (piece.Type)
        {
            case PieceType.Pawn:
                return FindLegalPawnMoves(square);

            case PieceType.Knight:
                return FindLegalPawnMoves(square);

            case PieceType.Bishop:
                return FindLegalPawnMoves(square);

            case PieceType.Rook:
                return FindLegalPawnMoves(square);

            case PieceType.Queen:
                return FindLegalPawnMoves(square);

            case PieceType.King:
                return FindLegalPawnMoves(square);

            default:
                return null;
        }
    }

    /// <summary>
    /// Finds the piece occupying a square, or null if no piece is there.
    /// </summary>
    /// <param name="square">The square to check.</param>
    /// <returns></returns>
    public Piece FindPieceOnSquare(Square square)
    {
        return _state[square.Index];
    }

    public bool IsEnemyPiece(Piece piece)
    {
        return piece.Colour == Turn ? false : true;
    }

    private List<Move> FindLegalPawnMoves(Square square)
    {
        List<Move> moves = new();

        // Flag to indicate something in the way of the pawn
        bool isBlocked = false;

        // White pieces have +1 direction, black have -1
        int direction = (Turn == PieceColour.White) ? 1 : -1;

        // Max number of forward moves the pawn can take
        int maxMoves = ((square.Row == 2 && direction == 1) || (square.Row == 7 && direction == -1)) ? 2 : 1;

        // Checks for forward moves
        for (int i = 1; i < maxMoves + 1; i++)
        {
            var newSquare = new Square(square.Col, square.Row + i * direction);

            // Add square in front of pawn to moves if not blocked
            if (FindPieceOnSquare(newSquare) == null && !isBlocked)
            {
                moves.Add(new Move(square, newSquare));
            }
            else
            {
                isBlocked = true;
            }
        }

        // Checks for diagonal moves
        for (int j = -1; j < 2; j += 2)
        {
            var newSquare = new Square(square.Col + j, square.Row + direction);

            // Add diagonal square to moves if opponent's piece can be taken there
            Piece diagonalPiece = FindPieceOnSquare(newSquare);

            if (diagonalPiece != null && (diagonalPiece.Colour != FindPieceOnSquare(square).Colour))
            {
                moves.Add(new Move(square, newSquare));
            }
        }

        return moves;
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