using UnityEngine;

public class Goblin : QuaiVat
{
    [Header("Movement (Mặt đất)")]
    public float speed = 2.5f;
    
    [Header("Patrol Line (Đi tuần trái/phải)")]
    public float patrolDistance = 4f; 
    private float leftBoundary;
    private float rightBoundary;
    private bool movingRight = true;

    [Header("Detect Player")]
    public Transform player;
    public float detectRange = 5f;
    public float attackRange = 1.2f;

    [Header("Attack")]
    public int damage = 15;
    public float attackCooldown = 1.5f;
    private float attackTimer;
    
    public LayerMask playerLayer;
    private bool facingRight = true;

    // --- PHẦN THÊM MỚI ---
    [Header("Thành phần kết nối")]
    public Animator kiemAnim; // Tham chiếu đến Animator của hiệu ứng kiếm
    // ---------------------

    protected override void Start()
    {
        base.Start(); 

        leftBoundary = transform.position.x - patrolDistance;
        rightBoundary = transform.position.x + patrolDistance;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
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

        if (player == null)
        {
            GroundPatrol();
            return;
        }

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
            GroundPatrol();
        }
    }

    // ================= PATROL (ĐI TUẦN) ================= //

    void GroundPatrol()
    {
        float currentX = transform.position.x;

        if (movingRight && currentX >= rightBoundary)
        {
            movingRight = false;
        }
        else if (!movingRight && currentX <= leftBoundary)
        {
            movingRight = true;
        }

        float dirX = movingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(dirX * speed, rb.linearVelocity.y);

        UpdateDirection(dirX);
    }

    // ================= CHASE (RƯỢT ĐUỔI) ================= //

    void ChasePlayer()
    {
        float dirX = Mathf.Sign(player.position.x - transform.position.x);

        if (Mathf.Abs(player.position.x - transform.position.x) > 0.1f)
        {
            rb.linearVelocity = new Vector2(dirX * speed, rb.linearVelocity.y);
            UpdateDirection(dirX);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // ================= ATTACK ================= //

    void Attack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 

        if (attackTimer > 0) return;

        attackTimer = attackCooldown;

        // Kích hoạt animation của bản thân Goblin
        if (anim != null)
            anim.SetTrigger("TanCong");

        // --- PHẦN THÊM MỚI ---
        // Kích hoạt animation của hiệu ứng kiếm chém
        if (kiemAnim != null)
            kiemAnim.SetTrigger("Chem"); 
        // ---------------------

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            DieuKhienNhanVat p = hitPlayer.GetComponent<DieuKhienNhanVat>();
            if (p != null)
            {
                p.NhanSatThuong(damage);
            }
        }
    }

    // ================= FLIP ================= //

    void UpdateDirection(float dir)
    {
        if (dir > 0 && !facingRight)
        {
            Flip();
        }
        else if (dir < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ================= GIZMOS ================= //

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            new Vector3(transform.position.x - patrolDistance, transform.position.y, 0),
            new Vector3(transform.position.x + patrolDistance, transform.position.y, 0)
        );
    }
}