using UnityEngine;

public class Slime : QuaiVat
{
[Header("Stats")]
public int maxHP = 30;
private int currentHP;

[Header("Movement")]
public float speed = 2f;

[Header("Patrol Line")]
public float patrolDistance = 3f;

private float leftBoundary;
private float rightBoundary;

private bool movingRight = true;

[Header("Player Detect")]
public Transform player;

public float detectRange = 4f;
public float attackRange = 1f;

[Header("Attack")]
public int damage = 10;

public float attackCooldown = 1.5f;

private float attackTimer;

public LayerMask playerLayer;

private bool facingRight = true;

// ================= START ================= //

protected override void Start()
{
    base.Start();

    currentHP = maxHP;

    leftBoundary =
        transform.position.x - patrolDistance;

    rightBoundary =
        transform.position.x + patrolDistance;

    GameObject p =
        GameObject.FindGameObjectWithTag("Player");

    if (p != null)
    {
        player = p.transform;
    }
}

// ================= UPDATE ================= //

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

    // Attack
    if (distance <= attackRange)
    {
        Attack();
    }

    // Chase
    else if (distance <= detectRange)
    {
        ChasePlayer();
    }

    // Patrol
    else
    {
        Patrol();
    }

    UpdateAnimation();
}

// ================= PATROL ================= //

void Patrol()
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

    rb.linearVelocity =
        new Vector2(
            dirX * speed,
            rb.linearVelocity.y
        );

    UpdateDirection(dirX);
}

// ================= CHASE ================= //

void ChasePlayer()
{
    float dirX =
        Mathf.Sign(
            player.position.x -
            transform.position.x
        );

    rb.linearVelocity =
        new Vector2(
            dirX * speed,
            rb.linearVelocity.y
        );

    UpdateDirection(dirX);
}

// ================= ATTACK ================= //

void Attack()
{
    rb.linearVelocity =
        new Vector2(
            0,
            rb.linearVelocity.y
        );

    if (attackTimer > 0) return;

    attackTimer = attackCooldown;

    // Animation Attack
    if (anim != null)
    {
        anim.SetTrigger("Attack");
    }

    Collider2D hitPlayer =
        Physics2D.OverlapCircle(
            transform.position,
            attackRange,
            playerLayer
        );

    if (hitPlayer != null)
    {
        DieuKhienNhanVat p = hitPlayer.GetComponent<DieuKhienNhanVat>();

        if (p != null)
        {
            p.NhanSatThuong(damage);
        }
    }
}

// ================= TAKE DAMAGE ================= //

public void TakeHit(int damageTaken)
{
    if (isDead) return;

    currentHP -= damageTaken;

    // Nếu còn sống
    if (currentHP > 0)
    {
        if (anim != null)
        {
            anim.SetTrigger("TakeHit");
        }
    }
    else
    {
        Die();
    }
}

// ================= DIE ================= //

void Die()
{
    isDead = true;

    // Dừng di chuyển
    rb.linearVelocity = Vector2.zero;

    // Tắt collider
    Collider2D coll =
        GetComponent<Collider2D>();

    if (coll != null)
    {
        coll.enabled = false;
    }

    // Tắt rigidbody
    rb.simulated = false;

    // Tắt script
    this.enabled = false;

    // Destroy slime
    Destroy(gameObject, 0.15f);
}

// ================= ANIMATION ================= //

void UpdateAnimation()
{
    if (anim != null)
    {
        anim.SetBool(
            "isRunning",
            Mathf.Abs(rb.linearVelocity.x) > 0.1f
        );
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

    transform.localScale = scale;
}

// ================= GIZMOS ================= //

void OnDrawGizmosSelected()
{
    // Detect range
    Gizmos.color = Color.red;

    Gizmos.DrawWireSphere(
        transform.position,
        detectRange
    );

    // Attack range
    Gizmos.color = Color.yellow;

    Gizmos.DrawWireSphere(
        transform.position,
        attackRange
    );

    // Patrol line
    Gizmos.color = Color.cyan;

    Gizmos.DrawLine(
        new Vector3(
            transform.position.x - patrolDistance,
            transform.position.y,
            0
        ),

        new Vector3(
            transform.position.x + patrolDistance,
            transform.position.y,
            0
        )
    );
}

}