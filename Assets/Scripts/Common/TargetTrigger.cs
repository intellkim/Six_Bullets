using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    public GunShootManager gunShootManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("zone 들어옴");
            gunShootManager.EnterBulletChoiceMode();
            gameObject.SetActive(false);
        }
    }
}
