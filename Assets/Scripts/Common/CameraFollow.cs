using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 0, -10f);  // 카메라 거리 조절
    public float followSpeed = 5f;  // 따라가는 속도

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
