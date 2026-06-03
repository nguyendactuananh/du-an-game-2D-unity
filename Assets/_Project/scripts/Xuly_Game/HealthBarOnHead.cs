using UnityEngine;
using UnityEngine.UI;

public class HealthBarOnHead : MonoBehaviour
{
    [Header("Kết nối dữ liệu")]
    public DieuKhienNhanVat player; // Kéo thả script nhân vật vào đây
    public Image fillImage;        // Kéo thả ảnh "Fill" màu xanh vào đây

    private Vector3 initialLocalScale;

    void Start()
    {
        // Lưu lại kích thước Local Tỷ lệ ban đầu của Canvas thanh máu
        initialLocalScale = transform.localScale;

        // Nếu chưa kéo thả Player, tự động tìm kiếm ở lớp cha
        if (player == null)
        {
            player = GetComponentInParent<DieuKhienNhanVat>();
        }
    }

    void Update()
    {
        if (player != null && fillImage != null)
        {
            // Tính toán phần trăm máu dựa trên chỉ số sinh tồn của Player
            // Ép kiểu (float) để phép chia không bị trả về số 0 tròn trĩnh
            float phanTramMau = (float)player.mauHienTai / player.mauToiDa;

            // Cập nhật lượng lấp đầy của ảnh (giá trị chuẩn từ 0.0 đến 1.0)
            fillImage.fillAmount = phanTramMau;
        }
    }

    void LateUpdate()
    {
        // KHẮC PHỤC LỖI NGƯỢC ĐẢO CHIỀU KHI NHÂN VẬT LẬT (FLIP)
        if (transform.parent != null)
        {
            Vector3 currentScale = transform.localScale;

            // Nếu cha bị âm scale (nhân vật quay sang trái), 
            // con cũng tự đổi dấu localScale để triệt tiêu góc lật (luôn giữ hướng chuẩn)
            if (transform.parent.localScale.x < 0)
            {
                currentScale.x = -Mathf.Abs(initialLocalScale.x);
            }
            else
            {
                currentScale.x = Mathf.Abs(initialLocalScale.x);
            }

            transform.localScale = currentScale;
        }
    }
}