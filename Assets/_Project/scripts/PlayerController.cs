using UnityEngine;
using UnityEngine.InputSystem;

public class DieuKhienNhanVat : MonoBehaviour
{
    [Header("Chỉ số Sinh Tồn")]
    public int mauToiDa = 100;

    public int mauHienTai;

    private bool isDead = false;

    [Header("Cấu hình di chuyển")]
    public float tocDo = 5f;

    public float lucNhay = 25f;

    public bool isFacingRight = true;

    [Header("Cấu hình mặt đất")]
    public Transform checkDat;

    public float banKinhCheck = 0.2f;

    public LayerMask layerMatDat;

    private bool isGrounded;

    [Header("Cấu hình Tấn công")]
    public Transform diemTanCong;

    public float banKinhTanCong = 0.5f;

    public LayerMask layerQuaiVat;

    public int satThuongVukhi = 20;

    [Header("Cooldown Attack")]
    public float attackCooldown = 0.4f;

    private float attackTimer;

    [Header("Thành phần kết nối")]
    public Animator anim;

    public Animator kiemAnim;

    private Rigidbody2D rb;

    // Input
    private float moveInput;

    // ================= START ================= //

    void Start()
    {
        mauHienTai = mauToiDa;

        rb = GetComponent<Rigidbody2D>();
    }

    // ================= UPDATE ================= //

    void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;

        DocThongTinBanPhim();

        KiemTraChamDat();

        XuLyNhay();

        XuLyLatNhanVat();

        CaiDatHoatHinh();

        XuLyTanCong();
    }

    // ================= FIXED UPDATE ================= //

    void FixedUpdate()
    {
        if (isDead) return;

        DiChuyenNhanVat();
    }

    // ================= INPUT ================= //

    private void DocThongTinBanPhim()
    {
        moveInput = 0f;

        if (Keyboard.current != null)
        {
            // Đi trái
            if (
                Keyboard.current.aKey.isPressed ||
                Keyboard.current.leftArrowKey.isPressed
            )
            {
                moveInput = -1f;
            }

            // Đi phải
            else if (
                Keyboard.current.dKey.isPressed ||
                Keyboard.current.rightArrowKey.isPressed
            )
            {
                moveInput = 1f;
            }
        }
    }

    // ================= MOVE ================= //

    private void DiChuyenNhanVat()
    {
        rb.linearVelocity =
            new Vector2(
                moveInput * tocDo,
                rb.linearVelocity.y
            );
    }

    // ================= GROUND CHECK ================= //

    private void KiemTraChamDat()
    {
        if (checkDat != null)
        {
            isGrounded =
                Physics2D.OverlapCircle(
                    checkDat.position,
                    banKinhCheck,
                    layerMatDat
                );
        }
    }

    // ================= JUMP ================= //

    private void XuLyNhay()
    {
        if (Keyboard.current == null) return;

        if (!isGrounded) return;

        if (
            Keyboard.current.spaceKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame
        )
        {
            rb.linearVelocity =
                new Vector2(
                    rb.linearVelocity.x,
                    lucNhay
                );

            if (anim != null)
            {
                anim.SetTrigger("Nhay");
            }
        }
    }

    // ================= FLIP ================= //

    private void XuLyLatNhanVat()
    {
        if (
            (moveInput > 0 && !isFacingRight) ||
            (moveInput < 0 && isFacingRight)
        )
        {
            ThucHienLat();
        }
    }

    private void ThucHienLat()
    {
        isFacingRight = !isFacingRight;

        Vector3 currentScale =
            transform.localScale;

        currentScale.x *= -1;

        transform.localScale =
            currentScale;
    }

    // ================= ANIMATION ================= //

    private void CaiDatHoatHinh()
    {
        if (anim != null)
        {
            anim.SetFloat(
                "dichuyen",
                Mathf.Abs(moveInput)
            );

            anim.SetFloat(
                "vanTocY",
                rb.linearVelocity.y
            );

            anim.SetBool(
                "trenDat",
                isGrounded
            );
        }
    }

    // ================= ATTACK ================= //

    private void XuLyTanCong()
    {
        if (Keyboard.current == null) return;

        // Cooldown
        if (attackTimer > 0) return;

        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            attackTimer = attackCooldown;

            // Animation player
            if (anim != null)
            {
                anim.SetTrigger("TanCong");
            }

            // Animation kiếm
            if (kiemAnim != null)
            {
                kiemAnim.SetTrigger("Chem");
            }

            // Damage
            GaySatThuong();
        }
    }

    private void GaySatThuong()
    {
        if (diemTanCong == null) return;

        // Quét tất cả enemy trong layer QuaiVat
        Collider2D[] cacKeDichTrungDon =
            Physics2D.OverlapCircleAll(
                diemTanCong.position,
                banKinhTanCong,
                layerQuaiVat
            );

        foreach (Collider2D keDich in cacKeDichTrungDon)
        {
            // Lấy script lớp CHA
            QuaiVat quaiVat =
                keDich.GetComponent<QuaiVat>();

            if (quaiVat != null)
            {
                quaiVat.NhanSatThuong(
                    satThuongVukhi
                );
            }
        }
    }

    // ================= TAKE DAMAGE ================= //

    public void NhanSatThuong(int satThuong)
    {
        if (isDead) return;

        mauHienTai -= satThuong;

        Debug.Log(
            "Người chơi bị đánh! Máu còn: "
            + mauHienTai
        );

        // Animation hit
        if (anim != null)
        {
            anim.SetTrigger("BiThuong");
        }

        if (mauHienTai <= 0)
        {
            Chet();
        }
    }

    // ================= HOI MAU (HEAL) ================= //

    public void HoiMau(int luongMauHoi)
    {
        if (isDead) return; // Nếu đã chết thì không cho hồi máu nữa

        mauHienTai += luongMauHoi;
        
        // Đảm bảo máu không vượt quá mức tối đa
        if (mauHienTai > mauToiDa)
        {
            mauHienTai = mauToiDa;
        }
        
        Debug.Log("Đã hồi máu! Máu hiện tại: " + mauHienTai);
        // Tại đây bạn có thể gọi thêm code cập nhật thanh máu UI nếu có
    }

    // ================= DIE ================= //

    private void Chet()
    {
        Debug.Log("Game Over!");

        isDead = true; // Đánh dấu trạng thái đã chết để khóa mọi thao tác

        if (anim != null)
            anim.SetTrigger("Chet");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
            coll.enabled = false;

        this.enabled = false;

        Invoke(nameof(GoGameOver), 2f);
    }

    private void GoGameOver()
    {
        GameManager.instance.GameOver();
    }

    // ================= GIZMOS ================= //

    private void OnDrawGizmosSelected()
    {
        // Ground Check
        if (checkDat != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                checkDat.position,
                banKinhCheck
            );
        }

        // Attack Range
        if (diemTanCong != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(
                diemTanCong.position,
                banKinhTanCong
            );
        }
    }
}