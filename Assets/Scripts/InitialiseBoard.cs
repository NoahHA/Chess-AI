using UnityEngine;

/// <summary>
/// Initialises the board at the start of a game
/// </summary>
public class InitialiseBoard : MonoBehaviour
{
    void Start() => Board.GeneratePosition(Board.startingPosition);
}