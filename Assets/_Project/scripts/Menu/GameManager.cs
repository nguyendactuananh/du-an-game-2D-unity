using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scenes")]
    public string gameOverScene = "GameOver";
    public string victoryScene  = "Victory";
    public string menuScene     = "MainMenu";
    public string firstLevelScene = "map_bau_troi_1";

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == menuScene)
        {
            SetupMainMenuButtons();
        }
    }

    private void SetupMainMenuButtons()
    {
        // Tự động tìm Button Play trong scene và gán sự kiện click
        GameObject playObj = GameObject.Find("Play");
        if (playObj != null)
        {
            Button playButton = playObj.GetComponent<Button>();
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(() => LoadScene(firstLevelScene));
                Debug.Log("GameManager: Đã tự động kết nối nút Play!");
            }
        }

        // Tự động tìm Button Exit trong scene và gán sự kiện click
        GameObject exitObj = GameObject.Find("Exit");
        if (exitObj != null)
        {
            Button exitButton = exitObj.GetComponent<Button>();
            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(QuitGame);
                Debug.Log("GameManager: Đã tự động kết nối nút Exit!");
            }
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

    public void QuitGame()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }
}