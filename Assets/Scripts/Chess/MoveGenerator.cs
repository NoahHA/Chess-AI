using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoveGenerator
{
    public static List<Move> GenerateMoves(Board board, Square startSquare, PieceColour turn, Piece piece)
    {
        List<Move> moves = new();
        moves.Clear();

        if (piece != null && piece.Colour == turn)
        {
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    GeneratePawnMoves(board, startSquare, turn, moves);
                    break;

                case PieceType.Knight:
                    GenerateKnightMoves(board, startSquare, turn, moves);
                    break;

                case PieceType.Bishop:
                    GenerateBishopMoves(board, startSquare, turn, moves);
                    break;

                case PieceType.Rook:
                    GenerateRookMoves(board, startSquare, turn, moves);
                    break;

                case PieceType.Queen:
                    GenerateQueenMoves(board, startSquare, turn, moves);
                    break;

                case PieceType.King:
                    GenerateKingMoves(board, startSquare, turn, moves);
                    break;
            }
        }

        return moves;
    }

    public static List<Move> GenerateMoves(Board board, Square startSquare, PieceColour turn)
    {
        return GenerateMoves(board, startSquare, turn, board.FindPieceOnSquare(startSquare));
    }

    private static void GeneratePawnMoves(Board board, Square startSquare, PieceColour turn, List<Move> moves)
    {
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
            if (board.FindPieceOnSquare(newSquare) == null && !isBlocked)
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
            Piece diagonalPiece = board.FindPieceOnSquare(newSquare);

            // En passant
            if (newSquare == board.EnPassantSquare)
            {
                moves.Add(new Move(startSquare, newSquare, enPassant: true));
            }
            else if (diagonalPiece != null && (diagonalPiece.Colour != board.FindPieceOnSquare(startSquare).Colour))
            {
                moves.Add(new Move(startSquare, newSquare));
            }
        }
    }

    private static void GenerateKnightMoves(Board board, Square startSquare, PieceColour turn, List<Move> moves)
    {
        for (int i = -2; i <= 2; i += 4)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                if (Square.IsValidSquare(startSquare.Col + j, startSquare.Row + i))
                {
                    var s1 = new Square(startSquare.Col + j, startSquare.Row + i);

                    // If s1 is empty or occupied by an enemy piece
                    if (board.FindPieceOnSquare(s1) == null || board.IsEnemyPiece(board.FindPieceOnSquare(s1), turn))
                    {
                        moves.Add(new Move(startSquare, s1));
                    }
                }

                if (Square.IsValidSquare(startSquare.Col + i, startSquare.Row + j))
                {
                    var s2 = new Square(startSquare.Col + i, startSquare.Row + j);

                    // If s2 is empty or occupied by an enemy piece
                    if (board.FindPieceOnSquare(s2) == null || board.IsEnemyPiece(board.FindPieceOnSquare(s2), turn))
                    {
                        moves.Add(new Move(startSquare, s2));
                    }
                }
            }
        }
    }

    private static void GenerateBishopMoves(Board board, Square startSquare, PieceColour turn, List<Move> moves)
    {
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
                        if (board.FindPieceOnSquare(newSquare) == null)
                        {
                            moves.Add(new Move(startSquare, newSquare));
                        }

                        // If square is occupied by piece
                        else
                        {
                            // If square is occupied by an enemy piece
                            if (board.IsEnemyPiece(board.FindPieceOnSquare(newSquare), turn))
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
    }

    private static void GenerateRookMoves(Board board, Square startSquare, PieceColour turn, List<Move> moves)
    {
        // loop over both directions, then loop from start col to
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
                    Piece newSquarePiece = board.FindPieceOnSquare(newSquare);

                    if (!isBlocked)
                    {
                        // If square is empty
                        if (newSquarePiece == null)
                        {
                            moves.Add(new Move(startSquare, newSquare));
                        }

                        // If square is occupied by an enemy piece
                        else if (board.IsEnemyPiece(newSquarePiece, turn))
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
    }

    private static void GenerateQueenMoves(Board board, Square startSquare, PieceColour turn, List<Move> moves)
    {
        GenerateBishopMoves(board, startSquare, turn, moves);
        GenerateRookMoves(board, startSquare, turn, moves);
    }

    private static void GenerateKingMoves(Board board, Square startSquare, PieceColour turn, List<Move> moves)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (!Square.IsValidSquare(startSquare.Col + j, startSquare.Row + i))
                {
                    continue;
                }

                var newSquare = new Square(startSquare.Col + j, startSquare.Row + i);
                Piece newSquarePiece = board.FindPieceOnSquare(newSquare);

                // If square is empty or is occupied by an enemy piece
                if (newSquarePiece == null || board.IsEnemyPiece(newSquarePiece, turn))
                {
                    moves.Add(new Move(startSquare, newSquare));
                }
            }
        }

        GenerateCastlingMoves(board, turn, moves);
    }

    private static void GenerateCastlingMoves(Board board, PieceColour turn, List<Move> moves)
    {
        Square kingPosition = board.FindKing(turn);
        List<Square> kingRow = Enumerable.Range(1, 8).Select(i => new Square(i, kingPosition.Row)).ToList();

        // If they can castle queen side (king and rook haven't moved and no pieces are between them)
        if (board.FEN.CanCastle(Castling.QueenSide, turn) && kingRow.GetRange(1, 3).All(s => board.FindPieceOnSquare(s) == null))
        {
            moves.Add(new Move(kingPosition, kingRow[2], castling: true));
        }

        // If they can castle king side
        if (board.FEN.CanCastle(Castling.KingSide, turn) && kingRow.GetRange(5, 2).All(s => board.FindPieceOnSquare(s) == null))
        {
            moves.Add(new Move(kingPosition, kingRow[6], castling: true));
        }
    }
}