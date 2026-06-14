using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance?.PlayVictorySound();

            Invoke(nameof(LoadVictoryScene), 1f);
        }
    }

    private void LoadVictoryScene()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.Victory();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Victory");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            Gizmos.DrawCube(transform.position, col.bounds.size);
    }
}