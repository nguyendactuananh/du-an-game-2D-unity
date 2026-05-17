using UnityEngine;
using UnityEngine.InputSystem; 

public class DieuKhienNhanVat : MonoBehaviour
{
    [Header("Chỉ số Sinh Tồn")]
    public int mauToiDa = 100;
    public int mauHienTai;

    [Header("Cấu hình di chuyển")]
    public float tocDo = 5f;
    public float lucNhay = 25f; 
    public bool isFacingRight = true; 

    [Header("Cấu hình mặt đất")]
    public Transform checkDat;       // Transform đặt dưới chân nhân vật
    public float banKinhCheck = 0.2f; // Bán kính vòng tròn kiểm tra
    public LayerMask layerMatDat;    // Chỉ định layer nào là đất
    private bool isGrounded;         // Cờ xác nhận đang đứng trên đất

    [Header("Cấu hình Tấn công")]
    public Transform diemTanCong;        // Điểm phát ra đòn chém 
    public float banKinhTanCong = 0.5f;  // Độ rộng của đòn chém
    public LayerMask layerQuaiVat;       // Chỉ định layer nào là quái vật
    public int satThuongVukhi = 20;      // Sát thương mỗi nhát chém

    [Header("Thành phần kết nối")]
    public Animator anim;                // Hoạt hình của người chơi
    public Animator kiemAnim;            // Hoạt hình của thanh kiếm
    private Rigidbody2D rb;
    
    // Biến lưu trữ
    private float moveInput; 

    void Start()
    {
        // Khởi tạo đầy máu khi bắt đầu game
        mauHienTai = mauToiDa;
        
        // Lấy Component tự động
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Nếu đã chết thì không cho phép điều khiển nữa, dừng các xử lý bên dưới
        if (mauHienTai <= 0) return;

        // Phân chia logic rõ ràng theo từng hàm
        DocThongTinBanPhim();
        KiemTraChamDat();
        XuLyNhay();
        XuLyLatNhanVat();
        CaiDatHoatHinh();
        XuLyTanCong();
    }

    void FixedUpdate()
    {
        // Nếu đã chết thì không di chuyển
        if (mauHienTai <= 0) return;

        // FixedUpdate chuyên dùng để tính toán vật lý (Rigidbody)
        DiChuyenNhanVat();
    }

    // ================= CÁC HÀM XỬ LÝ CHỨC NĂNG ================= //

    private void DocThongTinBanPhim()
    {
        moveInput = 0f;

        // An toàn kiểm tra xem có thiết bị bàn phím không
        if (Keyboard.current != null)
        {
            // Di chuyển trái phải bằng A/D hoặc Mũi tên
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                moveInput = -1f; // Trục X âm
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                moveInput = 1f;  // Trục X dương
            }
        }
    }

    private void DiChuyenNhanVat()
    {
        // Cập nhật vận tốc ngang, giữ nguyên vận tốc rơi tự do dọc (Y)
        rb.linearVelocity = new Vector2(moveInput * tocDo, rb.linearVelocity.y);
    }

    private void KiemTraChamDat()
    {
        // Vẽ một vòng tròn nhỏ dưới chân để xem có chạm vào Layer mặt đất không
        if (checkDat != null)
        {
            isGrounded = Physics2D.OverlapCircle(checkDat.position, banKinhCheck, layerMatDat);
        }
    }

    private void XuLyNhay()
    {
        if (Keyboard.current != null && isGrounded)
        {
            // Nhảy khi bấm Space hoặc Mũi tên lên và nhân vật đang đứng trên đất
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, lucNhay);
                
                if (anim != null)
                {
                    anim.SetTrigger("Nhay"); 
                }
            }
        }
    }

    private void XuLyLatNhanVat()
    {
        // Lật mặt nhân vật dựa trên hướng di chuyển và trạng thái lật hiện tại
        if ((moveInput > 0 && !isFacingRight) || (moveInput < 0 && isFacingRight))
        {
            ThucHienLat();
        }
    }

    private void ThucHienLat()
    {
        isFacingRight = !isFacingRight;

        // Đảo ngược trục X của scale để lật hình ảnh mượt mà
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    private void CaiDatHoatHinh()
    {
        if (anim != null)
        {
            // Tham số tốc độ di chuyển (luôn dương)
            anim.SetFloat("dichuyen", Mathf.Abs(moveInput)); 
            
            // Tham số rớt/nhảy (nếu có dùng Blend Tree cho trục Y)
            anim.SetFloat("vanTocY", rb.linearVelocity.y);
            anim.SetBool("trenDat", isGrounded);
        }
    }

    private void XuLyTanCong()
    {
        // Lắng nghe phím J để tấn công
        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            // 1. Kích hoạt hoạt hình của NHÂN VẬT
            if (anim != null)
            {
                anim.SetTrigger("TanCong"); 
            }

            // 2. Kích hoạt hoạt hình của THANH KIẾM 
            if (kiemAnim != null)
            {
                kiemAnim.SetTrigger("Chem"); 
            }

            // 3. Thực hiện trừ máu quái vật
            GaySatThuong();
        }
    }

    private void GaySatThuong()
    {
        // Đảm bảo đã có điểm tấn công thì mới quét sát thương
        if (diemTanCong != null)
        {
            // Quét vòng tròn để tìm tất cả quái vật nằm trong tầm chém
            Collider2D[] cacKeDichTrungDon = Physics2D.OverlapCircleAll(diemTanCong.position, banKinhTanCong, layerQuaiVat);

            // Gây sát thương cho từng con quái vật trúng đòn
            foreach(Collider2D keDich in cacKeDichTrungDon)
            {
                // Lấy script QuaiVat gắn trên kẻ địch
                QuaiVat quaiVat = keDich.GetComponent<QuaiVat>();
                
                // Đảm bảo kẻ địch có script này thì mới gọi hàm nhận sát thương
                if (quaiVat != null)
                {
                    quaiVat.NhanSatThuong(satThuongVukhi);
                }
            }
        }
    }

    // ================= HÀM BỊ QUÁI VẬT ĐÁNH ================= //
    
    public void NhanSatThuong(int satThuong)
    {
        mauHienTai -= satThuong;
        Debug.Log("Người chơi bị đánh! Máu còn: " + mauHienTai);

        if (mauHienTai <= 0)
        {
            Chet();
        }
        else
        {
            if (anim != null) anim.SetTrigger("BiThuong");
        }
    }

    private void Chet()
    {
        Debug.Log("Game Over! Người chơi đã gục ngã.");
        if (anim != null) anim.SetTrigger("Chet");

        // Dừng mọi lực di chuyển đang có
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Tắt BoxCollider2D để quái không đánh vào cái xác nữa
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = false;
        }
        
        // Tắt script này đi để người chơi không thể bấm phím gì nữa
        this.enabled = false; 
    }

    // ================= HÀM PHỤ TRỢ (VẼ HÌNH GIZMOS) ================= //
    
    // Hàm phụ trợ giúp vẽ vòng tròn kiểm tra đất trong màn hình Scene để dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn dưới chân (màu đỏ)
        if (checkDat != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkDat.position, banKinhCheck);
        }

        // Vẽ vòng tròn tầm đánh của kiếm (màu vàng)
        if (diemTanCong != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(diemTanCong.position, banKinhTanCong);
        }
    }
}