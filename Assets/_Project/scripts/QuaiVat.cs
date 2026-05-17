using UnityEngine;

public class QuaiVat : MonoBehaviour
{
    [Header("Chỉ số chiến đấu")]
    public int mauToiDa = 50;
    private int mauHienTai;

    [Header("Cấu hình Di chuyển (Tuần tra)")]
    public float tocDo = 2f;
    public bool isFacingRight = false; // Mặc định quái quay mặt sang trái hay phải?
    public Transform diemKiemTra;      // Điểm đặt trước mặt quái vật để soi đường
    public float khoangCachNhin = 0.5f;
    public LayerMask layerMatDat;      // Layer đất để quái biết đâu là vực/tường

    [Header("Cấu hình Đuổi & Đánh (AI)")]
    public Transform nguoiChoi;          // Mục tiêu để rượt đuổi
    public float banKinhPhatHien = 5f;   // Tầm nhìn thấy người chơi
    public float banKinhTanCong = 1.5f;  // Lại gần bao nhiêu thì vung tay đánh?
    public int satThuongDanh = 15;       // Sát thương mỗi cú đánh trúng người chơi
    public float thoiGianHoiDon = 2f;    // Đánh xong phải đợi 2 giây mới đánh tiếp được
    private float demNguocTanCong = 0f;

    [Header("Kết nối")]
    public Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        // Hồi đầy máu khi xuất hiện
        mauHienTai = mauToiDa;
        rb = GetComponent<Rigidbody2D>();

        // Tự động tìm nhân vật chính thông qua Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null) 
        {
            nguoiChoi = playerObj.transform;
        }
    }

    void Update()
    {
        // Liên tục trừ lùi thời gian đếm ngược tấn công
        if (demNguocTanCong > 0)
        {
            demNguocTanCong -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // Nếu quái chết rồi thì ngưng mọi hoạt động suy nghĩ
        if (mauHienTai <= 0) return;

        // Bắt đầu logic AI rượt đuổi và tấn công
        if (nguoiChoi != null)
        {
            // Lấy script của người chơi để kiểm tra xem người chơi còn sống không
            DieuKhienNhanVat scriptNguoiChoi = nguoiChoi.GetComponent<DieuKhienNhanVat>();

            if (scriptNguoiChoi != null && scriptNguoiChoi.mauHienTai > 0)
            {
                // Tính khoảng cách từ quái đến người chơi
                float khoangCachToiNguoiChoi = Vector2.Distance(transform.position, nguoiChoi.position);

                if (khoangCachToiNguoiChoi <= banKinhTanCong)
                {
                    // TRONG TẦM ĐÁNH: Đứng im lại và vung tay
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    
                    if (demNguocTanCong <= 0)
                    {
                        TanCong();
                    }
                }
                else if (khoangCachToiNguoiChoi <= banKinhPhatHien)
                {
                    // TRONG TẦM NHÌN (Nhưng chưa với tới): Rượt đuổi theo!
                    HuongVePhiaNguoiChoi();
                    float vanTocX = isFacingRight ? tocDo : -tocDo;
                    rb.linearVelocity = new Vector2(vanTocX, rb.linearVelocity.y);
                }
                else
                {
                    // NGOÀI TẦM NHÌN: Đi tuần tra bình thường qua lại
                    DiChuyenTuanTra();
                }
            }
            else
            {
                // Người chơi đã chết -> Quay lại đi tuần tra thong thả
                DiChuyenTuanTra();
            }
        }
        else
        {
            // Không tìm thấy object người chơi -> Đi tuần tra
            DiChuyenTuanTra();
        }
    }

    // ================= CÁC HÀM XỬ LÝ HÀNH ĐỘNG ================= //

    private void TanCong()
    {
        // Phát hoạt hình tấn công
        if (anim != null) anim.SetTrigger("TanCong");

        // Gây sát thương thẳng cho người chơi
        if (nguoiChoi != null)
        {
            DieuKhienNhanVat scriptNguoiChoi = nguoiChoi.GetComponent<DieuKhienNhanVat>();
            if (scriptNguoiChoi != null)
            {
                scriptNguoiChoi.NhanSatThuong(satThuongDanh);
            }
        }

        // Đặt lại đồng hồ đếm ngược chờ cú đánh tiếp theo
        demNguocTanCong = thoiGianHoiDon;
    }

    private void HuongVePhiaNguoiChoi()
    {
        // Lật mặt quái vật sao cho luôn nhìn chằm chằm vào người chơi khi đuổi
        if (nguoiChoi.position.x > transform.position.x && !isFacingRight) 
        { 
            LatMat(); 
        }
        else if (nguoiChoi.position.x < transform.position.x && isFacingRight) 
        { 
            LatMat(); 
        }
    }

    private void DiChuyenTuanTra()
    {
        // Cứ thế đi tới trước
        float vanTocX = isFacingRight ? tocDo : -tocDo;
        rb.linearVelocity = new Vector2(vanTocX, rb.linearVelocity.y);

        // Bắn tia laser kiểm tra vực và tường
        if (diemKiemTra != null)
        {
            Vector2 huongNhin = isFacingRight ? Vector2.right : Vector2.left;
            bool chamTuong = Physics2D.Raycast(diemKiemTra.position, huongNhin, khoangCachNhin, layerMatDat);
            bool hetDuong = !Physics2D.Raycast(diemKiemTra.position, Vector2.down, khoangCachNhin, layerMatDat);

            if (chamTuong || hetDuong) 
            { 
                LatMat(); 
            }
        }
    }

    private void LatMat()
    {
        isFacingRight = !isFacingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    // ================= HÀM NHẬN SÁT THƯƠNG TỪ NGƯỜI CHƠI ================= //

    public void NhanSatThuong(int satThuong)
    {
        mauHienTai -= satThuong;
        Debug.Log("Chém trúng! Quái vật còn: " + mauHienTai + " máu.");

        if (mauHienTai <= 0) 
        { 
            Chet(); 
        }
        else 
        { 
            // Chỉ nhăn mặt bị thương nếu còn sống
            if (anim != null) anim.SetTrigger("BiThuong"); 
        }
    }

    private void Chet()
    {
        Debug.Log("Quái vật đã bị tiêu diệt!");
        if (anim != null) anim.SetTrigger("Chet");

        // Dừng mọi lực di chuyển và tắt trọng lực để xác không rơi qua đất
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.bodyType = RigidbodyType2D.Kinematic; 
        }

        // Tắt BoxCollider để người chơi đi xuyên qua xác
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = false;
        }

        // Vô hiệu hóa script này
        this.enabled = false; 

        // Xóa xác quái khỏi màn hình sau 1 giây
        Destroy(gameObject, 1f); 
    }

    // ================= HÀM PHỤ TRỢ (VẼ HÌNH GIZMOS) ================= //

    private void OnDrawGizmosSelected()
    {
        // Vẽ tia laser soi đường
        if (diemKiemTra != null)
        {
            Gizmos.color = Color.cyan;
            Vector2 huongNhin = isFacingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(diemKiemTra.position, huongNhin * khoangCachNhin);
            Gizmos.DrawRay(diemKiemTra.position, Vector2.down * khoangCachNhin);
        }
        
        // Vẽ vòng tròn tầm nhìn (Màu xanh lá) và tầm đánh (Màu đỏ) để dễ căn chỉnh trên Scene
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, banKinhPhatHien);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, banKinhTanCong);
    }
}