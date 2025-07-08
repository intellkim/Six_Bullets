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
    public float shootDelay = 0.5f; // 🔥 추가: 총알 쏘기 전 대기시간
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;

    [Header("Patrol Points")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Laser Line")] // 🔥 추가
    public LineRenderer laserLine;

    private Transform player;
    private Transform currentTarget;
    private float shootTimer = 0f;
    private bool isChasing = false;
    private bool isPreparingToShoot = false; // 🔥 추가
    private Vector2 shootDirection; // 🔥 조준 방향 저장용


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentTarget = rightPoint;
        laserLine.enabled = false; // 🔥 추가
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            isChasing = true;
        }
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

        // 총 쏘기 (예고 → 발사)
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval && !isPreparingToShoot)
        {
            shootTimer = 0f;
            isPreparingToShoot = true;
            ShootWithWarning(); // 🔥 추가: 레이저 후 발사
        }
    }

    void ShootWithWarning() // 🔥 추가
    {
        shootDirection = (player.position - firePoint.position).normalized; // ✅ 조준 순간의 방향 저장
        Vector2 endPoint = (Vector2)firePoint.position + shootDirection * 10f;

        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, endPoint);
        laserLine.enabled = true;

        Invoke("Shoot", shootDelay);
        Invoke("DisableLaser", shootDelay);
    }

    void DisableLaser() // 🔥 추가
    {
        laserLine.enabled = false;
        isPreparingToShoot = false;
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection * bulletSpeed; // ✅ 저장한 방향 그대로 사용
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
