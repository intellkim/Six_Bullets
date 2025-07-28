using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WreckingBall : MonoBehaviour
{
    public Rigidbody2D rb;                // 레킹볼 rigidbody
    public Transform anchor;             // 고정점
    public LineRenderer lineRenderer;    // 줄 표현
    public float swingAmplitude = 160f;    // 진자 힘의 진폭 (기존 swingForce)
    public float swingFrequency = 1.5f;  // 진자 진동수
    public float pushForce = 80f;        // 플레이어 밀어내는 힘
    public float dragAmount = 0.5f;      // 감속 (속도 폭주 방지)

    private DistanceJoint2D joint;

    void Start()
    {
        // Rigidbody 설정
        rb.mass = 200f;
        rb.linearDamping = dragAmount;
        rb.gravityScale = 1f;

        // DistanceJoint 연결 설정
        joint = rb.GetComponent<DistanceJoint2D>();
        if (joint == null) joint = rb.gameObject.AddComponent<DistanceJoint2D>();
        joint.autoConfigureDistance = false;
        joint.connectedAnchor = anchor.position;
        joint.distance = Vector2.Distance(rb.position, anchor.position);
        joint.enableCollision = true;

        // 초기 줄 시각화 설정
        if (lineRenderer != null)
            lineRenderer.positionCount = 2;

        // 초기 관성 살짝 주기 (오른쪽으로 시작)
        rb.AddForce(Vector2.right * swingAmplitude, ForceMode2D.Impulse);
    }

    void Update()
    {
        // 진자처럼 반복 운동
        float swingDirection = Mathf.Sin(Time.time * swingFrequency);
        rb.AddForce(Vector2.right * swingDirection * swingAmplitude, ForceMode2D.Force);

        // 줄 LineRenderer 업데이트
        if (lineRenderer != null)
        {
            Vector3 start = anchor.position;
            Vector3 end = rb.transform.position;
            start.z = end.z = 0f;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 pushDir = (playerRb.position - rb.position).normalized;

                // 🧠 위 방향일수록 힘을 줄이기
                float verticalRatio = Mathf.Clamp01(Vector2.Dot(pushDir, Vector2.up)); // 0(수평) ~ 1(정수직)
                float forceModifier = Mathf.Lerp(1f, 0.3f, verticalRatio); // 위일수록 30%로 줄임

                // 💥 힘 적용
                playerRb.AddForce(pushDir * pushForce * forceModifier, ForceMode2D.Impulse);

                // 🛡️ velocity 제한 (너무 날아가지 않게)
                playerRb.linearVelocity = Vector2.ClampMagnitude(playerRb.linearVelocity, 15f);

                Debug.Log($"💥 레킹볼 충돌: pushDir={pushDir}, verticalRatio={verticalRatio:F2}, velocity={playerRb.linearVelocity}");
            }
        }
    }

}
