using System;
using System.Collections.Generic;
using System.Linq;
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
        _state[position.Index] = piece;
    }

    /// <summary>
    /// Checks if a given move is legal.
    /// </summary>
    /// <param name="move"></param>
    /// <returns>Whether the move is a legal move.</returns>
    public bool IsLegalMove(Move move)
    {
        return FindLegalMoves(move.StartSquare).Contains(move);
    }

    /// <summary>
    /// Finds whether or not the player is in check.
    /// </summary>
    public bool IsInCheck()
    {
        var opponentBoard = new Board(Turn, FEN);
        opponentBoard.ChangeTurn();
        List<Move> opponentMoves = opponentBoard.FindAllMoves();
        Square kingPosition = FindKing();

        // Find any opponent move that will take the player's king
        return opponentMoves.Any(move => move.EndSquare == kingPosition);
    }

    /// <summary>
    /// Finds the square your king is in.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private Square FindKing()
    {
        for (int i = 0; i <= 63; i++)
        {
            if (_state[i]?.Type == PieceType.King && _state[i]?.Colour == Turn)
            {
                return new Square(i);
            }
        }
        return null;
    }

    /// <summary>
    /// Makes a move.
    /// </summary>
    /// <param name="move"></param>
    /// <returns>The piece taken by the move, or null if no piece was taken.</returns>
    /// <exception cref="InvalidOperationException">Indicates an invalid move.</exception>
    public Piece MakeMove(Move move)
    {
        if (IsLegalMove(move))
        {
            return MakeUnsafeMove(move);
        }
        else
        {
            throw new InvalidOperationException("Invalid Move.");
        }
    }

    /// <summary>
    /// Makes a move without first checking whether it's valid.
    /// </summary>
    /// <param name="move"></param>
    /// <returns>The piece taken by the move, or null if no piece was taken.</returns>
    public Piece MakeUnsafeMove(Move move)
    {
        Piece piece = FindPieceOnSquare(move.StartSquare);
        Piece takenPiece = _state[move.EndSquare.Index];
        _state[move.StartSquare.Index] = null;
        PlacePiece(piece, move.EndSquare);

        return takenPiece;
    }

    /// <summary>
    /// Reverses a given move, returning any taken pieces to their original position.
    /// </summary>
    /// <param name="move">The move to undo.</param>
    /// <param name="takenPiece">The piece that was taken (null if no piece was taken).</param>
    public void UndoMove(Move move, Piece takenPiece)
    {
        Piece piece = FindPieceOnSquare(move.EndSquare);
        PlacePiece(piece, move.StartSquare);
        PlacePiece(takenPiece, move.EndSquare);
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

    public void ClearBoard()
    {
        _state = new Piece[64];
    }

    /// <summary>
    /// Finds all legal moves for a given square.
    /// </summary>
    /// <param name="square"></param>
    /// <returns>A list of moves.</returns>
    public List<Move> FindLegalMoves(Square square)
    {
        List<Move> moves = FindMoves(square);
        FilterForChecks(moves);
        return moves;
    }

    /// <summary>
    /// Finds all moves for a given square, regardless of whether they put you in check.
    /// </summary>
    /// <param name="square"></param>
    /// <returns>A list of moves.</returns>
    public List<Move> FindMoves(Square square)
    {
        Piece piece = FindPieceOnSquare(square);

        if (piece == null)
        {
            return null;
        }

        return piece.Type switch
        {
            PieceType.Pawn => FindPawnMoves(square),
            PieceType.Knight => FindKnightMoves(square),
            PieceType.Bishop => FindBishopMoves(square),
            PieceType.Rook => FindRookMoves(square),
            PieceType.Queen => FindQueenMoves(square),
            PieceType.King => FindKingMoves(square),
            _ => null,
        };
    }

    /// <summary>
    /// Find every legal move available on the board for a given colour.
    /// </summary>
    /// <returns>A list of all legal moves.</returns>
    public List<Move> FindAllLegalMoves()
    {
        List<Move> moves = FindAllMoves();
        FilterForChecks(moves);
        return moves;
    }

    /// <summary>
    /// Find every move available on the board for a given colour, regardless of whether it puts them in check.
    /// </summary>
    /// <returns>A list of all moves.</returns>
    public List<Move> FindAllMoves()
    {
        List<Move> moves = new List<Move>();

        for (int i = 0; i < 64; i++)
        {
            var square = new Square(i);

            if (FindPieceOnSquare(square) != null && FindPieceOnSquare(square).Colour == Turn)
            {
                moves.AddRange(FindMoves(square));
            }
        }

        return moves;
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
        return piece.Colour != Turn;
    }

    /// <summary>
    /// Removes all moves that result in the player being in check.
    /// </summary>
    /// <param name="moves"></param>
    private void FilterForChecks(List<Move> moves)
    {
        // Iterates through moves backwards so you can remove items
        for (int i = moves.Count - 1; i >= 0; i--)
        {
            Move moveCopy = moves[i]; // Copy the move so you can undo after it's deleted
            Piece takenPiece = MakeUnsafeMove(moves[i]);

            if (IsInCheck())
            {
                moves.RemoveAt(i);
            }

            UndoMove(moveCopy, takenPiece);
        }
    }

    private List<Move> FindPawnMoves(Square startSquare)
    {
        List<Move> moves = new();

        // Flag to indicate something in the way of the pawn
        bool isBlocked = false;

        // White pieces have +1 direction, black have -1
        int direction = (Turn == PieceColour.White) ? 1 : -1;

        // Max number of forward moves the pawn can take
        int maxMoves = ((startSquare.Row == 2 && direction == 1) || (startSquare.Row == 7 && direction == -1)) ? 2 : 1;

        // Checks for forward moves
        for (int i = 1; i <= maxMoves; i++)
        {
            // Prevents invalid moves
            if (!Square.IsValidSquare(startSquare.Col, startSquare.Row + i * direction))
            {
                continue;
            }
            var newSquare = new Square(startSquare.Col, startSquare.Row + i * direction);

            // Add square in front of pawn to moves if not blocked
            if (FindPieceOnSquare(newSquare) == null && !isBlocked)
            {
                moves.Add(new Move(startSquare, newSquare));
            }
            else
            {
                isBlocked = true;
            }
        }

        // Checks for diagonal moves
        for (int j = -1; j <= 1; j += 2)
        {
            // Prevents invalid moves
            if (!Square.IsValidSquare(startSquare.Col + j, startSquare.Row + direction))
            {
                continue;
            }
            var newSquare = new Square(startSquare.Col + j, startSquare.Row + direction);

            // Add diagonal square to moves if opponent's piece can be taken there
            Piece diagonalPiece = FindPieceOnSquare(newSquare);

            if (diagonalPiece != null && (diagonalPiece.Colour != FindPieceOnSquare(startSquare).Colour))
            {
                moves.Add(new Move(startSquare, newSquare));
            }
        }

        return moves;
    }

    private List<Move> FindRookMoves(Square startSquare)
    {
        // loop over both directions, then loop from start col to
        List<Move> moves = new();
        bool isBlocked = false;

        for (int rowDir = -1; rowDir <= 1; rowDir++)
        {
            for (int colDir = -1; colDir <= 1; colDir++)
            {
                for (int i = 1; i < 8; i++)
                {
                    // Prevents diagonal or invalid moves
                    if (rowDir != 0 && colDir != 0 || !Square.IsValidSquare(startSquare.Col + (i * colDir), startSquare.Row + (i * rowDir)))
                    {
                        continue;
                    }

                    var newSquare = new Square(startSquare.Col + (i * colDir), startSquare.Row + (i * rowDir));
                    Piece newSquarePiece = FindPieceOnSquare(newSquare);

                    if (!isBlocked)
                    {
                        // If square is empty
                        if (newSquarePiece == null)
                        {
                            moves.Add(new Move(startSquare, newSquare));
                        }

                        // If square is occupied by an enemy piece
                        else if (IsEnemyPiece(newSquarePiece))
                        {
                            moves.Add(new Move(startSquare, newSquare));
                            isBlocked = true;
                        }

                        // If square is occupied by friendly piece
                        else
                        {
                            isBlocked = true;
                        }
                    }
                }
                isBlocked = false;
            }
        }

        return moves;
    }

    private List<Move> FindKnightMoves(Square startSquare)
    {
        List<Move> moves = new();

        for (int i = -2; i <= 2; i += 4)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                if (Square.IsValidSquare(startSquare.Col + j, startSquare.Row + i))
                {
                    var s1 = new Square(startSquare.Col + j, startSquare.Row + i);

                    // If s1 is empty or occupied by an enemy piece
                    if (FindPieceOnSquare(s1) == null || IsEnemyPiece(FindPieceOnSquare(s1)))
                    {
                        moves.Add(new Move(startSquare, s1));
                    }
                }

                if (Square.IsValidSquare(startSquare.Col + i, startSquare.Row + j))
                {
                    var s2 = new Square(startSquare.Col + i, startSquare.Row + j);

                    // If s2 is empty or occupied by an enemy piece
                    if (FindPieceOnSquare(s2) == null || IsEnemyPiece(FindPieceOnSquare(s2)))
                    {
                        moves.Add(new Move(startSquare, s2));
                    }
                }
            }
        }

        return moves;
    }

    private List<Move> FindKingMoves(Square startSquare)
    {
        List<Move> moves = new();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (!Square.IsValidSquare(startSquare.Col + j, startSquare.Row + i))
                {
                    continue;
                }

                var newSquare = new Square(startSquare.Col + j, startSquare.Row + i);
                Piece newSquarePiece = FindPieceOnSquare(newSquare);

                // If square is empty or is occupied by an enemy piece
                if (newSquarePiece == null || IsEnemyPiece(newSquarePiece))
                {
                    moves.Add(new Move(startSquare, newSquare));
                }
            }
        }

        return moves;
    }

    private List<Move> FindBishopMoves(Square startSquare)
    {
        List<Move> moves = new();
        bool isBlocked = false;

        for (int rowDir = -1; rowDir <= 1; rowDir += 2)
        {
            for (int colDir = -1; colDir <= 1; colDir += 2)
            {
                for (int i = 1; i < 8; i++)
                {
                    if (!Square.IsValidSquare(startSquare.Col + (i * colDir), startSquare.Row + (i * rowDir)))
                    {
                        continue;
                    }

                    var newSquare = new Square(startSquare.Col + (i * colDir), startSquare.Row + (i * rowDir));

                    if (!isBlocked)
                    {
                        // If square is empty
                        if (FindPieceOnSquare(newSquare) == null)
                        {
                            moves.Add(new Move(startSquare, newSquare));
                        }

                        // If square is occupied by piece
                        else
                        {
                            // If square is occupied by an enemy piece
                            if (IsEnemyPiece(FindPieceOnSquare(newSquare)))
                            {
                                moves.Add(new Move(startSquare, newSquare));
                            }

                            isBlocked = true;
                        }
                    }
                }
                isBlocked = false;
            }
        }

        return moves;
    }

    private List<Move> FindQueenMoves(Square startSquare)
    {
        List<Move> moves = FindBishopMoves(startSquare);
        moves.AddRange(FindRookMoves(startSquare));

        return moves;
    }

    public void ChangeTurn()
    {
        Turn = (Turn == PieceColour.White) ? PieceColour.Black : PieceColour.White;
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