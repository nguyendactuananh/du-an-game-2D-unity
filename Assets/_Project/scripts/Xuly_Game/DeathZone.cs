using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        if (other.CompareTag("Player"))
        {
            // Lấy script điều khiển nhân vật
            DieuKhienNhanVat player = other.GetComponent<DieuKhienNhanVat>();
            
            if (player != null)
            {
                Debug.Log("Người chơi rơi xuống hố tử thần!");
                // Gây sát thương cực lớn để người chơi chết ngay lập tức
                player.NhanSatThuong(9999);
            }
        }
    }

    // Vẽ vùng DeathZone màu đỏ nhạt trong Scene view để bạn dễ thấy và căn chỉnh
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawCube(transform.position, col.bounds.size);
        }
    }
}
