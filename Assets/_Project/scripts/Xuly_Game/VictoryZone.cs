using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.Victory();
            }
            else
            {
                // Dự phòng nếu chạy thử trực tiếp scene màn chơi mà không có GameManager
                UnityEngine.SceneManagement.SceneManager.LoadScene("Victory");
            }
        }
    }

    // Thấy vùng trigger trong Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawCube(transform.position, col.bounds.size);
    }
}