using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Tooltip("Lượng máu vật phẩm này sẽ hồi phục")]
    public int healthAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DieuKhienNhanVat playerScript =
                collision.GetComponent<DieuKhienNhanVat>();

            if (playerScript != null)
            {
                // Phát âm thanh nhặt vật phẩm
                AudioManager.Instance?.PlayPickupSound();

                // Hồi máu
                playerScript.HoiMau(healthAmount);

                // Xóa vật phẩm
                Destroy(gameObject);
            }
        }
    }
}