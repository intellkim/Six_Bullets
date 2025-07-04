using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GunShootManager : MonoBehaviour
{
    public GameObject[] targets;            // 타겟들
    public GameObject cinematicOverlay;     // 어두운 연출용 오버레이 (ex. 반투명 검정)
    public AudioSource heartbeatAudio;
    public AudioClip gunshotSFX;
    public AudioSource audioSource;
    public TextMeshProUGUI dialogueText;
    public GameObject crosshairUI;
    private bool canShoot = false;
    public Animator playerAnim;
    public PlayerController playerController;
    void Start(){
        PlayerPrefs.SetInt("BulletsLeft", 6);
    }
    void Update()
    {
        if (canShoot && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    public void EnterBulletChoiceMode()
    {
        if (playerController != null)
    {
        playerController.ForceGrounded();  // ✅ 총기 선택 직전 플레이어 고정
    }
        playerAnim.SetBool("isAiming", true);
        heartbeatAudio.Play();             // 심장 소리
        cinematicOverlay.SetActive(true);  // 어두운 연출
        Time.timeScale = 0.1f;               // 시간 느리게

        foreach (GameObject target in targets)
        {
            target.SetActive(true);        // 타겟 표시
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = false;
        crosshairUI.SetActive(true);
        canShoot = true;
    }
    void Shoot()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("맞춘 타겟: " + hit.collider.name);

            Time.timeScale = 1f;
            heartbeatAudio.Stop();
            cinematicOverlay.SetActive(false);
            crosshairUI.SetActive(false);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true;

            Target targetScript = hit.collider.GetComponent<Target>();
            
            if (hit.collider.CompareTag("TargetA"))
            {
                ShowDialogue("You hit Target A!");
                FireGunEffect();
                if (targetScript != null)
                {
                    StartCoroutine(targetScript.PlayHitEffect());
                }

                int bulletsLeft = PlayerPrefs.GetInt("BulletsLeft", 6);
                bulletsLeft = Mathf.Max(bulletsLeft - 1, 0);
                PlayerPrefs.SetInt("BulletsLeft", bulletsLeft);
                // 🔽 다음 씬으로 이동
                Invoke("LoadNextScene", 0.8f);

            }
            else if (hit.collider.CompareTag("TargetB"))
            {
                ShowDialogue("You hit Target B!");
                FireGunEffect();
                if (targetScript != null)
                {
                    StartCoroutine(targetScript.PlayHitEffect());
                }

                int bulletsLeft = PlayerPrefs.GetInt("BulletsLeft", 6);
                bulletsLeft = Mathf.Max(bulletsLeft - 1, 0);
                PlayerPrefs.SetInt("BulletsLeft", bulletsLeft);
                // 🔽 다음 씬으로 이동
                Invoke("LoadNextScene", 0.8f);

            }
            else if (hit.collider.CompareTag("Target_Betrayer"))
            {
                ShowDialogue("You hit Target Target_Betrayer!");
                FireGunEffect();
                if (targetScript != null)
                {
                    StartCoroutine(targetScript.PlayHitEffect());
                }

                int bulletsLeft = PlayerPrefs.GetInt("BulletsLeft", 6);
                bulletsLeft = Mathf.Max(bulletsLeft - 1, 0);
                PlayerPrefs.SetInt("BulletsLeft", bulletsLeft);
                // 🔽 다음 씬으로 이동
                Invoke("LoadNextScene", 0.8f);

            }
        }
    }

    void FireGunEffect()
    {
        Debug.Log("총 발사 빵! 🔫");
        audioSource.PlayOneShot(gunshotSFX); 
        // 이펙트나 사운드 추가하면 됨
    }

    public void ActivateTargets()
    {
        foreach (GameObject target in targets)
        {
            target.SetActive(true);
        }
        canShoot = true;
    }
    void LoadNextScene()
    {
      SceneManager.LoadScene(SceneList.BulletCount);
    }
    void ShowDialogue(string message)
    {
        dialogueText.text = message;
        dialogueText.gameObject.SetActive(true);
    }
}
