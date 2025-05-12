using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BulletCountDisplay : MonoBehaviour
{
    public Image[] bulletIcons; // Inspector에 Bullet1~6 연결
    public Color activeColor = Color.yellow;
    public Color inactiveColor = new Color(0.2f, 0.2f, 0.2f);

    void Start()
    {
        int bulletsLeft = PlayerPrefs.GetInt("BulletsLeft", 6);
        Debug.Log("총알 남은 수: " + bulletsLeft);

        for (int i = 0; i < bulletIcons.Length; i++)
        {
            bulletIcons[i].color = (i < bulletsLeft) ? activeColor : inactiveColor;
        }

        Invoke("GoToNextScene", 2f); // 2초 후 자동 이동
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(SceneList.BadEnd1);
    }
}
