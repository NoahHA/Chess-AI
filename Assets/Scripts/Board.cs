using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Antlr3.Runtime;
using UnityEngine;

/// <summary>
/// Defines the state of a board, including the positions of every piece and who's turn it is.
/// </summary>
public class Board
{
    private Piece[] _state = new Piece[64];
    private string _fen;
    private PieceColour _turn;
    private bool[] _canCastle = new bool[4] { true, true, true, true };
    private Square _enPassantSquare;

    // Always update the FEN when setting the state so FEN is always up to date
    public Piece[] State
    {
        get => _state;
        private set { _state = value; _fen = ResetFen(); }
    }

    // Always update the State when setting the FEN so state is always up to date
    public string FEN
    {
        get => _fen;
        set
        {
            _fen = value;
            _state = value.GetStateFromFen();
            _turn = value.GetTurnFromFen();
            _enPassantSquare = value.GetEnPassantSquareFromFen();
            _canCastle = value.GetCanCastleFromFen();
        }
    }

    [Tooltip("An enum representing who's turn it is currently.")]
    public PieceColour Turn
    {
        get => _turn;
        set { _turn = value; _fen = ResetFen(); }
    }

    public bool[] CanCastle
    {
        get => _canCastle;
        set { _canCastle = value; _fen = ResetFen(); }
    }

    public Square EnPassantSquare
    {
        get => _enPassantSquare;
        set { _enPassantSquare = value; _fen = ResetFen(); }
    }

    public Board(string fen = "8/8/8/8/8/8/8/8 w KQkq -")
    {
        FEN = fen;
    }

    public Board(PieceColour turn)
    {
        FEN = "8/8/8/8/8/8/8/8 w KQkq -";
        Turn = turn;
    }

    public void PlacePiece(Piece piece, Square position)
    {
        State[position.Index] = piece;
        FEN = ResetFen();
    }

    /// <summary>
    /// Checks if a given move is legal.
    /// </summary>
    /// <param name="move"></param>
    /// <returns>Whether the move is a legal move.</returns>
    public bool IsLegalMove(Move move, PieceColour turn)
    {
        return FindLegalMoves(move.StartSquare, turn).Contains(move);
    }

    /// <summary>
    /// Finds whether or not the player is in check.
    /// </summary>
    public bool IsInCheck(PieceColour turn)
    {
        Square kingPosition = FindKing(turn);
        var opponentBoard = new Board(FEN);
        opponentBoard.Turn = turn;
        opponentBoard.ChangeTurn();

        // Find any opponent move that will take the player's king
        return opponentBoard.FindAllMoves(opponentBoard.Turn).Any(move => move.EndSquare == kingPosition);
    }

    /// <summary>
    /// Finds the square your king is in.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private Square FindKing(PieceColour turn)
    {
        return new Square(
            Array.IndexOf(
                _state, _state.FirstOrDefault(piece => piece?.Type == PieceType.King && piece?.Colour == turn)
            )
        );
    }

    /// <summary>
    /// Makes a move (but does not check whether it's valid).
    /// </summary>
    /// <param name="move"></param>
    /// <returns>The piece taken by the move, or null if no piece was taken.</returns>
    public void MakeMove(Move move)
    {
        Piece piece = FindPieceOnSquare(move.StartSquare);
        PlacePiece(piece, move.EndSquare);
        PlacePiece(null, move.StartSquare);

        // If castling also move the rook
        if (move.Castling)
        {
            Castling castlingType = (move.StartSquare.Col > move.EndSquare.Col) ? Castling.QueenSide : Castling.KingSide;
            DisableCastling(piece.Colour);

            Square rookStartSquare = new((castlingType == Castling.QueenSide) ? 1 : 8, move.StartSquare.Row);
            Square rookEndSquare = new((castlingType == Castling.QueenSide) ? 4 : 6, move.StartSquare.Row);

            MakeMove(new Move(rookStartSquare, rookEndSquare));
        }

        // If en passant then take the en-passanted pawn
        if (move.EnPassant)
        {
            int dir = piece.Colour == PieceColour.White ? 1 : -1;
            PlacePiece(null, new Square(move.EndSquare.Col, move.EndSquare.Row - dir));
        }

        // If king moves, disable castling for that colour
        if (piece?.Type == PieceType.King)
        {
            DisableCastling(piece.Colour);
        }
        // If rook moves, disable castling for that rook
        else if (piece?.Type == PieceType.Rook && (move.StartSquare.Col == 1 || move.StartSquare.Col == 8))
        {
            Castling castlingType = (move.StartSquare.Col == 1) ? Castling.QueenSide : Castling.KingSide;
            DisableCastling(castlingType, piece.Colour);
        }
        // If pawn gets to the end, upgrade it
        else if (piece?.Type == PieceType.Pawn && (move.EndSquare.Row == 1 || move.EndSquare.Row == 8))
        {
            State[move.EndSquare.Index] = new Piece(PieceType.Queen, piece.Colour);
        }

        // If pawn makes a double move, make en passant available
        if (piece?.Type == PieceType.Pawn && Math.Abs(move.EndSquare.Row - move.StartSquare.Row) == 2)
        {
            int dir = piece.Colour == PieceColour.White ? 1 : -1;
            EnPassantSquare = new Square(move.EndSquare.Col, move.EndSquare.Row - dir);
        }
        else
        {
            EnPassantSquare = null;
        }
    }

