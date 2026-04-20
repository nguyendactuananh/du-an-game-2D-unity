using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Chỉ số di chuyển")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 10f; // Thử giảm xuống 10 (trước là 12)

    private Rigidbody2D rb;
    private float moveInput;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // --- CÁCH NHẢY MỚI (NHẠY HƠN) ---
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            // Gán thẳng vận tốc đi lên để nhân vật vọt lên ngay lập tức
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        CheckFlip();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void CheckFlip()
    {
        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}