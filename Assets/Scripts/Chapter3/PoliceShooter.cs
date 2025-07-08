using UnityEngine;
using UnityEngine.SceneManagement;

public class PoliceShooter : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;

    [Header("Detection")]
    public float detectionRange = 7f;
    public float forgetRange = 10f;

    [Header("Shooting")]
    public float shootInterval = 2f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;

    [Header("Patrol Points")]
    public Transform leftPoint;
    public Transform rightPoint;

    private Transform player;
    private Transform currentTarget;
    private float shootTimer = 0f;
    private bool isChasing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentTarget = rightPoint;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 추격 시작
        if (distanceToPlayer < detectionRange)
        {
            isChasing = true;
        }
        // 추격 중인데 너무 멀어지면 포기하고 순찰로 복귀
        else if (distanceToPlayer > forgetRange)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChaseAndShoot();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        float step = patrolSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, step);

        Vector2 currentPos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPos2D = new Vector2(currentTarget.position.x, currentTarget.position.y);

        if (Vector2.Distance(currentPos2D, targetPos2D) < 0.1f)
        {
            currentTarget = (currentTarget == leftPoint) ? rightPoint : leftPoint;
        }
    }

    void ChaseAndShoot()
    {
        // 이동
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * chaseSpeed * Time.deltaTime);

        // 총 쏘기
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        Vector2 direction = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }
        Debug.Log("총 발사!");
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
