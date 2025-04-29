using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GunShootManager : MonoBehaviour
{
    public GameObject[] targets;
    public TextMeshProUGUI dialogueText;
    private bool canShoot = false;

    void Update()
    {
        if (canShoot && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("맞춘 타겟: " + hit.collider.name);

            FireGunEffect();
            Target targetScript = hit.collider.GetComponent<Target>();
            if (targetScript != null)
            {
                StartCoroutine(targetScript.PlayHitEffect());
            }
            if (hit.collider.CompareTag("TargetA"))
            {
                ShowDialogue("You hit Target A!");
            }
            else if (hit.collider.CompareTag("TargetB"))
            {
                ShowDialogue("You hit Target B!");
            }
        }
    }

    void FireGunEffect()
    {
        Debug.Log("총 발사 빵! 🔫");
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

    void LoadSceneA()
    {
        SceneManager.LoadScene("SceneA");
    }

    void LoadSceneB()
    {
        SceneManager.LoadScene("SceneB");
    }
    void ShowDialogue(string message)
    {
        dialogueText.text = message;
        dialogueText.gameObject.SetActive(true);
    }
}
