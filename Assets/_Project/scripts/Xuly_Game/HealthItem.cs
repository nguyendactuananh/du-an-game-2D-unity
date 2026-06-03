using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Tooltip("Lượng máu vật phẩm này sẽ hồi phục")]
    public int healthAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Kiểm tra xem người chạm vào có phải là Player không
        if (collision.CompareTag("Player"))
        {
            // 2. Tìm script DieuKhienNhanVat trên người chơi đó
            // Lưu ý: Thay "DieuKhienNhanVat" bằng đúng tên class script nhân vật của bạn
            DieuKhienNhanVat playerScript = collision.GetComponent<DieuKhienNhanVat>();

            if (playerScript != null)
            {
                // 3. Gọi hàm bơm máu
                playerScript.HoiMau(healthAmount);

                // 4. Tiêu hủy vật phẩm khỏi Scene sau khi ăn xong
                Destroy(gameObject);
            }
        }
    }
}