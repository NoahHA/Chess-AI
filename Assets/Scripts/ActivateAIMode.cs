using UnityEngine;

public class ActivateAIMode : MonoBehaviour
{
    [Tooltip("Whether player is playing against a human or an AI")]
    public bool AIMode;

    public void TurnOnAIMode()
    {
        AIMode = true;
    }

    private void Awake()
    {
        AIMode = false;
    }
}