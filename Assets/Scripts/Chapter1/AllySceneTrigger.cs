using UnityEngine;
using UnityEngine.SceneManagement;

public class AllySceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("조력자에게 도착! 씬 전환");
            SceneManager.LoadScene(SceneList.Chapter1_Gunchoice); // 또는 SceneList.Chapter1_GunChoice
        }
    }
}
