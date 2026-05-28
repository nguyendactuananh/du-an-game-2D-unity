using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    
    [Header("Cấu hình Camera")]
    [Tooltip("Độ mượt của camera. Số càng nhỏ camera lướt càng chậm và êm.")]
    public float smoothSpeed = 5f; 
    
    [Tooltip("Độ lệch của camera so với nhân vật (Trục Z luôn phải là -10)")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    void Start()
    {
        // Tự động tìm nhân vật có gắn Tag "Player" khi vừa load màn
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Camera không tìm thấy ai có thẻ Player trong màn này!");
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // 1. Xác định vị trí đích đến (Vị trí nhân vật + độ lệch Z)
            Vector3 targetPosition = player.position + offset;
            
            // 2. Dùng hàm Lerp để tạo hiệu ứng lướt mượt mà từ vị trí hiện tại đến vị trí đích
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            
            // 3. Cập nhật vị trí mới cho Camera
            transform.position = smoothedPosition;
        }
    }
}