using UnityEngine;
using UnityEngine.SceneManagement;

public class PoliceSlow : MonoBehaviour
{
    public float patrolSpeed = 1f;
    public float chaseSpeed = 2f;
    public float detectionRange = 5f;
    public Transform leftPoint;
    public Transform rightPoint;

    private Transform player;
    private bool isChasing = false;
    private Transform currentTarget;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentTarget = rightPoint;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {
            isChasing = true;
        }
        else if (distance > detectionRange + 2f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        float step = patrolSpeed * Time.deltaTime;
        Debug.Log($"[Step] step = {step}");
        Vector3 newPos = Vector3.MoveTowards(transform.position, currentTarget.position, step);
        transform.position = newPos;

        // 거리 계산을 Vector2 기준으로!
        Vector2 currentPos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPos2D = new Vector2(currentTarget.position.x, currentTarget.position.y);

        float distance = Vector2.Distance(currentPos2D, targetPos2D);
        Debug.Log($"[Patrol] 현재 위치: {transform.position}, 목표: {currentTarget.name}, 거리: {distance}");

        if (distance < 0.1f)
        {
            Debug.Log($"[Patrol] {currentTarget.name}에 도달 → 방향 전환");
            currentTarget = (currentTarget == leftPoint) ? rightPoint : leftPoint;
            Debug.Log($"[Patrol] 새로운 타겟: {currentTarget.name}");
        }

    }


    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * chaseSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("경찰에게 잡힘 → 씬 리셋");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
