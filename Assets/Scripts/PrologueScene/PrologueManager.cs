using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class PrologueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    [Header("조명 연출")]
    public Light2D globalLight;  // Light 2D (Global Light)
    public float darkIntensity = 0.2f; // 어두워질 때 값
    public float lightIntensity = 1f;  // 기본 밝기
    void Awake()
    {
        PlayerPrefs.SetInt("BulletsLeft", 6);
        PlayerPrefs.Save(); // (선택) 즉시 저장
    }

    void Start()
    {
        dialogueText.text = "";
        if (globalLight != null)
            globalLight.intensity = lightIntensity;
    }

    public void StartBagCutscene()
    {
        StartCoroutine(PlayCutscene());
    }

    System.Collections.IEnumerator PlayCutscene()
    {
        if (globalLight != null)
            globalLight.intensity = darkIntensity;
        yield return new WaitForSecondsRealtime(0.5f);

        yield return ShowLine("...가방 안에... 총?");
        yield return new WaitForSecondsRealtime(1f);
        yield return ShowLine("총알이... 6발이나?");
        yield return new WaitForSecondsRealtime(1f);
        yield return ShowLine("이건... 무슨 일이야...");

        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(SceneList.Chapter1_Jumpmap);
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
