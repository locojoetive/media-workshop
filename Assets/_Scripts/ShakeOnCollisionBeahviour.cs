using UnityEngine;

public class ShakeOnCollisionBeahviour : MonoBehaviour
{
    [Header("Settings")]
    public float impactThreshold = 10f;
    public float shakeIntensity = 0.5f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > impactThreshold)
        {
            ShakeCamera.Instance.Shake(shakeIntensity);
        }
    }
}
