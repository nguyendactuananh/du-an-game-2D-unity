using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 50;
    private int currentHP;

    [Header("Movement")]
    public float speed = 2f;
    private bool movingRight = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkDistance = 0.5f;

    [Header("Player Detect")]
    public Transform player;
    public float detectRange = 5f;
    public float attackRange = 1.5f;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 0.8f;
    public LayerMask playerLayer;
    public int damage = 10;
    public float attackCooldown = 2f;

    private float attackTimer;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentHP = maxHP;

        GameObject p =
            GameObject.FindGameObjectWithTag("Player");

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
            Patrol();
            return;
        }

        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

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

    void Patrol()
    {
        float dir = movingRight ? 1 : -1;

        rb.linearVelocity =
            new Vector2(
                dir * speed,
                rb.linearVelocity.y
            );

        bool noGround =
            !Physics2D.Raycast(
                groundCheck.position,
                Vector2.down,
                checkDistance,
                groundLayer
            );

        bool wallHit =
            Physics2D.Raycast(
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

    void ChasePlayer()
    {
        if (isAttacking) return;

        float dir =
            player.position.x > transform.position.x
            ? 1
            : -1;

        rb.linearVelocity =
            new Vector2(
                dir * speed,
                rb.linearVelocity.y
            );

        if (dir > 0 && !movingRight)
        {
            Flip();
        }
        else if (dir < 0 && movingRight)
        {
            Flip();
        }
    }

    void Attack()
    {
        rb.linearVelocity =
            new Vector2(
                0,
                rb.linearVelocity.y
            );

        if (attackTimer > 0) return;

        attackTimer = attackCooldown;

        isAttacking = true;

        anim.SetTrigger("TanCong");

        Collider2D hitPlayer =
            Physics2D.OverlapCircle(
                attackPoint.position,
                attackRadius,
                playerLayer
            );

        if (hitPlayer != null)
        {
            DieuKhienNhanVat p =
                hitPlayer.GetComponent<DieuKhienNhanVat>();

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

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void UpdateAnimation()
    {
        anim.SetBool(
            "DiChuyen",
            Mathf.Abs(rb.linearVelocity.x) > 0.1f
        );
    }

    public void TakeHit(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        anim.SetTrigger("BiThuong");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        GetComponent<Collider2D>().enabled = false;

        anim.SetTrigger("Chet");

        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                groundCheck.position,
                groundCheck.position +
                Vector3.down * checkDistance
            );
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRadius
            );
        }
    }
}