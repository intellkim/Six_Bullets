using UnityEngine;
using System.Collections;

public class BetrayerAI : MonoBehaviour
{
    public Transform player;
    public float jumpForce = 12f;
    public float jumpInterval = 3f;
    public float attackForceX = 5f;
    public float maxJumpDistance = 5f; // 최대 도약 거리 제한

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool canBeHit = false;

    private enum State { Idle, Jumping, Landing }
    private State currentState = State.Idle;
    public int hitCount = 0;
    public int maxHits = 3;
    public GunShootManager gunShootManager; // 인스펙터에서 연결
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("JumpTowardPlayer", 1f, jumpInterval);
    }

    void JumpTowardPlayer()
    {
        if (!isGrounded) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > maxJumpDistance) return;

        float distanceX = player.position.x - transform.position.x;
        float rawForceX = distanceX * 50f;

        // 가까워도 최소 x힘 보장
        if (Mathf.Abs(rawForceX) < 200f)
            rawForceX = Mathf.Sign(rawForceX) * 200f;

        // Clamp 범위를 크게 설정하거나 생략
        // float clampedForceX = Mathf.Clamp(rawForceX, -600f, 600f);  // ← 충분히 넉넉하게

        Vector2 force = new Vector2(rawForceX, jumpForce);
        Debug.Log($"📐 점프 Force: {rawForceX}");

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        currentState = State.Jumping;
        isGrounded = false;
        canBeHit = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            // 점프 중 착지 감지
            if (currentState == State.Jumping)
            {
                currentState = State.Landing;
                StartCoroutine(LandingVulnerableWindow());
            }

            // ⭐ 최초 착지 또는 Idle 상태에서 착지했을 때도 isGrounded 처리
            if (currentState == State.Idle)
            {
                isGrounded = true;
                Debug.Log("📦 바닥에 닿음 → isGrounded = true");
            }
        }

        if (collision.gameObject.CompareTag("Player") && !isGrounded)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ApplyKnockback(transform.position, 4f, 0.1f);
                Debug.Log("💥 플레이어 넉백됨 (연출)!");
            }
        }
    }

    IEnumerator LandingVulnerableWindow()
    {
        canBeHit = true;
        yield return new WaitForSeconds(0.5f); // 착지 후 무방비 시간
        canBeHit = false;
        currentState = State.Idle;
        isGrounded = true;
    }

    public void TryTakeDamage()
    {
        if (canBeHit)
        {
            Debug.Log("🟥 배신자 피격 성공!");
            hitCount++;

            if (hitCount >= maxHits)
            {
                Debug.Log("🔫 총기 선택 트리거 발동!");
                if (gunShootManager != null)
                {
                    gunShootManager.EnterBulletChoiceMode(); // ← 연출 시작
                    this.enabled = false;  // AI 멈춤
                }
                else
                    Debug.LogWarning("GunShootManager 연결 안됨!");
            }

            // 피격 이펙트/리액션 추가 가능
        }
        else
        {
            Debug.Log("🟨 피격 무시됨 (점프 중 또는 무적 상태)");
        }
    }
}