    private void DisableCastling(Castling castlingType, PieceColour colour)
    {
        int idx = (castlingType == Castling.KingSide) ? (colour == PieceColour.White) ? 0 : 2 : (colour == PieceColour.White) ? 1 : 3;
        CanCastle[idx] = false;
        ResetFen();
    }

    private void DisableCastling(PieceColour colour)
    {
        if (colour == PieceColour.White)
        {
            CanCastle[0] = false;
            CanCastle[1] = false;
        }
        else
        {
            CanCastle[2] = false;
            CanCastle[3] = false;
        }
        ResetFen();
    }

    public string ResetFen()
    {
        int counter = 0;
        string tempFen = "";

        // Start from top left and go across each column and down each row
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
            if (State[i] == null)
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

        tempFen += (_turn == PieceColour.White) ? " w " : " b ";

        if (CanCastle[0]) tempFen += 'K';
        if (CanCastle[1]) tempFen += 'Q';
        if (CanCastle[2]) tempFen += 'k';
        if (CanCastle[3]) tempFen += 'q';
        if (CanCastle.All(castle => castle == false)) tempFen += "-";

        string enPassantString = (EnPassantSquare == null) ? "-" : EnPassantSquare.ToString();
        tempFen += " " + enPassantString;

        return tempFen;
    }

    public static bool operator ==(Board obj1, Board obj2)
    {
        return obj1.FEN == obj2.FEN;
    }

    public static bool operator !=(Board obj1, Board obj2)
    {
        return obj1.FEN != obj2.FEN;
    }

    public void SetBoardToStartingPosition()
    {
        FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq -";
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
            return FEN == boardState.FEN;
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
        State = new Piece[64];
    }

    /// <summary>
    /// Finds all legal moves for a given square.
    /// </summary>
    /// <param name="square"></param>
    /// <returns>A list of moves.</returns>
    public List<Move> FindLegalMoves(Square square, PieceColour turn)
    {
        List<Move> moves = FindMoves(square, turn);
        FilterForChecks(moves, turn);
        return moves;
    }

