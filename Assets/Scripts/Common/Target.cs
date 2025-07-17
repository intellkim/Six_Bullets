using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class Target : MonoBehaviour
{
    [Header("이펙트")]
    public GameObject hitEffect;  // 타겟 맞았을 때 켜질 이펙트
    public Light2D spotlight;

    [Header("타겟 메타 정보")]
    [TextArea]
    public string dialogueText;        // 맞았을 때 보여줄 대사
    public string nextSceneName;       // BulletCount 이후 이동할 씬 이름
    public bool reduceBullet = true;   // 총알 줄일지 여부
    void Awake()
    {
        if (spotlight != null)
            spotlight.enabled = false;  // 시작 시 꺼짐
    }
    public IEnumerator PlayHitEffect()
    {
        if (hitEffect != null)
        {
            hitEffect.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            hitEffect.SetActive(false);
        }
    }
    public void EnableSpotlight(bool enable)
    {
        if (spotlight != null)
            spotlight.enabled = enable;
    }
}
