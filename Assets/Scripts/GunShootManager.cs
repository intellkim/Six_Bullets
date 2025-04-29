using UnityEngine;
using UnityEngine.SceneManagement;

public class GunShootManager : MonoBehaviour
{
    public GameObject[] targets;
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

            if (hit.collider.CompareTag("TargetA"))
            {
                Invoke("LoadSceneA", 1.0f);
            }
            else if (hit.collider.CompareTag("TargetB"))
            {
                Invoke("LoadSceneB", 1.0f);
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
}
