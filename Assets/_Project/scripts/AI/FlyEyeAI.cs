using UnityEngine;

public class FlyEye : QuaiVat
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Random Patrol Area")]
    public Vector2 areaSize =
        new Vector2(4f, 2f);

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

    public LayerMask playerLayer;

    private bool facingRight = true;

    protected override void Start()
    {
        base.Start();

        startPosition =
            transform.position;

        ChooseNewTarget();

        GameObject p =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

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

    // ================= RANDOM PATROL ================= //

    void RandomPatrol()
    {
        Vector2 direction =
            (
                targetPosition -
                (Vector2)transform.position
            ).normalized;

        rb.linearVelocity =
            direction * speed;

        UpdateDirection(direction.x);

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
            new Vector2(
                randomX,
                randomY
            );
    }

    // ================= CHASE ================= //

    void ChasePlayer()
    {
        Vector2 direction =
            (
                player.position -
                transform.position
            ).normalized;

        rb.linearVelocity =
            direction * speed;

        UpdateDirection(direction.x);
    }

    // ================= ATTACK ================= //

    void Attack()
    {
        rb.linearVelocity =
            Vector2.zero;

        if (attackTimer > 0) return;

        attackTimer =
            attackCooldown;

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
                hitPlayer.GetComponent
                <DieuKhienNhanVat>();

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

        Vector3 scale =
            transform.localScale;

        scale.x *= -1;

        transform.localScale =
            scale;
    }

    // ================= GIZMOS ================= //

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            detectRange
        );

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

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