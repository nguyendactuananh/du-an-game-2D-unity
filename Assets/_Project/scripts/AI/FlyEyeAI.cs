using UnityEngine;

public class FlyEye : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 30;
    private int currentHP;

    [Header("Movement")]
    public float speed = 2f;

    [Header("Random Patrol Area")]
    public Vector2 areaSize = new Vector2(4f, 2f);

    private Vector2 startPosition;
    private Vector2 targetPosition;

    [Header("Detect Player")]
    public Transform player;

    public float detectRange = 5f;
    public float attackRange = 1.2f;

    [Header("Attack")]
    public int damage = 10;

    public float attackCooldown = 1.5f;

    private float attackTimer;

    [Header("Layer")]
    public LayerMask playerLayer;

    private Rigidbody2D rb;
    private Animator anim;

    private bool facingRight = true;

    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        currentHP = maxHP;

        startPosition = transform.position;

        ChooseNewTarget();

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
            RandomPatrol();
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
            RandomPatrol();
        }
    }

    // =========================
    // RANDOM PATROL
    // =========================

    void RandomPatrol()
    {
        if (isAttacking) return;

        Vector2 newPos =
            Vector2.MoveTowards(
                rb.position,
                targetPosition,
                speed * Time.fixedDeltaTime
            );

        rb.MovePosition(newPos);

        UpdateDirection(
            targetPosition.x -
            transform.position.x
        );

        if (
            Vector2.Distance(
                transform.position,
                targetPosition
            ) < 0.2f
        )
        {
            ChooseNewTarget();
        }
    }

    void ChooseNewTarget()
    {
        float randomX =
            Random.Range(
                -areaSize.x,
                areaSize.x
            );

        float randomY =
            Random.Range(
                -areaSize.y,
                areaSize.y
            );

        targetPosition =
            startPosition +
            new Vector2(randomX, randomY);
    }

    // =========================
    // CHASE PLAYER
    // =========================

    void ChasePlayer()
    {
        if (isAttacking) return;

        Vector2 newPos =
            Vector2.MoveTowards(
                rb.position,
                player.position,
                speed * Time.fixedDeltaTime
            );

        rb.MovePosition(newPos);

        UpdateDirection(
            player.position.x -
            transform.position.x
        );
    }

    // =========================
    // ATTACK
    // =========================

    void Attack()
    {
        rb.linearVelocity = Vector2.zero;

        if (attackTimer > 0) return;

        attackTimer = attackCooldown;

        isAttacking = true;

        anim.SetTrigger("TanCong");

        Collider2D hitPlayer =
            Physics2D.OverlapCircle(
                transform.position,
                attackRange,
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

    // =========================
    // TAKE DAMAGE
    // =========================

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

    // =========================
    // DEATH
    // =========================

    void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        GetComponent<Collider2D>().enabled = false;

        anim.SetTrigger("Chet");

        Destroy(gameObject, 2f);
    }

    // =========================
    // FLIP
    // =========================

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

        Vector3 scale =
            transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }

    // =========================
    // GIZMOS
    // =========================

    void OnDrawGizmosSelected()
    {
        // detect range

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            detectRange
        );

        // attack range

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        // patrol area

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(
                areaSize.x * 2,
                areaSize.y * 2,
                1
            )
        );
    }
}

