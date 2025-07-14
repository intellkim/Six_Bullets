using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // -------------------- 🔵 이동 및 점프 --------------------
    [SerializeField] private float moveSpeed = 4f;           // 기본 이동 속도
    [SerializeField] private float jumpForce = 7f;           // 점프 힘
    [SerializeField] private float maxMovement = 6f;
    private bool isGrounded;               // 땅에 있는지 여부
    private bool isTouchingWall = false;   // 벽에 닿았는지 여부
    private bool isWallJumping = false;
    [SerializeField] private float wallJumpTime = 0.4f;
    private float wallJumpCounter;
    [SerializeField] private float wallJumpForceX = 6f;
    [SerializeField] private float wallJumpForceY = 10f;

    // -------------------- 🟠 전투 및 공격 --------------------
    public float attackRange = 1.5f;         // 주먹 공격 범위
    public LayerMask betrayerLayer;         // 배신자 레이어 탐지
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -10f;

    // -------------------- 🔴 피격 및 넉백 --------------------
    private bool isKnockedBack = false;
    public int hitByBetrayerCount = 0;
    public int maxHitsToTriggerGun = 3;
    public GunShootManager gunShootManager; // 인스펙터에서 연결

    // -------------------- 🟢 슬라이드 --------------------
    public float slideSpeed = 10f;       // 슬라이드 속도
    private bool isSliding = false;
    // ▶ 슬라이드용 콜라이더 사이즈 설정
    private BoxCollider2D boxCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    public Vector2 slideColliderSize = new Vector2(1.5f, 0.5f);   // 원하는 사이즈로 조정
    public Vector2 slideColliderOffset = new Vector2(0f, -0.25f); // 콜라이더 중심 조절

    // -------------------- 🟢 숨기 --------------------
    private bool isInHideSpot = false;
    public bool isHiding = false;
    private SpriteRenderer sr;

    // -------------------- ⚙️ 컴포넌트 --------------------
    private Rigidbody2D rb;
    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();


        boxCollider = GetComponent<BoxCollider2D>();
        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;
    }

    void Update()
    {
        isGrounded = rb.linearVelocity.y == 0f;

        Move();
        Jump();

        // ▶ Shift 누르고 있는 동안만 슬라이드 상태 유지
        if (!isSliding && isGrounded && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            StartSlide();
        }
        else if (isSliding && !Input.GetKey(KeyCode.LeftShift))
        {
            EndSlide();
        }
        if (isSliding)
        {
            float direction = Mathf.Sign(transform.localScale.x);
            rb.linearVelocity = new Vector2(direction * slideSpeed, rb.linearVelocity.y);
        }

        anim.SetBool("isJumping", !isGrounded);
        if (isWallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter <= 0f)
            {
                isWallJumping = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.X) && Time.time - lastAttackTime > attackCooldown)
        {

            Debug.Log("👊👊 배신자에게 주먹 공격 시도");
            TryPunch();
            lastAttackTime = Time.time;
        }
        if (isInHideSpot && Input.GetKeyDown(KeyCode.Z))
        {
            isHiding = !isHiding;
            Debug.Log(isHiding ? "😶 은신 시작" : "😶 은신 해제");

            if (isHiding)
            {
                sr.color = new Color(1f, 1f, 1f, 0.3f); // 30% 투명
            }
            else
            {
                sr.color = Color.white; // 불투명 복원
            }
        }

    }

    void Move()
    {
        if (isKnockedBack || isSliding) return; // 넉백 중이면 이동 차단

        float moveInput = Input.GetAxisRaw("Horizontal"); // A, D 키 또는 ←, → 방향키

        if (!isWallJumping)
        {
            float xMovement = moveSpeed * moveInput;
            if (!isGrounded)
            {
                xMovement += rb.linearVelocity.x;
                rb.linearDamping = 0.4f;
            }
            else
            {
                rb.linearDamping = 0f;
            }

            if (xMovement > maxMovement) xMovement = maxMovement;
            else if (xMovement < maxMovement * -1f) xMovement = maxMovement * -1f;

            rb.linearVelocity = new Vector2(xMovement, rb.linearVelocity.y);
        }


        // ✅ 애니메이션 파라미터 전달
        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        // ✅ 방향 전환 (좌우 반전)
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(rb.linearVelocity.x), 1, 1);
        }
    }

    void Jump()
    {
        if (isKnockedBack || isSliding) return; // 넉백 중이면 이동 차단

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
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HideSpot"))
        {
            isInHideSpot = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HideSpot"))
        {
            isInHideSpot = false;
            isHiding = false; // 밖으로 나오면 은신 해제
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision with: {collision.gameObject.name}");
        // 땅에 닿으면 isGrounded true
        if (collision.gameObject.CompareTag("Ground"))
        {
            isTouchingWall = true;
        }
        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 땅에서 떨어지면 isGrounded false
        if (collision.gameObject.CompareTag("Ground"))
        {
            isTouchingWall = false;
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
        anim.SetFloat("Speed", 0f);  // ✅ 강제로 정지 상태

    }
    public void ApplyKnockback(Vector2 attackerPos, float distance = 4f, float duration = 0.1f)
    {
        hitByBetrayerCount++;

        Debug.Log($"💢 플레이어 피격 누적: {hitByBetrayerCount}");

        if (hitByBetrayerCount >= maxHitsToTriggerGun)
        {
            if (gunShootManager != null)
            {
                Debug.Log("💀 플레이어 피격 3회 → 총기 선택 진입!");
                gunShootManager.EnterBulletChoiceMode();
                this.enabled = false;
            }
            else
            {
                Debug.LogWarning("⚠️ GunShootManager 연결 안됨");
            }
        }

        if (!isKnockedBack)
            StartCoroutine(KnockbackCoroutine(attackerPos, distance, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 attackerPos, float distance, float duration)
    {
        isKnockedBack = true;
        float xDir = Mathf.Sign(transform.position.x - attackerPos.x);
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir * distance, 0); // ⬅️⬅️ y=0 유지!

        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector2 nextPos = Vector2.Lerp(start, end, elapsed / duration);
            rb.MovePosition(nextPos);  // ✅ 충돌 감지 포함 이동
            elapsed += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;
    }
    void TryPunch()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, betrayerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Target_Betrayer"))  // 또는 hit.GetComponent<BetrayerAI>() != null
            {
                BetrayerAI ai = hit.GetComponent<BetrayerAI>();
                if (ai != null)
                {
                    ai.TryTakeDamage();
                    Debug.Log("👊 배신자에게 주먹 공격");
                }
            }
        }

        // 👉 Punch 애니메이션도 여기서 트리거 가능
    }
    void StartSlide()
    {
        isSliding = true;

        boxCollider.size = slideColliderSize;
        boxCollider.offset = slideColliderOffset;

        float direction = Mathf.Sign(transform.localScale.x);
        float slideAngle = (direction > 0) ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0f, 0f, slideAngle);
    }

    void EndSlide()
    {
        isSliding = false;

        // ✅ 콜라이더 복구
        boxCollider.size = originalColliderSize;
        boxCollider.offset = originalColliderOffset;
        // ▶ 원래 회전 상태로 복구
        transform.rotation = Quaternion.identity;
    }
}
