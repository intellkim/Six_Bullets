using UnityEngine;
using System.Collections;  // 코루틴을 쓰려면 필요

public class Target : MonoBehaviour
{
    public GameObject hitEffect;  // 타겟 맞았을 때 켜질 이펙트

    public IEnumerator PlayHitEffect()
    {
        hitEffect.SetActive(true);               // 이펙트 켜기
        yield return new WaitForSeconds(0.15f);  // 0.15초 기다림
        hitEffect.SetActive(false);              // 이펙트 끄기
    }
}
