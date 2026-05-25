using UnityEngine;

public class QuaiVat : MonoBehaviour
{
    [Header("Chỉ số chiến đấu")]
    public int mauToiDa = 50;

    protected int mauHienTai;

    protected Rigidbody2D rb;
    protected Animator anim;

    protected bool isDead = false;

    protected virtual void Start()
    {
        mauHienTai = mauToiDa;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // ================= NHẬN SÁT THƯƠNG =================

    public virtual void NhanSatThuong(int satThuong)
    {
        if (isDead) return;

        mauHienTai -= satThuong;

        Debug.Log(gameObject.name + " còn: " + mauHienTai + " máu.");

        if (mauHienTai <= 0)
        {
            Chet();
        }
        else
        {
            if (anim != null)
                anim.SetTrigger("BiThuong");
        }
    }

    // ================= CHẾT =================

    protected virtual void Chet()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log(gameObject.name + " đã bị tiêu diệt!");

        // Dừng vật lý
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Tắt collider
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = false;
        }

        // Animation chết
        if (anim != null)
            anim.SetTrigger("Chet");

        // ⚠️ CHỈ DESTROY nếu KHÔNG PHẢI BOSS
        Destroy(gameObject, 2f);
    }

    // ================= CHECK =================

    public bool DaChet()
    {
        return isDead;
    }
}