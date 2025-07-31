using UnityEngine;
using Cinemachine;

public class DynamicZoom : MonoBehaviour
{
    public Rigidbody2D playerRigidbody;
    public CinemachineVirtualCamera virtualCamera;

    public float minZoom = 5f;
    public float maxZoom = 12f;
    public float maxSpeed = 20f;

    void Update()
    {
        float speed = playerRigidbody.linearVelocity.magnitude;
        float t = Mathf.Clamp01(speed / maxSpeed);
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, t);

        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
            virtualCamera.m_Lens.OrthographicSize,
            targetZoom,
            Time.deltaTime * 3f // smoothing
        );
    }
}
