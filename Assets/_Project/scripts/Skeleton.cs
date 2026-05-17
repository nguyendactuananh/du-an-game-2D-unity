using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [Header("Move")]
    public float speed = 2f;

    [Header("Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Stats")]
    public int hp = 3;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        Patrol();
        UpdateAnim();
    }

    // ================= PATROL AI =================
    void Patrol()
    {
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

        // CHECK MẶT ĐẤT PHÍA TRƯỚC
        RaycastHit2D groundInfo =
            Physics2D.Raycast(
                groundCheck.position,
                Vector2.down,
                0.2f,
                groundLayer
            );

        // CHECK TƯỜNG PHÍA TRƯỚC
        RaycastHit2D wallInfo =
            Physics2D.Raycast(
                transform.position,
                Vector2.right * Mathf.Sign(speed),
                0.5f,
                groundLayer
            );

        // nếu mất đất hoặc gặp tường → quay đầu
        if (!groundInfo || wallInfo)
        {
            Flip();
        }
    }

    // ================= FLIP =================
    void Flip()
    {
        speed *= -1;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ================= ANIMATION =================
    void UpdateAnim()
    {
        anim.SetBool("DiChuyen", Mathf.Abs(speed) > 0);
    }

    // ================= DAMAGE =================
    public void TakeHit(int damage)
    {
        if (isDead) return;

        hp -= damage;

        anim.SetTrigger("BiThuong");

        if (hp <= 0)
        {
            Die();
        }
    }

    // ================= DIE =================
    void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("Chet");

        Destroy(gameObject, 2f);
    }
}