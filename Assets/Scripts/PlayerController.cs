using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;   // 이동 속도
    public float jumpForce = 7f;   // 점프 힘
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();

        anim.SetBool("isJumping", !isGrounded);
    }

    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); // A, D 키 또는 ←, → 방향키
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // ✅ 애니메이션 파라미터 전달
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // ✅ 방향 전환 (좌우 반전)
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 땅에 닿으면 isGrounded true
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 땅에서 떨어지면 isGrounded false
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
