using UnityEngine;

public class Boss : QuaiVat
{
    [Header("Movement")]
    public float speed = 2f;

    private bool movingRight = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkDistance = 0.5f;

    [Header("Player Detect")]
    public Transform player;
    public float detectRange = 8f;
    public float attackRange = 2f;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 2f;
    public int damage = 20;
    public float attackCooldown = 2f;

    private float attackTimer;
    private bool isAttacking = false;

    [Header("Boss HP UI")]
    public BossHPBarUI hpUI;

    protected override void Start()
    {
        base.Start();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }

        if (hpUI != null)
        {
            hpUI.SetMaxHP(mauToiDa);
            hpUI.UpdateHP(mauHienTai);
            hpUI.boss = transform;
        }
    }

    void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= detectRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        UpdateAnimation();
    }

    // ================= PATROL =================

    void Patrol()
    {
        if (isAttacking) return;

        float dir = movingRight ? 1 : -1;

        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        bool noGround = !Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            checkDistance,
            groundLayer
        );

        bool wallHit = Physics2D.Raycast(
            groundCheck.position,
            Vector2.right * dir,
            0.3f,
            groundLayer
        );

        if (noGround || wallHit)
        {
            Flip();
        }
    }

    // ================= CHASE =================

    void ChasePlayer()
    {
        if (isAttacking) return;

        float dir = player.position.x > transform.position.x ? 1 : -1;

        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        if (dir > 0 && !movingRight) Flip();
        else if (dir < 0 && movingRight) Flip();
    }

    // ================= ATTACK =================

    void Attack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (attackTimer > 0) return;

        attackTimer = attackCooldown;
        isAttacking = true;

        anim.SetTrigger("TanCong");

        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            LayerMask.GetMask("Player")
        );

        if (hit != null)
        {
            DieuKhienNhanVat p = hit.GetComponent<DieuKhienNhanVat>();
            if (p != null)
            {
                p.NhanSatThuong(damage);
            }
        }

        Invoke(nameof(ResetAttack), 0.8f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    // ================= DAMAGE =================

    public override void NhanSatThuong(int satThuong)
    {
        base.NhanSatThuong(satThuong);

        if (hpUI != null)
        {
            hpUI.UpdateHP(mauHienTai);
        }
    }

    // ================= DIE =================

    protected override void Chet()
    {
        if (isDead) return;

        isDead = true;

        // ẩn HP bar
        if (hpUI != null)
            hpUI.gameObject.SetActive(false);

        // dừng di chuyển
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // tắt collider
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
            coll.enabled = false;

        // animation chết
        if (anim != null)
            anim.SetTrigger("Chet");

        // ❗ biến mất giống enemy sau 2s
        Destroy(gameObject, 2f);
    }

    // ================= FLIP =================

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ================= ANIMATION =================

    void UpdateAnimation()
    {
        anim.SetBool(
            "DiChuyen",
            Mathf.Abs(rb.linearVelocity.x) > 0.1f
        );
    }
}