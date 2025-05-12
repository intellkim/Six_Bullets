using UnityEngine;

public class BagTrigger : MonoBehaviour
{
    public PrologueManager prologueManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            prologueManager.StartBagCutscene();
            gameObject.SetActive(false); // 트리거는 한 번만 작동
        }
    }
}
