using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GunShootManager : MonoBehaviour
{
    public GameObject[] targets;
    public GameObject cinematicOverlay;
    public AudioSource heartbeatAudio;
    public AudioClip gunshotSFX;
    public AudioSource audioSource;
    public TextMeshProUGUI dialogueText;
    public GameObject crosshairUI;
    public Animator playerAnim;
    public PlayerController playerController;

    private bool canShoot = false;

    void Start()
    {
        // 초기화는 BulletCountDisplay에서 책임지도록 수정 가능
        if (!PlayerPrefs.HasKey("BulletsLeft"))
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
            playerController.ForceGrounded();

        playerAnim.SetBool("isAiming", true);
        heartbeatAudio.Play();
        cinematicOverlay.SetActive(true);
        Time.timeScale = 0.1f;

        foreach (GameObject target in targets)
            target.SetActive(true);

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
            Cursor.visible = true;

            Target targetScript = hit.collider.GetComponent<Target>();
            if (targetScript != null)
            {
                HandleTargetHit(targetScript);
            }
        }
    }

    void HandleTargetHit(Target target)
    {
        ShowDialogue(target.dialogueText);
        FireGunEffect();

        if (target.reduceBullet)
        {
            int bulletsLeft = Mathf.Max(PlayerPrefs.GetInt("BulletsLeft", 6) - 1, 0);
            PlayerPrefs.SetInt("BulletsLeft", bulletsLeft);
        }

        if (!string.IsNullOrEmpty(target.nextSceneName))
        {
            PlayerPrefs.SetString("NextSceneAfterBulletCount", target.nextSceneName);
        }

        StartCoroutine(target.PlayHitEffect());

        Invoke(nameof(LoadNextScene), 0.8f);
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
    void FireGunEffect()
    {
        Debug.Log("총 발사 빵! 🔫");
        audioSource.PlayOneShot(gunshotSFX); 
        // 이펙트나 사운드 추가하면 됨
    }
}
