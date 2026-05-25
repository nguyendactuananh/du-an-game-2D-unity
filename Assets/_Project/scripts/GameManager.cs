using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scenes")]
    public string gameOverScene = "GameOver";
    public string victoryScene  = "Victory";
    public string menuScene     = "MainMenu";

    private string lastGameScene = "long_dat_1";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        lastGameScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(gameOverScene);
    }

    public void Victory()
    {
        SceneManager.LoadScene(victoryScene);
    }

    public void Restart()
    {
        SceneManager.LoadScene(lastGameScene);
    }

    public void GoMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}