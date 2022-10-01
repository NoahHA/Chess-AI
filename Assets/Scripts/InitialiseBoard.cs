using UnityEngine;

public class InitialiseBoard : MonoBehaviour
{
    void Start() => Board.GeneratePosition(Board.StartingPosition);
}