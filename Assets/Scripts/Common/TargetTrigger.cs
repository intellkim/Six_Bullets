using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    public GunShootManager gunShootManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("zone 들어옴");
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ForceGrounded(); // 아래에서 정의할 함수 호출
            }
            gunShootManager.EnterBulletChoiceMode();
            gameObject.SetActive(false);
        }
    }
}
