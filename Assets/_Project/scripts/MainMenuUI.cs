using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Cấu hình chuyển màn")]
    [Tooltip("Tên Scene của màn chơi đầu tiên khi bấm Play")]
    public string firstLevelScene = "map_bau_troi_1";

    // Gọi khi bấm nút Play
    public void OnClickPlay()
    {
        // Sử dụng GameManager để chuyển màn nếu GameManager được cấu hình
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadScene(firstLevelScene);
        }
        else
        {
            // Nếu không có GameManager trong Scene, dùng trực tiếp SceneManager
            SceneManager.LoadScene(firstLevelScene);
        }
    }

    // Gọi khi bấm nút Quit (nếu có)
    public void OnClickQuit()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }
}
