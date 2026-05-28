using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện này bắt buộc phải có để chuyển Scene

public class LevelTransition : MonoBehaviour
{
    [Header("Cấu hình chuyển màn")]
    [Tooltip("Gõ chính xác tên Scene bạn muốn chuyển tới (VD: mat_dat_level1)")]
    public string nextSceneName;

    // Hàm này tự động chạy khi có thứ gì đó chạm vào vùng Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem thứ vừa chạm vào có phải là Player không
        if (collision.CompareTag("Player"))
        {
            // Nếu đúng là Player, ra lệnh tải Scene mới
            Debug.Log("Đã chạm vào cửa! Đang chuyển sang màn: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}