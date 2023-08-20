using System;
using System.Collections.Generic;
using System.Linq;
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
            _state = UpdateStateFromFen(value);
            _turn = value.GetTurnFromFen();
            _enPassantSquare = value.GetEnPassantSquareFromFen();
            _canCastle = value.GetCanCastleFromFen();
        }
    }

    [Tooltip("An enum representing who's turn it is currently.")]
    public PieceColour Turn
    {
        get => _turn;
        set { _turn = value; _fen = _fen.UpdateTurn(value); }
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

    /// <summary>
    /// Updates the board state based on a chess FEN string.
    /// </summary>
    /// <param name="fen">FEN string</param>
    public Piece[] UpdateStateFromFen(string fen)
    {
        int counter = 0;
        int spaceIdx = fen.IndexOf(' ');

        foreach (char c in fen[0..spaceIdx])
        {
            if (char.IsDigit(c))
            {
                for (int i = 0; i < (int)char.GetNumericValue(c); i++)
                {
                    _state[counter] = null;
                    counter++;
                }
            }
            else if (char.IsLetter(c))
            {
                if (_state[counter] is null)
                {
                    _state[counter] = new Piece(c);
                }
                else if (_state[counter].Letter != c)
                {
                    _state[counter].Letter = c;
                }
                counter++;
            }
        }

        return _state;
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
        PieceColour opponentTurn = turn.ChangeTurn();

        // Find which pieces could take the king by finding which pieces the king could take if it were all other pieces
        List<Move> movesFromKingPosition = MoveGenerator.GenerateMoves(this, kingPosition, turn, new Piece(PieceType.Queen, turn));
        movesFromKingPosition.AddRange(MoveGenerator.GenerateMoves(this, kingPosition, turn, new Piece(PieceType.Knight, turn)));
        bool kingCanBeTaken = movesFromKingPosition.Any(move => FindPieceOnSquare(move.EndSquare)?.Colour == opponentTurn);

        if (!kingCanBeTaken)
        {
            return false;
        }

        ChangeTurn();
        // Find any opponent move that will take the player's king
        bool isInCheck = FindAllMoves(Turn).Any(move => move.EndSquare == kingPosition);
        ChangeTurn();
        return isInCheck;
    }

    /// <summary>
    /// Finds the square your king is in.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public Square FindKing(PieceColour turn)
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
            int dir = (piece.Colour == PieceColour.White) ? 1 : -1;
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

    public bool CanTakeEnPassant()
    {
        if (EnPassantSquare is null)
        {
            return false;
        }

        if (EnPassantSquare.Row == 3)
        {
            Square firstTakeLocation = new Square(EnPassantSquare.Col + 1, EnPassantSquare.Row + 1);
            Square secondTakeLocation = new Square(EnPassantSquare.Col - 1, EnPassantSquare.Row + 1);

            return FindPieceOnSquare(firstTakeLocation)?.Letter == 'p' || FindPieceOnSquare(secondTakeLocation)?.Letter == 'p';
        }

        else
        {
            Square firstTakeLocation = new Square(EnPassantSquare.Col + 1, EnPassantSquare.Row - 1);
            Square secondTakeLocation = new Square(EnPassantSquare.Col - 1, EnPassantSquare.Row - 1);

            return FindPieceOnSquare(firstTakeLocation)?.Letter == 'P' || FindPieceOnSquare(secondTakeLocation)?.Letter == 'P';
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
        List<Move> moves = MoveGenerator.GenerateMoves(this, square, turn);
        FilterForChecks(moves, turn);
        return moves;
    }

    /// <summary>
    /// Removes all moves that result in the player being in check.
    /// </summary>
    /// <param name="moves"></param>
    public void FilterForChecks(List<Move> moves, PieceColour turn)
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
        List<Move> moves = new();

        for (int i = 0; i < 64; i++)
        {
            var square = new Square(i);
            moves.AddRange(MoveGenerator.GenerateMoves(this, square, turn));
        }

        return moves;
    }

    public Piece FindPieceOnSquare(Square square) => State[square.Index];

    public Piece FindPieceOnSquare(int index) => State[index];

    public bool IsEnemyPiece(Piece piece, PieceColour turn) => piece.Colour != turn;

    public bool IsInCheckmate(PieceColour turn)
    {
        if (!IsInCheck(turn))
        {
            return false;
        }

        ;
        return FindAllLegalMoves(turn).Count == 0;
    }

    public bool IsInStalemate(PieceColour turn)
    {
        // If there's any non-king move that can be made then you're not in stalemate
        if (HasNonKingMove(turn))
        {
            return false;
        }

        if (!IsInCheck(turn) && FindAllLegalMoves(turn).Count == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Whether the player has any non-king moves available, used to calculate whether you're in stalemate.
    /// </summary>
    private bool HasNonKingMove(PieceColour turn)
    {
        for (int i = 0; i < 64; i++)
        {
            var square = new Square(i);
            Piece piece = FindPieceOnSquare(square);

            if (piece is null || piece.Colour != turn || piece.Type == PieceType.King)
            {
                continue;
            }

            if (MoveGenerator.GenerateMoves(this, square, turn).Count > 0)
            {
                return true;
            }
        }

        return false;
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