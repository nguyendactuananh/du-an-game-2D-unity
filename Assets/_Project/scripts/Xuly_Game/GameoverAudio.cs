using UnityEngine;

public class GameOverAudio : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance?.PlayGameOverSound();
    }
}