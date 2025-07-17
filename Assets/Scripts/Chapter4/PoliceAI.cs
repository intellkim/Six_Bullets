using UnityEngine;
using UnityEngine.SceneManagement;
public class PoliceAI : MonoBehaviour
{
    public Transform player;
    public float viewDistance = 6f;
    public float viewAngle = 45f;
    public LayerMask playerMask;

    public float patrolSpeed = 1f;
    public float approachSpeed = 2f;
    public Transform leftPoint, rightPoint;

    private bool movingRight = true;
    private float suspiciousTimer = 0f;
    public float suspiciousDuration = 2f; // 머무를 시간 (초)


    public enum PoliceState { Idle, Suspicious, Approach }
    private PoliceState currentState = PoliceState.Idle;

    void Update()
    {
        switch (currentState)
        {
            case PoliceState.Idle:
                Patrol();
                if (CanSeePlayer())
                {
                    Debug.Log("👀 상태 전이: Idle → Suspicious");
                    currentState = PoliceState.Suspicious;
                }
                break;

            case PoliceState.Suspicious:
                if (CanSeePlayer())
                {
                    suspiciousTimer += Time.deltaTime;

                    if (suspiciousTimer >= suspiciousDuration)
                    {
                        currentState = PoliceState.Approach;
                        suspiciousTimer = 0f; // 리셋
                    }
                }
                else
                {
                    currentState = PoliceState.Idle;
                    suspiciousTimer = 0f; // 감지 실패 → 리셋
                }
                break;
            case PoliceState.Approach:
                ApproachPlayer();
                if (!CanSeePlayer())
                {
                    Debug.Log("❌ 상태 전이: Approach → Idle");
                    currentState = PoliceState.Idle;
                }
                break;
        }
    }

    void Patrol()
    {
        transform.position += (movingRight ? Vector3.right : Vector3.left) * patrolSpeed * Time.deltaTime;

        if (movingRight && transform.position.x >= rightPoint.position.x)
        {
            Flip();
        }
        else if (!movingRight && transform.position.x <= leftPoint.position.x)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void ApproachPlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * approachSpeed * Time.deltaTime;

        if (dir.x > 0 && transform.localScale.x < 0) Flip();
        else if (dir.x < 0 && transform.localScale.x > 0) Flip();
    }

    bool CanSeePlayer()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null && pc.isHiding)
            return false;

        if (player == null) return false;

        Vector2 dirToPlayer = player.position - transform.position;
        Vector2 forwardDir = transform.localScale.x < 0 ? -transform.right : transform.right;

        float angle = Vector2.Angle(forwardDir, dirToPlayer);
        float distance = dirToPlayer.magnitude;

        if (angle > viewAngle / 2 || distance > viewDistance)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer.normalized, viewDistance, playerMask);
        if (hit.collider == null || !hit.collider.CompareTag("Player"))
            return false;

        return true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null && pc.isHiding)
            {
                Debug.Log("🫥 은신 중이라 체포 무시");
                return;
            }
            Debug.Log("🚨 플레이어 체포!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬 리셋
        }
    }
}
