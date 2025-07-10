using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float parallaxFactor = 0.5f;
    private Transform cam;
    private Vector3 previousCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        previousCamPos = cam.position;
    }

    void Update()
    {
        Vector3 delta = cam.position - previousCamPos;
        transform.position += new Vector3(delta.x * parallaxFactor, 0, 0);
        previousCamPos = cam.position;
    }
}