    /// <summary>
    /// Finds all moves for a given square, regardless of whether they put you in check.
    /// </summary>
    /// <param name="square"></param>
    /// <returns>A list of moves.</returns>
    public List<Move> FindMoves(Square square, PieceColour turn)
    {
        Piece piece = FindPieceOnSquare(square);

        if (piece == null || piece.Colour != turn)
        {
            return new List<Move>();
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
    public List<Move> FindAllLegalMoves(PieceColour turn)
    {
        List<Move> moves = FindAllMoves(turn);
        FilterForChecks(moves, turn);
        return moves;
    }

    /// <summary>
    /// Find every move available on the board for a given colour, regardless of whether it puts them in check.
    /// </summary>
    /// <returns>A list of all moves.</returns>
    public List<Move> FindAllMoves(PieceColour turn)
    {
        List<Move> moves = new List<Move>();

        for (int i = 0; i < 64; i++)
        {
            var square = new Square(i);

            if (FindPieceOnSquare(square) != null && FindPieceOnSquare(square).Colour == turn)
            {
                moves.AddRange(FindMoves(square, turn));
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
        return State[square.Index];
    }

    public bool IsEnemyPiece(Piece piece, PieceColour turn)
    {
        return piece.Colour != turn;
    }

    /// <summary>
    /// Removes all moves that result in the player being in check.
    /// </summary>
    /// <param name="moves"></param>
    private void FilterForChecks(List<Move> moves, PieceColour turn)
    {
        string initialFen = string.Copy(FEN);

        // Iterates through moves backwards so you can remove items
        for (int i = moves.Count - 1; i >= 0; i--)
        {
            MakeMove(moves[i]);

            if (IsInCheck(turn))
            {
                moves.RemoveAt(i);
            }

            FEN = initialFen;
        }
    }

    public bool IsInCheckmate(PieceColour turn)
    {
        List<Move> legalMoves = FindAllLegalMoves(turn);
        FilterForChecks(legalMoves, turn);
        return legalMoves.Count == 0;
    }

    private List<Move> FindPawnMoves(Square startSquare)
    {
        List<Move> moves = new();
        PieceColour turn = FindPieceOnSquare(startSquare).Colour;

        // Flag to indicate something in the way of the pawn
        bool isBlocked = false;

        // White pieces have +1 direction, black have -1
        int direction = (turn == PieceColour.White) ? 1 : -1;

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

            // En passant
            if (newSquare == EnPassantSquare)
            {
                moves.Add(new Move(startSquare, newSquare, enPassant: true));
            }
            else if (diagonalPiece != null && (diagonalPiece.Colour != FindPieceOnSquare(startSquare).Colour))
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
        PieceColour turn = FindPieceOnSquare(startSquare).Colour;
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
                        else if (IsEnemyPiece(newSquarePiece, turn))
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
        PieceColour turn = FindPieceOnSquare(startSquare).Colour;

        for (int i = -2; i <= 2; i += 4)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                if (Square.IsValidSquare(startSquare.Col + j, startSquare.Row + i))
                {
                    var s1 = new Square(startSquare.Col + j, startSquare.Row + i);

                    // If s1 is empty or occupied by an enemy piece
                    if (FindPieceOnSquare(s1) == null || IsEnemyPiece(FindPieceOnSquare(s1), turn))
                    {
                        moves.Add(new Move(startSquare, s1));
                    }
                }

                if (Square.IsValidSquare(startSquare.Col + i, startSquare.Row + j))
                {
                    var s2 = new Square(startSquare.Col + i, startSquare.Row + j);

                    // If s2 is empty or occupied by an enemy piece
                    if (FindPieceOnSquare(s2) == null || IsEnemyPiece(FindPieceOnSquare(s2), turn))
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
        PieceColour turn = FindPieceOnSquare(startSquare).Colour;

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
                if (newSquarePiece == null || IsEnemyPiece(newSquarePiece, turn))
                {
                    moves.Add(new Move(startSquare, newSquare));
                }
            }
        }

        moves.AddRange(FindCastlingMoves(turn));

        return moves;
    }

    private List<Move> FindCastlingMoves(PieceColour turn)
    {
        List<Move> moves = new();
        Square kingPosition = FindKing(turn);
        List<Square> kingRow = Enumerable.Range(1, 8).Select(i => new Square(i, kingPosition.Row)).ToList();

        // If they can castle queen side (king and rook haven't moved and no pieces are between them)
        if (FEN.CanCastle(Castling.QueenSide, turn) && kingRow.GetRange(1, 3).All(s => FindPieceOnSquare(s) == null))
        {
            moves.Add(new Move(kingPosition, kingRow[2], castling: true));
        }

        // If they can castle king side
        if (FEN.CanCastle(Castling.KingSide, turn) && kingRow.GetRange(5, 2).All(s => FindPieceOnSquare(s) == null))
        {
            moves.Add(new Move(kingPosition, kingRow[6], castling: true));
        }

        return moves;
    }

    private List<Move> FindBishopMoves(Square startSquare)
    {
        List<Move> moves = new();
        PieceColour turn = FindPieceOnSquare(startSquare).Colour;
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
                            if (IsEnemyPiece(FindPieceOnSquare(newSquare), turn))
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

    public bool IsCastleMove(Move move)
    {
        return (FindPieceOnSquare(move.StartSquare)?.Type == PieceType.King
            && Math.Abs(move.EndSquare.Col - move.StartSquare.Col) > 1);
    }

    public bool IsEnPassantMove(Move move)
    {
        return (move.EndSquare == EnPassantSquare
            && FindPieceOnSquare(move.StartSquare)?.Type == PieceType.Pawn);
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