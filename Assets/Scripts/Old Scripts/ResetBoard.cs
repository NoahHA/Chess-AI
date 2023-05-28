using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resets the rotations of all pieces and camera on game start.
/// Not sure why this is necessary but it is.
/// </summary>
public class ResetBoard : MonoBehaviour
{
    void Start()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        List<GameObject> pieces = PieceManager.Instance.pieces;

        // Reset camera rotation
        camera.transform.eulerAngles = new Vector3(0, 0, 0);
        // Reset piece rotations
        foreach (GameObject piece in pieces)
            piece.transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
