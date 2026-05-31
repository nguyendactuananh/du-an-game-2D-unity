using UnityEngine;

public class FlyOpp : QuaiVat
{
    [Header("Stats")]
    public int maxHP = 20;
    private int currentHP;

    [Header("Movement (Flying)")]
    public float speed = 2.5f;

    [Header("Patrol Area")]
    // Vì quái bay nên tuần tra theo bán kính hình tròn sẽ tự nhiên hơn
    public float patrolRadius = 3f;
    private Vector2 startPosition;
    private Vector2 patrolTarget;
    private float patrolTimer;
    public float changeTargetCooldown = 2f;

    [Header("Player Detect")]
    public Transform player;
    public float detectRange = 5f;
    public float attackRange = 1.5f;

    [Header("Attack Config")]
    public int damage = 12;
    public float attackCooldown = 2f;
    private float attackTimer;
    public LayerMask playerLayer;

    private bool facingRight = true;

    // ================= START ================= //
    protected override void Start()
    {
        base.Start();

        currentHP = maxHP;
        startPosition = transform.position;
        patrolTarget = GetRandomPatrolPoint();

        // Tự động tìm Player trong Scene bằng Tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }
    }

    // ================= UPDATE ================= //
    void Update()
    {
        if (isDead) return;

        // Giảm thời gian hồi chiêu liên tục
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (player == null)
        {
            Patrol();
            return;
        }

        // Tính khoảng cách tới Player
        float distance = Vector2.Distance(transform.position, player.position);

        // 1. Trạng thái Tấn công
        if (distance <= attackRange)
        {
            Attack();
        }
        // 2. Trạng thái Đuổi theo Player
        else if (distance <= detectRange)
        {
            ChasePlayer();
        }
        // 3. Trạng thái Bay tuần tra mặc định
        else
        {
            Patrol();
        }
    }

    // ================= PATROL (BAY TUẦN TRA) ================= //
    void Patrol()
    {
        patrolTimer += Time.fixedDeltaTime;

        // Cứ sau một khoảng thời gian, đổi một điểm bay ngẫu nhiên mới
        if (patrolTimer >= changeTargetCooldown || Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            patrolTarget = GetRandomPatrolPoint();
            patrolTimer = 0;
        }

        // Di chuyển hướng về điểm tuần tra mục tiêu
        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * (speed * 0.7f); // Bay tuần tra chậm hơn khi đuổi khách

        UpdateDirection(rb.linearVelocity.x);
    }

    Vector2 GetRandomPatrolPoint()
    {
        // Lấy một điểm ngẫu nhiên xung quanh vị trí xuất phát gốc
        return startPosition + Random.insideUnitCircle * patrolRadius;
    }

    // ================= CHASE (ĐUỔI THEO PLAYER) ================= //
    void ChasePlayer()
    {
        // Tính toán hướng bay thẳng tới vị trí của Player (cả trục X và Y)
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        
        rb.linearVelocity = direction * speed;

        UpdateDirection(rb.linearVelocity.x);
    }

    // ================= ATTACK (TẤN CÔNG) ================= //
    void Attack()
    {
        // Khựng lại trên không một chút khi tấn công
        rb.linearVelocity = Vector2.zero;

        if (attackTimer > 0) return;

        attackTimer = attackCooldown;

        // Kích hoạt Trigger "Attack" trong Animator
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        // Kiểm tra xem Player có còn nằm trong tầm đánh không
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            // Gọi script điều khiển nhân vật để trừ máu (Thay đổi tên class script Player của bạn nếu cần)
            DieuKhienNhanVat p = hitPlayer.GetComponent<DieuKhienNhanVat>();
            if (p != null)
            {
                p.NhanSatThuong(damage);
            }
        }
    }

    // ================= TAKE DAMAGE (NHẬN SÁT THƯƠNG) ================= //
    public void TakeHit(int damageTaken)
    {
        if (isDead) return;

        currentHP -= damageTaken;

        // Nếu còn sống thì có thể thêm hiệu ứng giật mình (nếu muốn), ở đây xử lý chết luôn khi hết HP
        if (currentHP <= 0)
        {
            Die();
        }
    }

    // ================= DIE (BỊ TIÊU DIỆT) ================= //
    void Die()
    {
        isDead = true;

        // Kích hoạt Trigger "Die" trong Animator để chạy hoạt ảnh rơi rụng/biến mất
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // Tắt vật lý để quái không va chạm với Player nữa
        rb.linearVelocity = Vector2.zero;
        
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null) coll.enabled = false;

        rb.simulated = false;
        this.enabled = false;

        // Biến mất sau khi diễn xong animation chết (ví dụ sau 0.5 giây tùy độ dài animation của bạn)
        Destroy(gameObject, 0.5f);
    }

    // ================= FLIP (QUAY MẶT) ================= //
    void UpdateDirection(float dirX)
    {
        if (dirX > 0.1f && !facingRight)
        {
            Flip();
        }
        else if (dirX < -0.1f && facingRight)
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

    // ================= GIZMOS (VẼ VÒNG TRÒN TRỰC QUAN TRÊN SCENE) ================= //
    void OnDrawGizmosSelected()
    {
        // Vòng tròn vùng tuần tra (Màu xanh dương)
        Gizmos.color = Color.blue;
        if (Application.isPlaying)
            Gizmos.DrawWireSphere(startPosition, patrolRadius);
        else
            Gizmos.DrawWireSphere(transform.position, patrolRadius);

        // Vòng tròn phát hiện Player (Màu đỏ)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Vòng tròn tầm đánh (Màu vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
