using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;   // 이동 속도
    public float jumpForce = 7f;   // 점프 힘
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator anim;
    // 벽점프 관련 변수
    public float wallJumpForceX = 6f;
    public float wallJumpForceY = 10f;

    private bool isTouchingWall = false;
    private bool isWallJumping = false;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
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
        if (isWallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter <= 0f)
            {
                isWallJumping = false;
            }
        }
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
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (isTouchingWall && !isGrounded)
            {
                isWallJumping = true;
                wallJumpCounter = wallJumpTime;

                // 벽 반대 방향으로 튕겨 나가기
                float direction = -Mathf.Sign(transform.localScale.x);
                rb.linearVelocity = new Vector2(wallJumpForceX * direction, wallJumpForceY);

                // 방향 전환
                transform.localScale = new Vector3(direction, 1f, 1f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 땅에 닿으면 isGrounded true
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 땅에서 떨어지면 isGrounded false
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = false;
    }
    public void ForceGrounded()
    {
        isGrounded = true;
        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // 수직 속도 정지
        anim.SetBool("isJumping", false);
    }

}
