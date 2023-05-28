using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles overall game behaviour
/// </summary>
public class GameController : MonoBehaviour
{
    [Tooltip("Who's turn it is, false=black, true=white")]
    public static bool playerTurn = true;

    /// <summary>
    /// Returns the legal moves for a given piece and board location
    /// </summary>
    /// <param name="position"> Chess tile that the selected piece is on </param>
    /// <param name="piece"> The selected piece </param>
    /// <param name="turn"> The player whos turn it is: true=white, false=black </param>
    /// <returns> </returns>
    public static List<Move> GetLegalPieceMoves(ChessSquare position, GameObject piece, bool turn)
    {
        List<Move> moves = new();
        string pieceName = piece.name;

        if (Board.ValidPieceClicked(pieceName))
        {
            // temporarily ignore piece so it doesn't collide with itself
            piece.layer = LayerMask.NameToLayer("Ignore");

            // Checks for the type of piece and calls the relevant function
            if (pieceName.Contains("pawn")) moves.AddRange(GetPawnMoves(position, turn, piece));
            else if (pieceName.Contains("rook")) moves.AddRange(GetRookMoves(position, turn, piece));
            else if (pieceName.Contains("bishop")) moves.AddRange(GetBishopMoves(position, turn, piece));
            else if (pieceName.Contains("queen")) moves.AddRange(GetQueenMoves(position, turn, piece));
            else if (pieceName.Contains("knight")) moves.AddRange(GetKnightMoves(position, turn, piece));
            else if (pieceName.Contains("king"))
            {
                moves.AddRange(GetKingMoves(position, turn, piece));
                moves.AddRange(GetCastlingMoves(turn, piece));
            }

            piece.layer = LayerMask.NameToLayer("Default");
        }
        // Filters out illegal moves
        moves = FilterMoves(moves);
        return moves;
    }

    /// <summary>
    /// Finds all legal moves for a given player to make that turn
    /// </summary>
    /// <param name="turn"> Who's turn it is, white=1, black=0 </param>
    /// <returns>
    /// Dictionary containing the GameObject and a list of move locations for each piece
    /// </returns>
    public static List<Move> GetLegalMoves(bool turn)
    {
        List<GameObject> playerPieces = GetPlayerPieces(turn);
        List<Move> moves = new();

        foreach (GameObject piece in playerPieces)
        {
            var position = new ChessSquare(piece.gameObject.transform.position);

            if (piece.name.Contains("pawn"))
                moves.AddRange(GetPawnMoves(position, turn, piece));
            else if (piece.name.Contains("rook"))
                moves.AddRange(GetRookMoves(position, turn, piece));
            else if (piece.name.Contains("bishop"))
                moves.AddRange(GetBishopMoves(position, turn, piece));
            else if (piece.name.Contains("queen"))
                moves.AddRange(GetQueenMoves(position, turn, piece));
            else if (piece.name.Contains("knight"))
                moves.AddRange(GetKnightMoves(position, turn, piece));
            else if (piece.name.Contains("king"))
            {
                var kingMoves = GetKingMoves(position, turn, piece);
                kingMoves.AddRange(GetCastlingMoves(turn, piece));
                moves.AddRange(kingMoves);
            }
        }
        // filters out illegal moves
        moves = FilterMoves(moves);
        return moves;
    }

    /// <summary>
    /// Filters out illegal moves from the list of available moves.
    /// </summary>
    /// <param name="moves"> A list of moves. </param>
    /// <returns> A filtered version of moves </returns>
    public static List<Move> FilterMoves(List<Move> moves)
    {
        moves.RemoveAll(move => move.Square.Row > 7 || move.Square.Row < 0 || move.Square.Col > 7 || move.Square.Col < 0);
        return moves;
    }

