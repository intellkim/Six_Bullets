using UnityEngine;

public class RopeSwing : MonoBehaviour
{
    // public Transform ropeEnd;              // 줄 끝 지점
    public LineRenderer lineRenderer;      // 줄 시각화
    private Rigidbody2D playerRb;
    private DistanceJoint2D joint;

    public float swingForce = 10f;
    public KeyCode attachKey = KeyCode.C;
    public KeyCode detachKey = KeyCode.Space;

    private bool isSwinging = false;

    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = true;

        // 🎯 z값 고정
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + new Vector3(3f, -3f, 0f);

        startPos.z = 0f;
        endPos.z = 0f;

        lineRenderer.SetPosition(0, startPos); // 줄 시작점 (SwingRope)
        lineRenderer.SetPosition(1, endPos);   // 아래로 미리 보기

        Debug.Log($"🧵 [Start] 줄 초기화: Start={startPos}, End={endPos}, enabled={lineRenderer.enabled}");
    }

    void Update()
    {
        // 🎯 줄 먼저 그려줌 (항상)
        if (lineRenderer != null)
        {
            Vector3 start = transform.position;
            Vector3 end = isSwinging && playerRb != null
                ? playerRb.transform.position
                : transform.position + new Vector3(3f, -3f, 0f);

            // 👉 z값 고정 (카메라 앞면에 줄이 보이도록)
            start.z = 0f;
            end.z = 0f;

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            Debug.Log($"🧵 줄 위치: {start} → {end}");
        }

        // ❌ playerRb 없으면 스윙 관련은 생략
        if (playerRb == null) return;

        if (Input.GetKeyDown(attachKey) && !isSwinging)
        {
            AttachToRope();
        }

        if (Input.GetKeyDown(detachKey) && isSwinging)
        {
            DetachFromRope();
        }

        if (isSwinging)
        {
            float h = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(h) > 0.1f)
            {
                playerRb.AddForce(Vector2.right * h * swingForce, ForceMode2D.Force);
            }
        }
    }

    void AttachToRope()
    {
        Debug.Log("🧗 DistanceJoint 연결");
        joint = playerRb.gameObject.AddComponent<DistanceJoint2D>();
        joint.autoConfigureDistance = false;
        joint.connectedAnchor = transform.position; // SwingRope 위치
        joint.distance = Vector2.Distance(playerRb.position, transform.position);
        joint.enableCollision = false;

        isSwinging = true;
    }

    void DetachFromRope()
    {
        if (joint != null) Destroy(joint);
        isSwinging = false;
        playerRb = null;
        Debug.Log("🪂 줄에서 탈출");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerRb = other.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isSwinging)
        {
            playerRb = null;
        }
    }
}
