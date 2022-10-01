using System;
using UnityEditor;
using UnityEngine;

public static class Board
{
    public static string StartingPosition 
    {
        get { return "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"; }
        private set { }
    }

    public static void GeneratePosition(string position)
    {
        ClearBoard();
        ChessSquare currentSquare = new ChessSquare(0, 0);

        foreach (char letter in position)
        {
            if (char.IsDigit(letter))
                currentSquare.Col += (int)char.GetNumericValue(letter) - 1;
            
            else if (letter.Equals('/'))
            {
                currentSquare.Row++;
                currentSquare.Col = 0;
            }
            else
            {
                GameObject piece = GetPieceFromLetter(letter);
                var createdPiece = GameObject.Instantiate(piece, currentSquare.Location, piece.transform.rotation);
                createdPiece.tag = "Piece";
                if (currentSquare.Col < 7) { currentSquare.Col++; }
            }
        };
    }

    public static void ClearBoard()
    {
        var clones = GameObject.FindGameObjectsWithTag("Piece");

        foreach (var clone in clones)
            GameObject.Destroy(clone);
    }

    public static GameObject GetPieceFromLetter(char letter)
    {
        int index;

        // uppercase = white, lowercase = black
        if (letter.Equals('p')) { index = 0; }
        else if (letter.Equals('r')) { index = 1; }
        else if (letter.Equals('n')) { index = 2; }
        else if (letter.Equals('b')) { index = 3; }
        else if (letter.Equals('q')) { index = 4; }
        else if (letter.Equals('k')) { index = 5; }
        else if (letter.Equals('P')) { index = 6; }
        else if (letter.Equals('R')) { index = 7; }
        else if (letter.Equals('N')) { index = 8; }
        else if (letter.Equals('B')) { index = 9; }
        else if (letter.Equals('Q')) { index = 10; }
        else if (letter.Equals('K')) { index = 11; }
        else { throw new ArgumentException($"Letter not recognized: {letter}", nameof(letter)); }

        return PieceManager.Instance.pieces[index];
    }
}