    /// <summary>
    /// Checks whether the player is currently in check
    /// </summary>
    /// <param name="turn"> true if white is playing, false if black is playing </param>
    /// <returns> </returns>
    public static bool IsInCheck(bool turn)
    {
        List<Move> opponentMoves = GetLegalMoves(!turn);
        ChessSquare kingPosition = FindKing(turn);

        // Find any opponent move that will take the player's king
        foreach (Move move in opponentMoves)
        {
            if (move.Square.Location == kingPosition.Location)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks whether the player has been checkmated
    /// </summary>
    /// <param name="turn"> true if white is playing, false if black is playing </param>
    /// <returns> </returns>
    public static bool IsInCheckmate(bool turn)
    {
        List<Move> playerMoves = GetLegalMoves(turn);

        // Find any move that will not put the player in check
        foreach (Move move in playerMoves)
        {
            move.MakeMove(); // Move the piece
            bool inCheck = IsInCheck(turn); // Check if player is in check
            move.UnmakeMove(); // Unmove the piece

            if (!inCheck)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Finds the location of a given player's king
    /// </summary>
    /// <param name="turn"> Which player to look for: white=1, black=0 </param>
    /// <returns> The chess square that the player's king is on </returns>
    public static ChessSquare FindKing(bool turn)
    {
        GameObject king;

        if (turn) king = GameObject.Find("w_king(Clone)");
        else king = GameObject.Find("b_king(Clone)");

        var kingPosition = new ChessSquare(king.transform.position);

        return kingPosition;
    }

    /// <summary>
    /// Gets a list of the pieces a player has on the board
    /// </summary>
    /// <param name="player"> </param>
    /// <returns> An array of GameObjects for each player piece </returns>
    private static List<GameObject> GetPlayerPieces(bool player)
    {
        List<GameObject> playerPieces = new();

        // Loops through all GameObjects
        foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            // If object is a piece and is active i.e. still on the board
            if (gameObj.activeSelf && gameObj.tag == "Piece")
            {
                // If piece is the right colour
                if ((gameObj.name.Contains("b_") && !player) || (gameObj.name.Contains("w_") && player))
                {
                    playerPieces.Add(gameObj);
                }
            }
        }
        return playerPieces;
    }

    private static List<Move> GetPawnMoves(ChessSquare position, bool turn, GameObject piece)
    {
        List<Move> pawnMoves = new();
        // Flag to indicate the something in the way of the pawn
        bool isBlocked = false;
        // White pieces have +1 direction, black have -1
        int direction = 1;
        if (!turn) { direction = -1; }
        // Max number of forward moves the pawn can take
        int max_moves = 1;

        if ((position.Row == 6 && direction == 1) || (position.Row == 1 && direction == -1))
            max_moves++;

        // Checks for forward moves
        for (int i = 1; i < max_moves + 1; i++)
        {
            var square = new ChessSquare(position.Row - i * direction, position.Col);

            // Add square in front of pawn to moves if not blocked
            if (Board.FindPieceOnSquare(square) == null && !isBlocked)
                pawnMoves.Add(new Move(piece, square));
            else { isBlocked = true; }
        }

        // Checks for diagonal moves
        for (int j = -1; j < 2; j += 2)
        {
            var square = new ChessSquare(position.Row - 1 * direction, position.Col + j);

            // Add diagonal square to moves if opponent's piece can be taken there
            Collider2D diagonalPiece = Board.FindPieceOnSquare(square);
            if (diagonalPiece != null)
            {
                if (Board.IsEnemyPiece(turn, diagonalPiece.gameObject))
                    pawnMoves.Add(new Move(piece, square));
            }
        }

        return pawnMoves;
    }

    private static List<Move> GetRookMoves(ChessSquare position, bool turn, GameObject piece)
    {
        List<Move> rookMoves = new();
        bool isBlocked = false;

        for (int row_dir = -1; row_dir < 2; row_dir += 1)
        {
            for (int col_dir = -1; col_dir < 2; col_dir += 1)
            {
                for (int i = 1; i < 8; i++)
                {
                    if (row_dir != 0 && col_dir != 0) { continue; }

                    var square = new ChessSquare(position.Row + (i * row_dir), position.Col + (i * col_dir));

                    if (!isBlocked)
                    {
                        // If square is empty
                        if (Board.FindPieceOnSquare(square) == null)
                            rookMoves.Add(new Move(piece, square));

                        // If square is occupied
                        else
                        {
                            // If square is occupied by an enemy piece
                            if (Board.IsEnemyPiece(turn, Board.FindPieceOnSquare(square).gameObject))
                                rookMoves.Add(new Move(piece, square));

                            isBlocked = true;
                        }
                    }
                }
                isBlocked = false;
            }
        }
        return rookMoves;
    }

    private static List<Move> GetBishopMoves(ChessSquare position, bool turn, GameObject piece)
    {
        List<Move> bishopMoves = new();
        bool isBlocked = false;

        for (int row_dir = -1; row_dir < 2; row_dir += 2)
        {
            for (int col_dir = -1; col_dir < 2; col_dir += 2)
            {
                for (int i = 1; i < 8; i++)
                {
                    var square = new ChessSquare(position.Row + (i * row_dir), position.Col + (i * col_dir));

                    if (!isBlocked)
                    {
                        // If square is empty
                        if (Board.FindPieceOnSquare(square) == null)
                            bishopMoves.Add(new Move(piece, square));

                        // If square is occupied by piece
                        else
                        {
                            // If square is occupied by an enemy piece
                            if (Board.IsEnemyPiece(turn, Board.FindPieceOnSquare(square).gameObject))
                                bishopMoves.Add(new Move(piece, square));

                            isBlocked = true;
                        }
                    }
                }
                isBlocked = false;
            }
        }
        return bishopMoves;
    }

    private static List<Move> GetQueenMoves(ChessSquare position, bool turn, GameObject piece)
    {
        List<Move> queenMoves = new();
        queenMoves.AddRange(GetBishopMoves(position, turn, piece));
        queenMoves.AddRange(GetRookMoves(position, turn, piece));

        return queenMoves;
    }

    private static List<Move> GetKingMoves(ChessSquare position, bool turn, GameObject piece)
    {
        List<Move> kingMoves = new();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                var square = new ChessSquare(position.Row + i, position.Col + j);

                // If square is empty
                if (Board.FindPieceOnSquare(square) == null)
                    kingMoves.Add(new Move(piece, square));

                // If square is occupied by an enemy piece
                else if (Board.IsEnemyPiece(turn, Board.FindPieceOnSquare(square).gameObject))
                    kingMoves.Add(new Move(piece, square));
            }
        }
        return kingMoves;
    }

    private static List<Move> GetCastlingMoves(bool turn, GameObject piece)
    {
        List<Move> castlingMoves = new();
        ChessSquare kingPosition = FindKing(turn);
        GameObject kingPiece = GameObject.Find("b_king(Clone)");
        if (turn) kingPiece = GameObject.Find("w_king(Clone)");
        var kingMoved = kingPiece.GetComponent<HasPieceMoved>();

        // If the king has already moved, don't add any moves
        if (kingMoved.hasMoved)
            return castlingMoves;

        // If king is in the correct position
        if (kingPosition.Col == 4 && ((turn && kingPosition.Row == 7) || (!turn && kingPosition.Row == 0)))
        {
            var closeCastlePosition = new ChessSquare(kingPosition.Row, kingPosition.Col + 3);
            var farCastlePosition = new ChessSquare(kingPosition.Row, kingPosition.Col - 4);

            Collider2D closeCastlePiece = Board.FindPieceOnSquare(closeCastlePosition);
            Collider2D farCastlePiece = Board.FindPieceOnSquare(farCastlePosition);

            // temporarily ignore piece for linecast
            kingPiece.layer = LayerMask.NameToLayer("Ignore");

            // If kingside castle is in its starting position
            if (closeCastlePiece != null)
            {
                if (closeCastlePiece.gameObject.name.Contains("rook"))
                {
                    bool closeCastleMoved = closeCastlePiece.gameObject.GetComponent<HasPieceMoved>().hasMoved;

                    // temporarily ignore piece for linecast
                    closeCastlePiece.gameObject.layer = LayerMask.NameToLayer("Ignore");

                    // If nothing between king and castle and castle hasn't moved
                    if (!closeCastleMoved && !Physics2D.Linecast(kingPosition.Location, closeCastlePiece.gameObject.transform.position, LayerMask.GetMask("Default")))
                    {
                        var square = new ChessSquare(kingPosition.Row, kingPosition.Col + 2);
                        castlingMoves.Add(new Move(piece, square));
                    }

                    // Unignore piece
                    closeCastlePiece.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }

            // If queenside castle is in its starting position
            if (farCastlePiece != null)
            {
                if (farCastlePiece.gameObject.name.Contains("rook"))
                {
                    bool farCastleMoved = farCastlePiece.GetComponent<HasPieceMoved>().hasMoved;

                    // temporarily ignore piece for linecast
                    farCastlePiece.gameObject.layer = LayerMask.NameToLayer("Ignore");

                    // If nothing between king and castle and castle hasn't moved
                    if (!farCastleMoved && !Physics2D.Linecast(kingPosition.Location, farCastlePiece.gameObject.transform.position, LayerMask.GetMask("Default")))
                    {
                        var square = new ChessSquare(kingPosition.Row, kingPosition.Col - 2);
                        castlingMoves.Add(new Move(piece, square));
                    }

                    // Unignore piece
                    farCastlePiece.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }

            // Unignore piece
            kingPiece.layer = LayerMask.NameToLayer("Default");
        }
        return castlingMoves;
    }

    private static List<Move> GetKnightMoves(ChessSquare position, bool turn, GameObject piece)
    {
        List<Move> knightMoves = new();

        for (int i = -2; i < 5; i += 4)
        {
            for (int j = -1; j < 2; j += 2)
            {
                var s1 = new ChessSquare(position.Row + i, position.Col + j);
                var s2 = new ChessSquare(position.Row + j, position.Col + i);

                // If square is empty
                if (Board.FindPieceOnSquare(s1) == null)
                    knightMoves.Add(new Move(piece, s1));

                // If square is occupied by an enemy piece
                else if (Board.IsEnemyPiece(turn, Board.FindPieceOnSquare(s1).gameObject))
                    knightMoves.Add(new Move(piece, s1));

                // If square is empty
                if (Board.FindPieceOnSquare(s2) == null)
                    knightMoves.Add(new Move(piece, s2));

                // If square is occupied by an enemy piece
                else if (Board.IsEnemyPiece(playerTurn, Board.FindPieceOnSquare(s2).gameObject))
                    knightMoves.Add(new Move(piece, s2));
            }
        }
        return knightMoves;
    }
}