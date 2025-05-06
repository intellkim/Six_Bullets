using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject blackOverlay;

    void Start()
    {
        dialogueText.text = "";
        blackOverlay.SetActive(false);
    }

    public void StartBagCutscene()
    {
        StartCoroutine(PlayCutscene());
    }

    System.Collections.IEnumerator PlayCutscene()
    {
        blackOverlay.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);

        yield return ShowLine("...가방 안에... 총?");
        yield return new WaitForSecondsRealtime(1f);
        yield return ShowLine("총알이... 6발이나?");
        yield return new WaitForSecondsRealtime(1f);
        yield return ShowLine("이건... 무슨 일이야...");

        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene("MainScene");
    }

    System.Collections.IEnumerator ShowLine(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
}
