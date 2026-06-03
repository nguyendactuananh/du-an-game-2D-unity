using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    // Gọi từ nút Restart
    public void OnClickRestart()
    {
        GameManager.instance.Restart();
    }

    // Gọi từ nút Main Menu
    public void OnClickMenu()
    {
        GameManager.instance.GoMenu();
    }
}