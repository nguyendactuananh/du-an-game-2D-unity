using UnityEngine;

public class QuaiVat : MonoBehaviour
{
    [Header("Chỉ số chiến đấu")]
    public int mauToiDa = 50;
    private int mauHienTai;

    [Header("Cấu hình Di chuyển (AI)")]
    public float tocDo = 2f;
    public bool isFacingRight = false; // Mặc định quái quay mặt sang trái hay phải?
    public Transform diemKiemTra;      // Điểm đặt trước mặt quái vật để soi đường
    public float khoangCachNhin = 0.5f;
    public LayerMask layerMatDat;      // Layer đất để quái biết đâu là vực/tường

    [Header("Kết nối")]
    public Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        mauHienTai = mauToiDa;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        DiChuyenTuanTra();
    }

    private void DiChuyenTuanTra()
    {
        // 1. Quái vật tự động đi tới trước
        float vanTocX = isFacingRight ? tocDo : -tocDo;
        rb.linearVelocity = new Vector2(vanTocX, rb.linearVelocity.y);

        // 2. Bắn tia laser nhỏ từ điểm kiểm tra để xem có vực hoặc tường không
        if (diemKiemTra != null)
        {
            Vector2 huongNhin = isFacingRight ? Vector2.right : Vector2.left;

            // Kiểm tra có chạm tường không
            bool chamTuong = Physics2D.Raycast(diemKiemTra.position, huongNhin, khoangCachNhin, layerMatDat);
            
            // Kiểm tra dưới chân có đất không (nếu không có nghĩa là vực sâu)
            bool hetDuong = !Physics2D.Raycast(diemKiemTra.position, Vector2.down, khoangCachNhin, layerMatDat);

            // Nếu đập mặt vào tường hoặc đi tới mép vực -> Quay đầu
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

    // Hàm này sẽ được vung kiếm của Player gọi tới khi chém trúng
    public void NhanSatThuong(int satThuong)
    {
        mauHienTai -= satThuong;
        Debug.Log("Chém trúng! Quái vật còn: " + mauHienTai + " máu.");

        if (anim != null) anim.SetTrigger("BiThuong");

        if (mauHienTai <= 0)
        {
            Chet();
        }
    }

    private void Chet()
    {
        Debug.Log("Quái vật đã bị tiêu diệt!");
        if (anim != null) anim.SetTrigger("Chet");

        // Vô hiệu hóa va chạm và logic để quái không đi lại hay nhận sát thương nữa
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false; 

        // Xóa xác quái khỏi Scene sau 1 giây (nếu có animation chết thì thời gian này để animation chạy xong)
        Destroy(gameObject, 1f); 
    }

    // Vẽ tia laser ra màn hình Scene để bạn dễ căn chỉnh
    private void OnDrawGizmosSelected()
    {
        if (diemKiemTra != null)
        {
            Gizmos.color = Color.cyan;
            Vector2 huongNhin = isFacingRight ? Vector2.right : Vector2.left;
            // Vẽ tia nhìn phía trước (tường)
            Gizmos.DrawRay(diemKiemTra.position, huongNhin * khoangCachNhin);
            // Vẽ tia nhìn xuống dưới (vực)
            Gizmos.DrawRay(diemKiemTra.position, Vector2.down * khoangCachNhin);
        }
    }
}