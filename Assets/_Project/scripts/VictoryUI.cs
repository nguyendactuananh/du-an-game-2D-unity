using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    public void OnClickNextScene()
    {
        GameManager.instance.LoadScene("long_dat_2");
    }

    public void OnClickMenu()
    {
        GameManager.instance.GoMenu();
    }
}