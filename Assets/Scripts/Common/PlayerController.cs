using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerController : MonoBehaviour
{
    // -------------------- 🔵 이동 및 점프 --------------------
    [SerializeField] private float moveSpeed = 4f;           // 기본 이동 속도
    [SerializeField] private float jumpForce = 7f;           // 점프 힘
    private const float AIR_RESISTANCE = 0.4f;
    [SerializeField] private float maxMovement = 6f;
    private bool isGrounded;               // 땅에 있는지 여부
    private bool isTouchingWall = false;   // 벽에 닿았는지 여부
    private int currentWallTouching = 0;
    private bool isWallJumping = false;
    [SerializeField] private float wallJumpTime = 0.4f;
    private float wallJumpCounter;
    [SerializeField] private float wallJumpForceX = 6f;
    [SerializeField] private float wallJumpForceY = 10f;

    // -------------------- 🟠 전투 및 공격 --------------------
    [SerializeField] private float attackRange = 1.5f;         // 주먹 공격 범위
    [SerializeField] private LayerMask betrayerLayer;         // 배신자 레이어 탐지
    private float attackCooldown = 0.5f;
    private float lastAttackTime = -10f;

    // -------------------- 🔴 피격 및 넉백 --------------------
    private bool isKnockedBack = false;
    [SerializeField] private int hitByBetrayerCount = 0;
    [SerializeField] private int maxHitsToTriggerGun = 3;
    [SerializeField] private GunShootManager gunShootManager; // 인스펙터에서 연결

    // -------------------- 🟢 슬라이드 --------------------
    [SerializeField] private float slideSpeed = 10f;       // 슬라이드 속도
    [SerializeField] private float slideTimeMax;
    private float slideTimer;
    private bool isSliding = false;
    // ▶ 슬라이드용 콜라이더 사이즈 설정
    private BoxCollider2D boxCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    [SerializeField] private Vector2 slideColliderSize = new Vector2(1.5f, 0.5f);   // 원하는 사이즈로 조정
    [SerializeField] private Vector2 slideColliderOffset = new Vector2(0f, -0.25f); // 콜라이더 중심 조절

    // -------------------- 🟢 숨기 --------------------
    private bool isInHideSpot = false;
    public bool isHiding = false;
    private SpriteRenderer sr;


    // -------------------- ✅ 기능 ON/OFF 설정 (인스펙터에서 조절 가능) --------------------
    [Header("🔘 기능 활성화 여부")]
    [SerializeField] private bool enableMovement = true;
    [SerializeField] private bool enableJump = true;
    [SerializeField] private bool enableCombat = true;
    [SerializeField] private bool enableSlide = true;
    [SerializeField] private bool enableHide = true;

    // -------------------- ⚙️ 컴포넌트 --------------------
    private Rigidbody2D rb;
    private Animator anim;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();


        boxCollider = GetComponent<BoxCollider2D>();
        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.up, Color.green);

        isGrounded = Mathf.Abs(rb.linearVelocity.y) <= 0.01f;
        isTouchingWall = currentWallTouching > 0;

        if (enableMovement) Move(); // 🔵 이동 기능 토글
        if (enableJump) Jump(); // 🔵 점프 기능 토글

        anim.SetBool("isJumping", !isGrounded);
        if (isWallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter <= 0f)
            {
                isWallJumping = false;
            }
        }
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            Debug.Log($"⏱ 슬라이딩 중... 남은 시간: {slideTimer:F2}");
            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }

        if (enableCombat && Input.GetKeyDown(KeyCode.X) && Time.time - lastAttackTime > attackCooldown) // 🟠 전투 기능 토글
        {

            Debug.Log("👊👊 배신자에게 주먹 공격 시도");
            TryPunch();
            lastAttackTime = Time.time;
        }
        if (enableHide && isInHideSpot && Input.GetKeyDown(KeyCode.Z)) // 🟣 은신 기능 토글
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

    private void Move()
    {
        if (isKnockedBack) return; // 넉백 중이면 이동 차단

        float moveInput = Input.GetAxisRaw("Horizontal"); // A, D 키 또는 ←, → 방향키
        bool startSlideInput = Input.GetKeyDown(KeyCode.LeftShift);
        bool stopSlideInput = Input.GetKeyUp(KeyCode.LeftShift);

        if (!isWallJumping)
        {
            float xMovement = moveSpeed * moveInput;
            if (startSlideInput && Mathf.Abs(moveInput) > 0f)
            {
                Debug.Log("💨 슬라이드 입력 감지됨!");
                Debug.Log($"Shift: {Input.GetKeyDown(KeyCode.LeftShift)}, MoveInput: {moveInput}");
                StartSlide();
            }
            else if (stopSlideInput) EndSlide();

            if (!isGrounded)
            {
                xMovement += rb.linearVelocity.x;
                rb.linearDamping = AIR_RESISTANCE;
            }
            else
            {
                rb.linearDamping = 0f;
            }

            if (xMovement > maxMovement) xMovement = maxMovement;
            else if (xMovement < maxMovement * -1f) xMovement = maxMovement * -1f;

            if (isSliding && enableSlide)
            {
                float direction = Mathf.Sign(transform.localScale.x);
                xMovement = direction * slideSpeed;
            }

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

    private void Jump()
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HideSpot"))
        {
            isInHideSpot = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HideSpot"))
        {
            isInHideSpot = false;
            isHiding = false; // 밖으로 나오면 은신 해제
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log($"Collision with: {collision.gameObject.name}");
        // 땅에 닿으면 isGrounded true
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentWallTouching += 1;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.Log($"Exiting with: {collision.gameObject.name}");
        // 땅에서 떨어지면 isGrounded false
        if (collision.gameObject.CompareTag("Ground"))
        {
            currentWallTouching -= 1;
        }
    }

    public void ForceGrounded()
    {
        isGrounded = true;
        isWallJumping = false;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isJumping", false);
        anim.SetFloat("Speed", 0f);

        anim.Play("Idle 0"); // 👉 실제 Idle 상태 이름이 다르면 정확한 이름으로 수정

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

    private void TryPunch()
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

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideTimeMax;

        boxCollider.size = slideColliderSize;
        boxCollider.offset = slideColliderOffset;

        float direction = Mathf.Sign(transform.localScale.x);
        float slideAngle = (direction > 0) ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0f, 0f, slideAngle);
        Debug.Log($"▶ 슬라이드 시작! angle={slideAngle}, duration={slideTimer}");
    }

    private void EndSlide()
    {
        LayerMask mask = LayerMask.GetMask("Ground");
        float raycastDistance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, raycastDistance, mask);
        if (hit)
        {
            Debug.Log("Raycast hit!");
            return;
        }

        isSliding = false;
        rb.linearVelocity = Vector2.zero;

        // ✅ 콜라이더 복구
        boxCollider.size = originalColliderSize;
        boxCollider.offset = originalColliderOffset;
        // ▶ 원래 회전 상태로 복구
        transform.rotation = Quaternion.identity;
    }
}
